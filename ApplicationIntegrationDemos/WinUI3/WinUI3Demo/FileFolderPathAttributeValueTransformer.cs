using IoC.Configuration.AttributeValueTransformer;
using System.IO;
using System.Xml;

namespace WinUI3Demo;

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

                // Do any necessary path conversions here, such as commented out conversions below:
                // Get application execution path 
                //var applicationPath = AppContext.BaseDirectory;
                newAttributeValue = Path.Combine(WinUI3Demo.Properties.Settings.Default.IoCConfigurationDllsPath, xmlAttribute.Value.Replace('/', Path.DirectorySeparatorChar));

                //newAttributeValue = xmlAttribute.Value;
                return true;
            default:
                return false;
        }
    }
}