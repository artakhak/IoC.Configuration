using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class SelfBoundServiceElement : ServiceImplementationElement, ISelfBoundServiceElement
    {
        #region  Constructors

        public SelfBoundServiceElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                       [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent, assemblyLocator)
        {
            Implementations = new IServiceImplementationElement[] {this};
        }

        #endregion

        #region ISelfBoundServiceElement Interface Implementation

        public IEnumerable<IServiceImplementationElement> Implementations { get; }

        public override void Initialize()
        {
            base.Initialize();

            if (Enabled)
            {
                if (OwningPluginElement == null)
                {
                    if (Assembly.OwningPluginElement != null)
                        throw new ConfigurationParseException(this, $"Type '{ServiceType.FullName}' is defined in assembly {Assembly} which belongs to plugin '{Assembly.OwningPluginElement.Name}'. The service should be defined under '{ConfigurationFileElementNames.Services}' element for plugin '{Assembly.OwningPluginElement.Name}'.");
                }
                else if (Assembly.OwningPluginElement != OwningPluginElement)
                {
                    throw new ConfigurationParseException(this, $"Type '{ServiceType.FullName}' is defined in assembly {Assembly} which does not be belong to plugin '{OwningPluginElement.Name}' that owns the service.");
                }

                RegisterIfNotRegistered = this.GetAttributeValue<bool>(ConfigurationFileAttributeNames.RegisterIfNotRegistered);
            }
        }

        public bool RegisterIfNotRegistered { get; private set; }

        public Type ServiceType => ImplementationType;

        #endregion
    }
}