using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class WebApi : ConfigurationFileElementAbstr, IWebApi
    {
        #region  Constructors

        public WebApi([NotNull] XmlElement xmlElement, IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IWebApi Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IWebApiControllerAssemblies webApiControllerAssemblies)
                ControllerAssemblies = webApiControllerAssemblies;
        }

        public IWebApiControllerAssemblies ControllerAssemblies { get; private set; }

        #endregion
    }
}