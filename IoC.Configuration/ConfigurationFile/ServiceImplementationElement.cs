using System;
using System.Xml;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class ServiceImplementationElement : ServiceImplementationElementAbstr
    {
        #region Member Variables

        private DiResolutionScope _resolutionScope;

        #endregion

        #region  Constructors

        public ServiceImplementationElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent, [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent, assemblyLocator)
        {
        }

        #endregion

        #region Member Functions

        public override void Initialize()
        {
            base.Initialize();

            var resolutionScopeValue = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Scope);

            var resolutionScopeParsed = false;

            if (resolutionScopeValue.Length > 2)
            {
                resolutionScopeValue = $"{char.ToUpper(resolutionScopeValue[0])}{resolutionScopeValue.Substring(1)}";

                if (Enum.TryParse(resolutionScopeValue, out _resolutionScope))
                    resolutionScopeParsed = true;
            }

            if (!resolutionScopeParsed)
                throw new ConfigurationParseException(this, $"Invalid value specified for resolution scope: '{resolutionScopeValue}'.");

            //if (!_configuration.DiManagers.ActiveDiManagerElement.DiManager.SupportsResolutionScope(_resolutionScope))
            //    throw new ConfigurationParseException(this, $"DI container '{_configuration.DiManagers.ActiveDiManagerElement.DiManager.DiContainerName}' does not support a resolution scope '{resolutionScopeValue}'");
        }

        public override DiResolutionScope ResolutionScope => _resolutionScope;

        #endregion
    }
}