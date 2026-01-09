using System.Xml;
using IoC.Configuration.AttributeValueTransformer;

namespace MauiDemo;

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
                newAttributeValue = Path.Combine(MauiDemo.Properties.Settings.Default.IoCConfigurationDllsPath, xmlAttribute.Value.Replace('/', Path.DirectorySeparatorChar));
                return true;
            default:
                return false;
        }
    }
}
