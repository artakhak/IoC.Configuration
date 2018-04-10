using System.Xml;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class DiManagerElement : ObjectInstanceElementAbstr<IDiManager>, IDiManagerElement
    {
        #region  Constructors

        public DiManagerElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent, [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent, assemblyLocator)
        {
        }

        #endregion

        #region IDiManagerElement Interface Implementation

        public IDiManager DiManager => Instance;

        public override void Initialize()
        {
            base.Initialize();
            Name = this.GetNameAttributeValue();
        }

        public string Name { get; private set; }

        #endregion
    }
}