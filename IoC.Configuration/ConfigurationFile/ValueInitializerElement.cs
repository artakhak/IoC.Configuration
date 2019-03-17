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

using System.Xml;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class ValueInitializerElement : ConfigurationFileElementAbstr, IValueInitializerElement
    {
        #region Member Variables

        [NotNull]
        private bool _addCodeGenerateValueCSharpWasCalled;

        #endregion

        #region  Constructors

        public ValueInitializerElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent,
                                       [NotNull] ITypeHelper typeHelper) :
            base(xmlElement, parent)
        {
            TypeHelper = typeHelper;
        }

        #endregion

        #region IValueInitializerElement Interface Implementation

        public string GenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            if (!_addCodeGenerateValueCSharpWasCalled)
            {
                _addCodeGenerateValueCSharpWasCalled = true;
                AddCodeOnGenerateValueCSharp(dynamicAssemblyBuilder);
            }

            return DoGenerateValueCSharp(dynamicAssemblyBuilder);
        }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            ValueTypeInfo = GetValueTypeInfo();
        }

        /// <summary>
        ///     Can be null only if the parameter is declared with either 'object' or injectedObject elements, and the object type
        ///     referenced is in a disabled assembly.
        /// </summary>
        public ITypeInfo ValueTypeInfo { get; private set; }

        #endregion

        #region Current Type Interface

        protected virtual void AddCodeOnGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
        }

        protected abstract string DoGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder);
        public abstract object GenerateValue();

        protected virtual ITypeInfo GetValueTypeInfo()
        {
            return TypeHelper.GetTypeInfo(this, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly, ConfigurationFileAttributeNames.TypeRef);
        }

        public abstract bool IsResolvedFromDiContainer { get; }

        #endregion

        #region Member Functions

        [NotNull]
        protected ITypeHelper TypeHelper { get; }

        #endregion
    }
}