using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.ConfigurationFile
{
    public class ServiceElement : ConfigurationFileElementAbstr, IServiceElement
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        private IAssembly _assemblySetting;

        private readonly IList<IServiceImplementationElement> _serviceImplementations = new List<IServiceImplementationElement>();

        #endregion

        #region  Constructors

        public ServiceElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                              [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region IServiceElement Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IServiceImplementationElement)
            {
                var serviceImplementation = (IServiceImplementationElement) child;

                if (child.Enabled)
                {
                    if (!ServiceType.IsAssignableFrom(serviceImplementation.ImplementationType))
                        throw new ConfigurationParseException(serviceImplementation, $"Implementation '{serviceImplementation.ImplementationType.FullName}' is not valid for service '{ServiceType.FullName}'.", this);

                    // Note, the same implementation type might appear multiple times.
                    // We do not want to prevent this, since each implementation might have different constructor parameters or injected properties.
                    _serviceImplementations.Add(serviceImplementation);
                }
            }
        }

        public override bool Enabled => base.Enabled && _assemblySetting.Enabled;
        public IEnumerable<IServiceImplementationElement> Implementations => _serviceImplementations;

        public override void Initialize()
        {
            base.Initialize();

            _assemblySetting = Helpers.GetAssemblySettingByAssemblyAlias(this, this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Assembly));

            var serviceTypeName = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Type);

            if (Enabled)
            {
                ServiceType = Helpers.GetTypeInAssembly(_assemblyLocator, this, _assemblySetting, serviceTypeName);

                if (OwningPluginElement == null)
                {
                    if (_assemblySetting.OwningPluginElement != null)
                        throw new ConfigurationParseException(this, $"Type '{ServiceType.FullName}' is defined in assembly {_assemblySetting} which belongs to plugin '{_assemblySetting.OwningPluginElement.Name}'. The service should be defined under '{ConfigurationFileElementNames.Services}' element for plugin '{_assemblySetting.OwningPluginElement.Name}'.");
                }
                else if (_assemblySetting.OwningPluginElement != OwningPluginElement)
                {
                    throw new ConfigurationParseException(this, $"Type '{ServiceType.FullName}' is defined in assembly {_assemblySetting} which does not be belong to plugin '{OwningPluginElement.Name}' that owns the service.");
                }

                RegisterIfNotRegistered = this.GetAttributeValue<bool>(ConfigurationFileAttributeNames.RegisterIfNotRegistered);
            }
        }

        public bool RegisterIfNotRegistered { get; private set; }

        public Type ServiceType { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            // We might have a situation, when the only implementations are types in plugins, and all
            // plugins are disabled. In this case _serviceImplementations will be an empty enumeration, and that would be OK.
            if (_serviceImplementations.Count == 0)
                LogHelper.Context.Log.WarnFormat(MessagesHelper.GetNoImplementationsForServiceMessage(ServiceType));

            if (RegisterIfNotRegistered && _serviceImplementations.Count > 1)
                throw new ConfigurationParseException(this, MessagesHelper.GetMultipleImplementationsWithRegisterIfNotRegisteredOptionMessage($"attribute '{ConfigurationFileAttributeNames.RegisterIfNotRegistered}'"));
        }

        #endregion
    }
}