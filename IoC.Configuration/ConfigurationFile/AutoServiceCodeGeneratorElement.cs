// This software is part of the IoC.Configuration library
// Copyright � 2018 IoC.Configuration Contributors
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

using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.ConfigurationFile
{
    public class AutoServiceCodeGeneratorElement : ConfigurationFileElementAbstr, IAutoServiceCodeGeneratorElement
    {
        public AutoServiceCodeGeneratorElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }        

        public ICustomAutoServiceCodeGenerator CustomAutoServiceCodeGenerator { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            var valueInitializerElement = Children.FirstOrDefault(x => x is IValueInitializerElement) as IValueInitializerElement;

            var customAutoServiceCodeGeneratorType = typeof(ICustomAutoServiceCodeGenerator);

            if (!customAutoServiceCodeGeneratorType.IsAssignableFrom(valueInitializerElement.ValueTypeInfo.Type))
                throw new ConfigurationParseException(this, string.Format("The type of a value specified under '{0}' should implement interface '{1}'. The specified type is '{2}'.",
                    ConfigurationFileElementNames.AutoServiceCodeGenerator,
                    customAutoServiceCodeGeneratorType.GetTypeNameInCSharpClass(),
                    valueInitializerElement.ValueTypeInfo.Type.GetTypeNameInCSharpClass()));

            CustomAutoServiceCodeGenerator = (ICustomAutoServiceCodeGenerator) valueInitializerElement.GenerateValue();
        }
    }
}