using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using OROptimizer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.DynamicCode;
using OROptimizer.Serializer;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace IoC.Configuration.DynamicCode
{
    public class TypesListFactoryTypeGenerator : ITypesListFactoryTypeGenerator
    {
        #region Member Variables

        [NotNull]
        private readonly ITypeBasedSimpleSerializerAggregator _typeBasedSimpleSerializerAggregator;

        #endregion

        #region  Constructors

        public TypesListFactoryTypeGenerator([NotNull] ITypeBasedSimpleSerializerAggregator typeBasedSimpleSerializerAggregator)
        {
            _typeBasedSimpleSerializerAggregator = typeBasedSimpleSerializerAggregator;
        }

        #endregion

        #region ITypesListFactoryTypeGenerator Interface Implementation

        public IGeneratedTypeInfo GenerateType(IDynamicAssemblyBuilder dynamicAssemblyBuilder, Type interfaceToImplement,
                                               string dynamicImplementationsNamespace,
                                               IEnumerable<Type> returnedInstanceTypesForDefaultCase,
                                               IEnumerable<string[]> selectorParameterValues,
                                               IEnumerable<IEnumerable<Type>> returnedInstanceTypesForSelectorParameterValues)
        {
            LogHelper.Context.Log.InfoFormat("Generating dynamic implementation for interface '{0}'.", interfaceToImplement.FullName);
            try
            {
                string errorMessage;
                MethodInfo implementedMethodInfo;
                Type returnedItemsType = null;
                if (!ValidateImplementedInterface(interfaceToImplement, out implementedMethodInfo, out returnedItemsType, out errorMessage))
                    throw new Exception(errorMessage);

                if (returnedInstanceTypesForDefaultCase == null)
                    throw new Exception($"Some types should be provided to handle the default case.");

                if (selectorParameterValues?.Count() != returnedInstanceTypesForSelectorParameterValues?.Count())
                    throw new Exception($"The values of parameters  {nameof(selectorParameterValues)} and {nameof(returnedInstanceTypesForSelectorParameterValues)} should either be both nulls, or they should be collections that contain similar number of items.");

                var returnedInstanceTypesForSelectorParameterValuesAndElseSection = new List<IEnumerable<Type>>();

                if (returnedInstanceTypesForSelectorParameterValues != null)
                {
                    foreach (var returnTypes in returnedInstanceTypesForSelectorParameterValues)
                        ValidateReturnedValueTypesParameters(returnTypes, returnedItemsType);

                    returnedInstanceTypesForSelectorParameterValuesAndElseSection.AddRange(returnedInstanceTypesForSelectorParameterValues);
                }

                ValidateReturnedValueTypesParameters(returnedInstanceTypesForDefaultCase, returnedItemsType);
                returnedInstanceTypesForSelectorParameterValuesAndElseSection.Add(returnedInstanceTypesForDefaultCase);

                foreach (var selectorParameterValuesCurrent in selectorParameterValues)
                    if (!ValidateParameterValues(implementedMethodInfo, selectorParameterValuesCurrent, out errorMessage))
                        throw new Exception(errorMessage);

                var paremeters = implementedMethodInfo.GetParameters();
                var generatedTypeInfo = GenerateCSharpFile(interfaceToImplement, returnedItemsType, dynamicImplementationsNamespace, implementedMethodInfo.Name, paremeters,
                    new List<string[]>(selectorParameterValues),
                    returnedInstanceTypesForSelectorParameterValuesAndElseSection);

                //dynamicAssemblyBuilder.AddCSharpFile(generatedTypeInfo.CSharpFileContents);

                LogHelper.Context.Log.InfoFormat("Generation of dynamic implementation for interface '{0}' is complete.", interfaceToImplement.FullName);
                return generatedTypeInfo;
            }
            catch (Exception e)
            {
                dynamicAssemblyBuilder.SetIsAborted();
                LogHelper.Context.Log.Error(e);
                throw e;
            }
        }

        public bool ValidateImplementedInterface(Type interfaceToImplement, out MethodInfo implementedMethodInfo, out Type returnedItemsType, out string errorMessage)
        {
            errorMessage = null;
            implementedMethodInfo = null;
            returnedItemsType = null;

            if (!interfaceToImplement.IsInterface)
            {
                errorMessage = $"Type {interfaceToImplement.FullName} is not an interface.";
                return false;
            }

            var interfaceMembers = interfaceToImplement.GetMembers();
            if (interfaceMembers.Length != 1 || !(interfaceMembers[0] is MethodInfo))
            {
                errorMessage = $"Interface {interfaceToImplement.FullName} should have exactly one method, and no properties.";
                return false;
            }

            implementedMethodInfo = (MethodInfo) interfaceMembers[0];

            if (implementedMethodInfo.ReturnType.IsGenericType)
            {
                var implementedInterfaces = implementedMethodInfo.ReturnType.GetInterfaces();
                var genericTypeArguments = implementedMethodInfo.ReturnType.GenericTypeArguments;

                if (implementedInterfaces?.Length == 1 && implementedInterfaces[0] == typeof(IEnumerable) &&
                    implementedMethodInfo.ReturnType.UnderlyingSystemType.FullName.StartsWith("System.Collections.Generic.IEnumerable`", StringComparison.Ordinal) &&
                    genericTypeArguments?.Length == 1)
                    returnedItemsType = genericTypeArguments[0];
            }

            if (returnedItemsType == null || !returnedItemsType.IsInterface)
            {
                errorMessage = string.Format("The return type of method {0}.{1}(...) should be System.Collections.Generic.IEnumerable<T>, where T is any interface.{2}A valid example is: System.Collections.Generic.IEnumerable<IActionValidator>.",
                    interfaceToImplement.FullName,
                    implementedMethodInfo.Name,
                    Environment.NewLine);
                return false;
            }

            return true;
        }

        public bool ValidateParameterValues(MethodInfo implementedMethodInfo, IEnumerable<string> selectorParameterValues, out string errorMessage)
        {
            errorMessage = null;
            var interfaceToImplement = implementedMethodInfo.DeclaringType;

            var parameters = implementedMethodInfo.GetParameters();

            var selectorParameterValuesList = selectorParameterValues == null ? new List<string>() : new List<string>(selectorParameterValues);

            var numberOfParameterSelectors = selectorParameterValuesList.Count;

            if (numberOfParameterSelectors == 0)
            {
                errorMessage = "At least one selector parameter should be specified.";
                return false;
            }

            if (numberOfParameterSelectors > parameters.Length)
            {
                errorMessage = $"Number of expected parameters in '{interfaceToImplement.FullName}.{implementedMethodInfo.Name}(...)' is {parameters.Length}. Number of supplied parameters selector values is {(selectorParameterValuesList == null ? 0 : selectorParameterValuesList.Count)}. The number of parameter selectors cannot be greater than number of function parameters.";
                return false;
            }

            for (var paramIndex = 0; paramIndex < numberOfParameterSelectors; ++paramIndex)
            {
                var parameter = parameters[paramIndex];

                if (parameter.IsOut || parameter.ParameterType.IsByRef)
                {
                    errorMessage = $"Parameter {parameter.Name} in '{interfaceToImplement.FullName}.{implementedMethodInfo.Name}(...)' is an output or by reference parameter.";
                    return false;
                }

                if (!_typeBasedSimpleSerializerAggregator.HasSerializerForType(parameter.ParameterType))
                {
                    var message = new StringBuilder();

                    message.AppendFormat("Parameter '{0}' in '{1}.{2}(...)' has invalid type '{3}'. Only the following types are supported as selector value parameters:{4}",
                        parameter.Name,
                        interfaceToImplement.FullName, implementedMethodInfo.Name,
                        parameter.ParameterType, Environment.NewLine);

                    var i = 0;
                    foreach (var serializer in _typeBasedSimpleSerializerAggregator.GetRegisteredSerializers())
                    {
                        if (i > 0)
                            message.Append(", ");

                        message.Append($"'{serializer.SerializedType.FullName}'");
                        ++i;
                    }

                    message.Append(".");
                    errorMessage = message.ToString();
                    return false;
                }

                if (!_typeBasedSimpleSerializerAggregator.TryDeserialize(parameter.ParameterType, selectorParameterValuesList[paramIndex], out var deserializedValue) || deserializedValue == null)
                {
                    errorMessage = string.Format("Failed to deserialize the value for parameter '{0}' from '{1}' to '{2}' in '{3}.{4}(...)'.",
                        parameter.Name, selectorParameterValuesList[paramIndex] ?? "null",
                        parameter.ParameterType.FullName,
                        interfaceToImplement.FullName, implementedMethodInfo.Name);
                    return false;
                }
            }

            return true;
        }

        public bool ValidateReturnedType(Type specifiedReturnedType, Type returnedItemsType, out string errorMessage)
        {
            errorMessage = null;

            if (specifiedReturnedType.IsInterface || specifiedReturnedType.IsAbstract)
            {
                errorMessage = string.Format("Type {0} is an {1}. Only concrete classes are allowed.",
                    specifiedReturnedType.FullName,
                    specifiedReturnedType.IsInterface ? "interface" : "abstract class");
                return false;
            }

            if (!(specifiedReturnedType == returnedItemsType ||
                  returnedItemsType.IsInterface && specifiedReturnedType.GetInterfaces().Contains(returnedItemsType) ||
                  returnedItemsType.IsAssignableFrom(specifiedReturnedType)))
            {
                errorMessage = $"Type '{specifiedReturnedType.FullName}' cannot be converted to '{returnedItemsType.FullName}'.";
                return false;
            }

            return true;
        }

        #endregion

        #region Member Functions

        private GeneratedTypeInfo GenerateCSharpFile([NotNull] Type interfaceToImplement, [NotNull] Type returnedItemsType,
                                                     [NotNull] string dynamicImplementationsNamespace,
                                                     [NotNull] string implementedMethodName,
                                                     [NotNull] [ItemNotNull] ParameterInfo[] parameters,
                                                     [NotNull] [ItemNotNull] IReadOnlyList<string[]> serializedSelectorParameterValues,
                                                     [NotNull] [ItemNotNull] IReadOnlyList<IEnumerable<Type>> returnedInstanceTypesForSelectorParameterValuesAndElseSection)
        {
            var className = $"{interfaceToImplement.Name}_{GlobalsCoreAmbientContext.Context.GenerateUniqueId()}";

            var classStrBldr = new StringBuilder(1000);

            classStrBldr.AppendLine("using System;");
            classStrBldr.AppendLine("using System.Collections.Generic;");

            classStrBldr.AppendLine($"namespace {dynamicImplementationsNamespace}");
            classStrBldr.AppendLine("{");
            classStrBldr.AppendLine($"public class {className}: {interfaceToImplement.FullName}");
            classStrBldr.AppendLine("{");

            classStrBldr.AppendLine($"private List<object[]> _selectorValues;");
            classStrBldr.AppendLine($"private {typeof(IDiContainer).FullName} _diContainer;");
            classStrBldr.AppendLine($"public {className}({typeof(IDiContainer).FullName} diContainer, {typeof(ITypeBasedSimpleSerializerAggregator).FullName} serializerAggregator)");
            classStrBldr.AppendLine("{");
            classStrBldr.AppendLine($"_diContainer = diContainer;");
            classStrBldr.AppendLine($"_selectorValues = new List<object[]>({serializedSelectorParameterValues.Count});");

            classStrBldr.AppendLine("object deserializedValue;");

            foreach (var serializedSelectorValues in serializedSelectorParameterValues)
            {
                // When loading the configuration, throw an exception if number of selectors is not the same as number of parameters in implemented interface.
                classStrBldr.AppendLine("{");
                classStrBldr.AppendLine($"var deserializedValues = new object[{serializedSelectorValues.Length}];");
                classStrBldr.AppendLine("_selectorValues.Add(deserializedValues);");

                for (var i = 0; i < serializedSelectorValues.Length; ++i)
                {
                    classStrBldr.AppendLine($"if (!serializerAggregator.TryDeserialize(typeof({parameters[i].ParameterType.FullName}), \"{serializedSelectorValues[i]}\", out deserializedValue))");
                    classStrBldr.AppendLine($" throw new Exception(\"Failed to convert '{serializedSelectorValues[i]}' to '{parameters[i].ParameterType.FullName}'.\");");
                    classStrBldr.AppendLine($"deserializedValues[{i}]=deserializedValue;");
                }

                classStrBldr.AppendLine("}");
            }

            classStrBldr.AppendLine("}");

            classStrBldr.Append($"public System.Collections.Generic.IEnumerable<{returnedItemsType.FullName}> {implementedMethodName}(");

            for (var i = 0; i < parameters.Length; ++i)
            {
                if (i > 0)
                    classStrBldr.Append(", ");

                var parameterInfo = parameters[i];

                classStrBldr.Append(parameterInfo.ParameterType.FullName);
                classStrBldr.Append($" {parameterInfo.Name}");
            }

            classStrBldr.Append(")");
            classStrBldr.AppendLine();
            classStrBldr.AppendLine("{");

            classStrBldr.AppendLine($"var returnedValues = new System.Collections.Generic.List<{returnedItemsType.FullName}>();");

            for (var i = 0; i < returnedInstanceTypesForSelectorParameterValuesAndElseSection.Count; ++i)
            {
                if (i < serializedSelectorParameterValues.Count)
                {
                    classStrBldr.Append("if (");

                    for (var parameterIndex = 0; parameterIndex < serializedSelectorParameterValues[i].Length; ++parameterIndex)
                    {
                        if (parameterIndex > 0)
                            classStrBldr.Append(" && ");

                        var parameter = parameters[parameterIndex];

                        classStrBldr.Append("(");

                        if (parameter.ParameterType.IsClass)
                            classStrBldr.Append($"{parameter.Name} != null && ");

                        classStrBldr.Append($"{parameter.Name}.Equals(_selectorValues[{i}][{parameterIndex}])");
                        classStrBldr.Append(")");
                    }

                    classStrBldr.AppendLine(")");

                    classStrBldr.AppendLine("{");
                }

                var returnedInstanceTypes = returnedInstanceTypesForSelectorParameterValuesAndElseSection[i];

                // and resolve the types right away, so that resolution issues are found on application start.
                foreach (var type in returnedInstanceTypes)
                {
                    classStrBldr.AppendLine("try");
                    classStrBldr.AppendLine("{");

                    classStrBldr.AppendLine($"returnedValues.Add(_diContainer.Resolve<{type.FullName}>());");

                    classStrBldr.AppendLine("}");
                    classStrBldr.AppendLine("catch(Exception e)");
                    classStrBldr.AppendLine("{");

                    // Log that the type was not resolved and re-throw.
                    classStrBldr.AppendLine($"{typeof(LogHelper).FullName}.Context.Log.Error(\"Could not resolve the type '{type.FullName}'. Make sure to register the type in configuration file.\", e);");
                    classStrBldr.AppendLine("throw;");
                    classStrBldr.AppendLine("}");
                }

                classStrBldr.AppendLine("return returnedValues;");

                if (i < serializedSelectorParameterValues.Count)
                    classStrBldr.AppendLine("}");
            }

            //classStrBldr.AppendLine("return returnedValues;");
            classStrBldr.AppendLine("}");

            // Close class
            classStrBldr.AppendLine("}");

            // Close namespace
            classStrBldr.AppendLine("}");

            return new GeneratedTypeInfo($"{dynamicImplementationsNamespace}.{className}", classStrBldr.ToString());
        }

        private void ValidateReturnedValueTypesParameters(IEnumerable<Type> returnedInstanceTypes, Type returnedItemsType)
        {
            foreach (var returnedInstanceType in returnedInstanceTypes)
                if (!ValidateReturnedType(returnedInstanceType, returnedItemsType, out var errorMessage))
                    throw new Exception(errorMessage);
        }

        #endregion
    }
}