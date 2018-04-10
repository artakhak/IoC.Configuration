using System;
using System.IO;
using System.Xml;
using IoC.Configuration.ConfigurationFile;

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