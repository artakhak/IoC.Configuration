using IoC.Configuration.AttributeValueTransformer;
using System.Xml;

namespace WebApiDemo.Startup;

public class FileFolderPathAttributeValueTransformer: IAttributeValueTransformer
{
    /// <inheritdoc />
    public bool TryGetAttributeValue(string elementPath, XmlAttribute xmlAttribute, out string? newAttributeValue)
    {
        newAttributeValue = null;

        if (!xmlAttribute.Value.StartsWith(@"IoCConfigurationFiles/"))
            return false;

        switch (xmlAttribute.Name)
        {
            case "path":
            case "probingPath":
            case "overrideDirectory":
            case "pluginsDirPath":

                // Get application execution path 
                var applicationPath = AppContext.BaseDirectory;
                newAttributeValue = Path.Combine(applicationPath, ReplacePathSeparatorCharactersWithOsSeparators(xmlAttribute.Value));
                return true;
            default:
                return false;
        }
    }

    private static string ReplacePathSeparatorCharactersWithOsSeparators(string path)
    {
        return path.Replace('/', Path.DirectorySeparatorChar);
    }
}