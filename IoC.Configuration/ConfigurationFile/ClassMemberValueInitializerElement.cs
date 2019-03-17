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

using JetBrains.Annotations;
using OROptimizer.DynamicCode;
using System.Collections.Generic;
using System.Xml;

namespace IoC.Configuration.ConfigurationFile
{
    public class ClassMemberValueInitializerElement : ValueInitializerElement, IClassMemberValueInitializer
    {
        #region Member Variables
        [NotNull]
        private readonly IClassMemberValueInitializerHelper _classMemberValueInitializerHelper;

        private IParameters _parameters;
        #endregion

        #region  Constructors

        public ClassMemberValueInitializerElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent,
                                                  [NotNull] ITypeHelper typeHelper,
                                                  IClassMemberValueInitializerHelper classMemberValueInitializerHelper) : base(xmlElement, parent, typeHelper)
        {
            _classMemberValueInitializerHelper = classMemberValueInitializerHelper;
        }

        #endregion

        #region IClassMemberValueInitializer Interface Implementation

        public ClassMemberData ClassMemberData { get; private set; }
        

        #endregion

        #region Member Functions

        protected override string DoGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            return _classMemberValueInitializerHelper.GenerateValueCSharp(ClassMemberData, dynamicAssemblyBuilder);
        }

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IParameters parameters)
                _parameters = parameters;
        }


        /// <summary>
        ///     Generates the value using reflection. Use this value only at early stages of loading the configuration,
        ///     when the DI container is not yet initialized.
        /// </summary>
        /// <returns></returns>
        public override object GenerateValue()
        {
            return _classMemberValueInitializerHelper.GetValueWithReflection(this, ClassMemberData);
        }


        protected override ITypeInfo GetValueTypeInfo()
        {
            var classInfo = TypeHelper.GetTypeInfo(this, ConfigurationFileAttributeNames.DeclaringClass,
                ConfigurationFileAttributeNames.Assembly, ConfigurationFileAttributeNames.DeclaringClassRef);

            IEnumerable<IParameter> parameters;

            if (_parameters == null)
                parameters = new List<IParameter>();
            else
                parameters = _parameters.AllParameters;

            var memberName = GetAttributeValue(ConfigurationFileAttributeNames.MemberName);
            ClassMemberData = _classMemberValueInitializerHelper.GetClassMemberData(this, $"{classInfo.TypeCSharpFullName}.{memberName}", parameters);

            return ClassMemberData.MemberTypeInfo;
        }

        /// <summary>Gets a value indicating whether this instance is resolved from di container.</summary>
        public override bool IsResolvedFromDiContainer => ClassMemberData.IsInjectedClassMember;

        #endregion
    }
}