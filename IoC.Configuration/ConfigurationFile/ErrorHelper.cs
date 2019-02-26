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

using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class ErrorHelper : IErrorHelper
    {
        #region IErrorHelper Interface Implementation

        public string GenerateElementError(IConfigurationFileElement configurationFileElement, string message, IConfigurationFileElement parentElement = null)
        {
            var message2 = new StringBuilder();

            message2.AppendLine($"Error in element '{(parentElement != null ? parentElement : configurationFileElement).ElementName}':");

            message2.Append(message);
            message2.AppendLine();

            message2.Append(GenerateElementInTreeDetails(configurationFileElement));

            return message2.ToString();
        }

        #endregion

        #region Member Functions

        private string GenerateElementInTreeDetails([NotNull] IConfigurationFileElement configurationFileElement)
        {
            var structure = new StringBuilder();

            structure.AppendLine("Element location in configuration file:");
            var xmlElements = new LinkedList<IConfigurationFileElement>();

            var parentElement = configurationFileElement.Parent;

            while (parentElement != null)
            {
                xmlElements.AddFirst(parentElement);
                parentElement = parentElement.Parent;
            }

            var level = 0;

            // Add details about parents
            foreach (var parentXmlElement in xmlElements)
            {
                structure.Append(new string('\t', level));
                structure.AppendLine(parentXmlElement.ToString());
                ++level;
            }

            //Add some details about siblings 
            var siblingElements = configurationFileElement.Parent.Children;
            var indentation = new string('\t', level);
            var indexInParent = -1;

            for (var i = 0; i < siblingElements.Count; ++i)
            {
                var siblingElement = siblingElements[i];

                if (siblingElement == configurationFileElement)
                {
                    indexInParent = i;
                    break;
                }

                // We show only the first 2 elements and the last element in parent. 
                if (i < 2 || i == siblingElements.Count - 1 || siblingElements[i + 1] == configurationFileElement)
                    structure.AppendLine($"{indentation}{siblingElement.XmlElementToString()}");
                else if (i == 2)
                    structure.AppendLine($"{indentation}...");
            }

            if (indexInParent < 0)
                indexInParent = siblingElements.Count;

            structure.AppendLine();
            // Add details about current element
            structure.Append($"{indentation}{configurationFileElement.XmlElementToString()}");
            structure.Append($" <--- Element '{configurationFileElement.ElementName}' is the {indexInParent + 1}-th child element of element '{configurationFileElement.Parent.ElementName}'.");
            structure.AppendLine();

            return structure.ToString();
        }

        #endregion
    }
}