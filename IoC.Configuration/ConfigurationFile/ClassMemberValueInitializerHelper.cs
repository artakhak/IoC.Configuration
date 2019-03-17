// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder;
using IoC.Configuration.DiContainerBuilder.FileBased;
using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    public class ClassMemberValueInitializerHelper : IClassMemberValueInitializerHelper
    {
        #region Member Variables

        private readonly Dictionary<Type, string> _injectedTypesToVariableNameCSharpFile = new Dictionary<Type, string>();

        [NotNull]
        private readonly IPluginAssemblyTypeUsageValidator _pluginAssemblyTypeUsageValidator;

        [NotNull]
        private readonly ITypeHelper _typeHelper;

        [NotNull]
        private readonly ITypeMemberLookupHelper _typeMemberLookupHelper;

        private const string ExamplesOfValidClassPaths = "Examples of valid class members are Namespace1.ConnectionTypes.SQLite, MyClassAlias.DefaultConnectionString, etc.";

        #endregion

        #region  Constructors

        public ClassMemberValueInitializerHelper([NotNull] ITypeHelper typeHelper,
                                                 [NotNull] IPluginAssemblyTypeUsageValidator pluginAssemblyTypeUsageValidator,
                                                 [NotNull] ITypeMemberLookupHelper typeMemberLookupHelper)
        {
            _typeHelper = typeHelper;
            _pluginAssemblyTypeUsageValidator = pluginAssemblyTypeUsageValidator;
            _typeMemberLookupHelper = typeMemberLookupHelper;
        }

        #endregion

        #region IClassMemberValueInitializerHelper Interface Implementation

        public string GenerateValueCSharp(ClassMemberData classMemberData, IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            var valueCSharpStrBldr = new StringBuilder();

            if (classMemberData.IsInjectedClassMember)
            {
                var dynamicallyGeneratedClass = dynamicAssemblyBuilder.GetDynamicallyGeneratedClass(DynamicCodeGenerationHelpers.ClassMembersClassName);

                if (!_injectedTypesToVariableNameCSharpFile.TryGetValue(classMemberData.ClassInfo.Type, out var variableName))
                {
                    variableName = $"{classMemberData.ClassInfo.Type.Name}_{GlobalsCoreAmbientContext.Context.GenerateUniqueId()}";

                    _injectedTypesToVariableNameCSharpFile[classMemberData.ClassInfo.Type] = variableName;

                    var variableInitializationCode = new StringBuilder();

                    variableInitializationCode.AppendFormat("public static {0} {1}=", classMemberData.ClassInfo.TypeCSharpFullName, variableName);

#pragma warning disable CS0612, CS0618
                    variableInitializationCode.Append($"{typeof(DiContainerBuilderConfiguration).FullName}.{nameof(DiContainerBuilderConfiguration.DiContainerStatic)}.{nameof(IDiContainer.Resolve)}");
#pragma warning restore CS0612, CS0618

                    variableInitializationCode.Append($"<{classMemberData.ClassInfo.TypeCSharpFullName}>();");

                    dynamicallyGeneratedClass.AddCodeLine(variableInitializationCode.ToString());
                }

                valueCSharpStrBldr.Append($"{dynamicallyGeneratedClass.ClassFullName}.{variableName}");
            }
            else
            {
                valueCSharpStrBldr.Append($"{classMemberData.ClassInfo.TypeCSharpFullName}");
            }

            // Add class member name
            valueCSharpStrBldr.Append($".{classMemberData.ClassMemberInfo.Name}");

            if (classMemberData.ClassMemberCategory == ClassMemberCategory.Method)
            {
                valueCSharpStrBldr.Append("(");
                valueCSharpStrBldr.Append(string.Join(",", classMemberData.Parameters.Select(x => x.GenerateValueCSharp(dynamicAssemblyBuilder))));
                valueCSharpStrBldr.Append(")");
            }
                

            return valueCSharpStrBldr.ToString();
        }

        public ClassMemberData GetClassMemberData(IConfigurationFileElement configurationFileElement, string classMemberPath,
                                                  IEnumerable<IParameter> parameters)
        {
            ITypeInfo classInfo = null;
            ITypeInfo memberTypeInfo;
            string classMemberName = null;

            Type[] parameterTypes = parameters.Select(x => x.ValueTypeInfo.Type).ToArray();

            var allMatchedMemberInfos = new List<MemberInfo>();
            var isStaticMember = true;
            var classMemberCategory = ClassMemberCategory.Field;

            var lastIndexofDot = classMemberPath.LastIndexOf('.');

            if (lastIndexofDot > 0 && lastIndexofDot + 1 < classMemberPath.Length)
                classMemberName = classMemberPath.Substring(lastIndexofDot + 1).Trim();

            if (string.IsNullOrEmpty(classMemberName))
                throw new ConfigurationParseException(configurationFileElement, $"No class member is specified in '{classMemberPath}'. {ExamplesOfValidClassPaths}");

            var typeFullName = classMemberPath.Substring(0, lastIndexofDot).Trim();

            if (typeFullName.Length == 0)
                throw new ConfigurationParseException(configurationFileElement, $"No class name is provided in {classMemberPath}. {ExamplesOfValidClassPaths}");

            INamedTypeDefinitionElement namedTypeDefinitionElement = null;
            if (!typeFullName.Contains("."))
            {
                namedTypeDefinitionElement = configurationFileElement.GetTypeDefinition(typeFullName);

                if (namedTypeDefinitionElement != null)
                {
                    classInfo = namedTypeDefinitionElement.ValueTypeInfo;
                    _pluginAssemblyTypeUsageValidator.Validate(configurationFileElement, classInfo);
                }
            }

            if (classInfo == null)
                classInfo = _typeHelper.GetTypeInfoFromTypeFullName(configurationFileElement, typeFullName);

            if (classInfo.Type.IsEnum)
            {
                if (classInfo.Type.GetEnumNames().FirstOrDefault(x => classMemberName.Equals(x, StringComparison.Ordinal)) == null)
                    throw new ConfigurationParseException(configurationFileElement, $"Enumeration value '{classMemberName}' is not defined in enumeration '{classInfo.TypeCSharpFullName}'.");

                allMatchedMemberInfos.Add(classInfo.Type.GetMember(classMemberName).First());
                classMemberCategory = ClassMemberCategory.EnumValue;
                isStaticMember = true;
                memberTypeInfo = classInfo;
            }
            else
            {
                Type memberReturnType = null;

                void processType(Type type, ref bool stopProcessingParam)
                {
                    MemberInfo memberInfo = null;

                    try
                    {
                        if (parameterTypes.Length > 0)
                        {
                            // Lets try to find a method first.
                            var methodInfo = type.GetMethods().FirstOrDefault(x =>
                                Helpers.IsMethodAMatch(x, classMemberName, parameterTypes));

                            if (methodInfo != null)
                            {
                                memberInfo = methodInfo;

                                classMemberCategory = ClassMemberCategory.Method;
                                isStaticMember = methodInfo.IsStatic;
                                memberReturnType = methodInfo.ReturnType;
                            }

                            return;
                        }

                        var memberInfos = type.GetMembers().Where(x =>
                            classMemberName.Equals(x.Name, StringComparison.Ordinal)).ToList();

                        if (memberInfos.Count == 0)
                            return;


                        if (memberInfos[0] is MethodInfo)
                        {
                            var methodInfo = (MethodInfo)memberInfos.FirstOrDefault(x => x is MethodInfo methodInfo2 &&
                                                                                         methodInfo2.IsPublic &&
                                                                                         methodInfo2.GetParameters().Length == 0);
                            if (methodInfo == null)
                                return;

                            memberInfo = methodInfo;
                            classMemberCategory = ClassMemberCategory.Method;
                            isStaticMember = methodInfo.IsStatic;
                            memberReturnType = methodInfo.ReturnType;
                        }
                        else
                        {
                            memberInfo = memberInfos[0];

                            if (memberInfo is FieldInfo fieldInfo)
                            {
                                if (!fieldInfo.IsPublic)
                                    return;
                               
                                classMemberCategory = ClassMemberCategory.Field;
                                isStaticMember = fieldInfo.IsStatic || fieldInfo.IsLiteral;
                                memberReturnType = fieldInfo.FieldType;
                            }
                            else if (memberInfo is PropertyInfo propertyInfo)
                            {
                                var getMethod = propertyInfo.GetGetMethod();

                                if (!getMethod.IsPublic)
                                    return;

                                classMemberCategory = ClassMemberCategory.Property;
                                isStaticMember = getMethod.IsStatic;
                                memberReturnType = getMethod.ReturnType;
                            }
                        }
                    }
                    finally
                    {
                        if (memberInfo != null)
                        {
                            allMatchedMemberInfos.Add(memberInfo);

                            if (type == classInfo.Type)
                                stopProcessingParam = true;
                        }
                    }
                }

                var stopProcessing = false;
                _typeMemberLookupHelper.ProcessTypeImplementedInterfacesAndBaseTypes(classInfo.Type, processType, ref stopProcessing);

                if (allMatchedMemberInfos.Count == 1)
                {
                    memberTypeInfo = _typeHelper.GetTypeInfoFromType(configurationFileElement, memberReturnType);
                }
                else
                {
                    var errorMessage = new StringBuilder();
                    errorMessage.AppendFormat("Member with name '{0}'", classMemberName);

                    if (parameterTypes.Length > 0)
                    {
                        errorMessage.AppendFormat(" and parameters of types: ({0})",
                            string.Join(",", parameters.Select(x => x.ValueTypeInfo.TypeCSharpFullName)));
                    }
                    else
                    {
                        errorMessage.Append(" and no parameters");
                    }


                    if (allMatchedMemberInfos.Count == 0)
                    {
                        errorMessage.AppendFormat(" was not found in type '{0}' or any of its parents.", classInfo.TypeCSharpFullName);
                        errorMessage.AppendLine();
                    }
                    else
                    {
                        errorMessage.AppendFormat(" was not found in type '{0}', however a member with this name occurs multiple times in parents of type '{0}'.",
                            classInfo.TypeCSharpFullName);
                        errorMessage.AppendLine();

                        errorMessage.AppendLine("Please specify one of the parent types where the member is declared.");

                        errorMessage.AppendFormat($"Note, if necessary, you can use '{0}' to proxy the service '{1}'.",
                            ConfigurationFileElementNames.ProxyService, classInfo.TypeCSharpFullName);
                        errorMessage.AppendLine();

                        errorMessage.AppendLine(MessagesHelper.GenerateProxyServiceExample(allMatchedMemberInfos[0].DeclaringType, classInfo.Type));

                        errorMessage.AppendLine(
                            string.Format("The following is the list of types, where the member named '{0}' was found: {1}",
                                classMemberName,
                                string.Join(", ", allMatchedMemberInfos.Select(x => x.DeclaringType.GetTypeNameInCSharpClass()))));
                    }

                    throw new ConfigurationParseException(configurationFileElement, errorMessage.ToString());
                }
            }

            return new ClassMemberData(classInfo, memberTypeInfo, allMatchedMemberInfos[0], parameters,  !isStaticMember, classMemberCategory);
        }

        public object GetValueWithReflection(IConfigurationFileElement configurationFileElement, ClassMemberData classMemberData)
        {
            object injectedObject = null;
            if (classMemberData.IsInjectedClassMember)
            {
#pragma warning disable CS0612, CS0618
                if (DiContainerBuilderConfiguration.DiContainerStatic == null)
                    throw new ConfigurationParseException(configurationFileElement,
                        $"Di container {DiContainerBuilderConfiguration.DiContainerStatic} is not yet initialized");

                injectedObject = DiContainerBuilderConfiguration.DiContainerStatic.Resolve(classMemberData.ClassInfo.Type);

                if (injectedObject == null)
                    throw new ConfigurationParseException(configurationFileElement, $"Failed to resolve type '{classMemberData.ClassInfo.TypeCSharpFullName}' from DI container.");
#pragma warning restore CS0612, CS0618
            }

            var classMemberInfo = classMemberData.ClassMemberInfo;

            if (classMemberInfo is PropertyInfo propertyInfo)
                classMemberInfo = propertyInfo.GetGetMethod();

            if (classMemberInfo is MethodInfo methodInfo)
            {
                //return methodInfo.Invoke(injectedObject, new object[0]);
                return methodInfo.Invoke(injectedObject, classMemberData.Parameters.Select(x =>
                   x.GenerateValue()).ToArray());
            }
                

            if (classMemberInfo is FieldInfo fieldInfo)
                return fieldInfo.GetValue(injectedObject);

            throw new ConfigurationParseException(configurationFileElement, $"Failed to get value from '{classMemberData.ClassInfo.TypeCSharpFullName}.{classMemberData.ClassMemberInfo.Name}'.");
        }

        #endregion
    }
}