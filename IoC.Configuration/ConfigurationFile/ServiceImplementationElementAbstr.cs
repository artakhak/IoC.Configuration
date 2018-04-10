using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class ServiceImplementationElementAbstr : ConfigurationFileElementAbstr, IServiceImplementationElement
    {
        #region Member Variables

        [NotNull]
        protected readonly IAssemblyLocator _assemblyLocator;

        #endregion

        #region  Constructors

        public ServiceImplementationElementAbstr([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                                 [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region IServiceImplementationElement Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IParameters)
                Parameters = (IParameters) child;
            else if (child is IInjectedProperties)
                InjectedProperties = (IInjectedProperties) child;

#if DEBUG
// Will enable this code and finish implementation in release mode when Autofac implementation for this feature is available.
            WhenInjectedIntoType = null;
            ConditionalInjectionType = ConditionalInjectionType.None; 
#endif
        }

        public IAssembly Assembly { get; private set; }

        public ConditionalInjectionType ConditionalInjectionType { get; private set; }

        public override bool Enabled => base.Enabled && Assembly.Enabled;

        public Type ImplementationType { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            Assembly = Helpers.GetAssemblySettingByAssemblyAlias(this, this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Assembly));

            var implementationTypeName = this.GetAttributeValue<string>(ImplementationTypeAttributeName);

            if (Enabled)
            {
                ImplementationType = Helpers.GetTypeInAssembly(_assemblyLocator, this, Assembly, implementationTypeName);

                if (ImplementationType.IsAbstract || ImplementationType.IsInterface)
                    throw new ConfigurationParseException(this, $"Type '{ImplementationType.FullName}' should be a concrete class. In other words it cannot be an interface or an abstract class.");

                // If no constructor parameter was specified, we will be injecting by type.
                if (ImplementationType.GetConstructors().FirstOrDefault(x => x.IsPublic) == null)
                    throw new ConfigurationParseException(this, $"Type '{ImplementationType.FullName}' has no public constructors.");

                if (OwningPluginElement != null && Assembly.OwningPluginElement != OwningPluginElement)
                    throw new ConfigurationParseException(this, $"Type '{ImplementationType.FullName}' is defined in an assembly '{Assembly}' which does not belong to plugin '{OwningPluginElement.Name}'.");
            }
        }

        [CanBeNull]
        public IInjectedProperties InjectedProperties { get; private set; }

        [CanBeNull]
        public IParameters Parameters { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            if (Enabled)
            {
                // If no constructor parameter was specified, we will be injecting by type.
                if (Parameters != null && Parameters.AllParameters.Any())
                    if (!GlobalsCoreAmbientContext.Context.CheckTypeConstructorExistence(ImplementationType,
                        Parameters.GetParameterValues().Select(x => x.ParameterType).ToArray(),
                        out var constructorInfo, out var errorMessage))
                        throw new ConfigurationParseException(this, errorMessage);

                if (InjectedProperties != null)
                    foreach (var injectedProperty in InjectedProperties.AllProperties)
                    {
                        PropertyInfo propertyInfo = null;
                        try
                        {
                            propertyInfo = ImplementationType.GetProperty(injectedProperty.Name);
                        }
                        catch (Exception)
                        {
                            // We will throw later
                        }

                        if (propertyInfo == null || !propertyInfo.PropertyType.IsAssignableFrom(injectedProperty.ValueType) ||
                            propertyInfo.GetSetMethod(false) == null)
                            throw new ConfigurationParseException(injectedProperty, $"Injected property '{injectedProperty.Name}' is invalid for type '{ImplementationType.FullName}'. The type '{ImplementationType.FullName}' does not have a property named '{injectedProperty.Name}' of type '{injectedProperty.ValueType.FullName}' with public setter.", this);
                    }
            }
        }

        public Type WhenInjectedIntoType { get; private set; }

        #endregion

        #region Current Type Interface

        protected virtual string ImplementationTypeAttributeName => ConfigurationFileAttributeNames.Type;

        public abstract DiResolutionScope ResolutionScope { get; }

        #endregion
    }
}