using System.Xml;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class ObjectInstanceElementAbstr<TInstantiatedType> : ConfigurationFileElementAbstr where TInstantiatedType : class
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        private IAssembly _assemblySetting;

        [CanBeNull]
        private IParameters _parameters;

        #endregion

        #region  Constructors

        public ObjectInstanceElementAbstr([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                          [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region Current Type Interface

        protected virtual bool ShouldBeEnabled => true;

        #endregion

        #region Member Functions

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IParameters)
            {
                _parameters = (IParameters) child;

                foreach (var parameter in _parameters.AllParameters)
                    if (parameter.ValueInstantiationType == ValueInstantiationType.ResolveFromDiContext)
                        throw new ConfigurationParseException(parameter, $"Injected parameters cannot be used in element '{ElementName}'", this);
            }
        }

        public override bool Enabled => base.Enabled && _assemblySetting.Enabled;

        public override void Initialize()
        {
            base.Initialize();
            _assemblySetting = Helpers.GetAssemblySettingByAssemblyAlias(this, this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Assembly));

            if (!Enabled && ShouldBeEnabled)
                throw new ConfigurationParseException(this, $"The element '{ElementName}' is disabled. Either make sure the assembly '{_assemblySetting}' is enabled, or delete this element.");
        }

        protected TInstantiatedType Instance { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            var typeFullName = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Type);

            if (Enabled)
            {
                var typeInAssembly = Helpers.GetTypeInAssembly(_assemblyLocator, this, _assemblySetting, typeFullName);

                if (!GlobalsCoreAmbientContext.Context.TryCreateInstanceFromType(typeof(TInstantiatedType), typeInAssembly, _parameters == null ? new ParameterInfo[0] : _parameters.GetParameterValues(), out var serializerObject, out var errorMessage))
                    throw new ConfigurationParseException(this, errorMessage);

                Instance = (TInstantiatedType) serializerObject;
            }
        }

        #endregion
    }
}