
using OROptimizer.Diagnostics.Log;
using System.IO;
using System.Xml;
using IoC.Configuration.AttributeValueTransformer;
using TestsSharedLibrary;

namespace IoC.Configuration.Tests;

public class FileFolderPathAttributeValueTransformer : IAttributeValueTransformer
{
    public bool TryGetAttributeValue(string elementPath, XmlAttribute xmlAttribute, out string newAttributeValue)
    {
        newAttributeValue = null;

        if (!xmlAttribute.Value.StartsWith(@"TestFiles\"))
            return false;

        switch (xmlAttribute.Name)
        {
            case "path":
            case "probingPath":
            case "overrideDirectory":
            case "pluginsDirPath":

                var result =
                    TestsHelper.TryGetFilePathRelativeToTestProjectFolder("IoC.Configuration.Tests",
                        typeof(IoC.Configuration.Tests.TypeInfoTests), Path.Combine("bin", xmlAttribute.Value));

                if (!result.isSuccess)
                {
                    LogHelper.Context.Log.ErrorFormat("Failed to parse a file path from '{0}'. Error: {1}",
                        xmlAttribute.Value, result.errorMessage);
                    return false;
                }

                newAttributeValue = result.absoluteFilePath;
                return true;
            default:
                return false;
        }
    }
}