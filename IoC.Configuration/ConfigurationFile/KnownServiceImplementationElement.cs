using System;
using System.Collections.Generic;
using System.Xml;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class KnownServiceImplementationElement : ServiceImplementationElementAbstr, IServiceElement
    {
        #region Member Variables

        #endregion

        #region  Constructors

        public KnownServiceImplementationElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                                 [NotNull] Type implementedServiceType,
                                                 [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent, assemblyLocator)
        {
            ServiceType = implementedServiceType;
            Implementations = new IServiceImplementationElement[] {this};
        }

        #endregion

        #region IServiceElement Interface Implementation

        public IEnumerable<IServiceImplementationElement> Implementations { get; }

        public override void Initialize()
        {
            base.Initialize();

            if (Enabled)
                if (!ServiceType.IsAssignableFrom(ImplementationType))
                    throw new ConfigurationParseException(this, $"Class '{ImplementationType.FullName}' does not implement interface '{ServiceType.FullName}'.");
        }

        public bool RegisterIfNotRegistered => false;

        [NotNull]
        public Type ServiceType { get; }

        #endregion

        #region Member Functions

        public override DiResolutionScope ResolutionScope => DiResolutionScope.Singleton;

        #endregion
    }
}