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

using JetBrains.Annotations;
using System;
using System.IO;
using System.Xml;

namespace IoC.Configuration.AttributeValueTransformer
{
    /// <summary>
    /// Default value provider for attribute 'path' in XML configuration element "/iocConfiguration/appDataDir".
    /// Returns a path where dynamically generated files are saved. Example of value returned for "path" attribute value in element
    /// /iocConfiguration/appDataDir" is "C:\Users\johnsmith\AppData\Local\MyApp\DynamicFiles". 
    /// </summary>
    public class AppDataDirAttributeValueTransformer : IAttributeValueTransformer
    {
        [NotNull] private readonly string _appName;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="appName">
        /// Application name. Example "SmartXML" or "UniversalExpressionParser.TestTool".
        /// The app name will be used in generated path.</param>
        public AppDataDirAttributeValueTransformer([NotNull] string appName)
        {
            _appName = appName;
        }

        public bool TryGetAttributeValue(string elementPath, XmlAttribute xmlAttribute, out string newAttributeValue)
        {
            newAttributeValue = null;

            if (string.IsNullOrWhiteSpace(xmlAttribute.Value) ||
                !string.Equals(elementPath, "/iocConfiguration/appDataDir", StringComparison.Ordinal) ||
                !string.Equals(xmlAttribute.Name, "path", StringComparison.Ordinal))
                return false;

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _appName, "DynamicFiles");

            newAttributeValue = path;
            return true;
        }
    }
}