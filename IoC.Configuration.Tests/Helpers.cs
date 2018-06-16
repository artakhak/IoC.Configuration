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
using System.IO;
using System.Xml;
using IoC.Configuration.ConfigurationFile;
using TestsSharedLibrary.DependencyInjection;

namespace IoC.Configuration.Tests
{
    public static class Helpers
    {
        #region Member Variables

        public static readonly string TestsEntryAssemblyFolder = Path.GetDirectoryName(typeof(Helpers).Assembly.Location);

        #endregion

        #region Member Functions

        public static T GetPropertyValue<T>(object propertyOwnerObject, string propertyName)
        {
            var propertyInfo = propertyOwnerObject.GetType().GetProperty(propertyName);
            var propertyValue = propertyInfo.GetValue(propertyOwnerObject);
            return propertyValue == null ? default(T) : (T) propertyValue;
        }

        public static string GetPropertyValueToString(object propertyOwnerObject, string propertyName)
        {
            var propertyValue = GetPropertyValue<object>(propertyOwnerObject, propertyName);
            return propertyValue?.ToString();
        }

        public static string GetTestDllsFolderPath()
        {
            var entryAssemblyFolder = TestsEntryAssemblyFolder;

            var projectFolderName = "IoC.Configuration.Tests";

            var indexOfProject = entryAssemblyFolder.LastIndexOf(projectFolderName, StringComparison.OrdinalIgnoreCase);

            if (indexOfProject >= 0)
                return Path.Combine(entryAssemblyFolder.Substring(0, indexOfProject + projectFolderName.Length), "TestDlls");

            return null;
        }

        public static Type GetType(string typeFullName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeFullName);
                if (type != null)
                    return type;
            }

            throw new Exception($"Type '{typeFullName}' was not found.");
        }

        public static void ReplaceActiveDiManagerInConfigurationFile(XmlDocument xmlDocument, DiImplementationType diImplementationType)
        {
            string activeDiManagerName = null;

            switch (diImplementationType)
            {
                case DiImplementationType.Autofac:
                case DiImplementationType.Ninject:
                    activeDiManagerName = diImplementationType.ToString();
                    break;
                default:
                    throw new Exception($"The value is not handled: {diImplementationType}.");
            }

            xmlDocument.SelectElement("/iocConfiguration/diManagers", x => true)
                       .SetAttributeValue(ConfigurationFileAttributeNames.ActiveDiManagerName, activeDiManagerName);
        }

        #endregion
    }
}