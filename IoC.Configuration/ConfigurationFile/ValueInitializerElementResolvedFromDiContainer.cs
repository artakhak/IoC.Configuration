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
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainerBuilder;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    public class ValueInitializerElementResolvedFromDiContainer : ValueInitializerElement
    {
        #region  Constructors

        public ValueInitializerElementResolvedFromDiContainer([NotNull] XmlElement xmlElement,
                                                              IConfigurationFileElement parent,
                                                              [NotNull] ITypeHelper typeHelper) :
            base(xmlElement, parent, typeHelper)
        {
        }

        #endregion

        #region Member Functions

        protected override string DoGenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
#pragma warning disable CS0612, CS0618
            return $"{typeof(DiContainerBuilderConfiguration).FullName}.{nameof(DiContainerBuilderConfiguration.DiContainerStatic)}.{nameof(IDiContainer.Resolve)}<{ValueTypeInfo.TypeCSharpFullName}>()";
#pragma warning restore CS0612, CS0618
        }

        public override object GenerateValue()
        {
#pragma warning disable CS0612, CS0618
            if (DiContainerBuilderConfiguration.DiContainerStatic == null)
                throw new ConfigurationParseException(this,
                    $"Di container {DiContainerBuilderConfiguration.DiContainerStatic} is not yet initialized");

            return DiContainerBuilderConfiguration.DiContainerStatic.Resolve(ValueTypeInfo.Type);
#pragma warning restore CS0612, CS0618
        }

        public override bool IsResolvedFromDiContainer => true;

        #endregion
    }
}