using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class WebApiControllerAssemblies : ConfigurationFileElementAbstr, IWebApiControllerAssemblies
    {
        #region Member Variables

        [NotNull]
        private readonly IList<IWebApiControllerAssembly> _assemblies = new List<IWebApiControllerAssembly>();

        #endregion

        #region  Constructors

        public WebApiControllerAssemblies([NotNull] XmlElement xmlElement, IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IWebApiControllerAssemblies Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IWebApiControllerAssembly webApiControllerAssembly)
                _assemblies.Add(webApiControllerAssembly);
        }

        public IEnumerable<IWebApiControllerAssembly> Assemblies => _assemblies;

        #endregion
    }
}