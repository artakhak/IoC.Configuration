using System;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class TypeFactoryReturnedType : ConfigurationFileElementAbstr, ITypeFactoryReturnedType
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        private IAssembly _assemblySetting;

        #endregion

        #region  Constructors

        public TypeFactoryReturnedType([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                       [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region ITypeFactoryReturnedType Interface Implementation

        public override bool Enabled => base.Enabled && _assemblySetting.Enabled;

        public override void Initialize()
        {
            base.Initialize();

            _assemblySetting = Helpers.GetAssemblySettingByAssemblyAlias(this, this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Assembly));

            var serviceTypeName = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Type);

            if (Enabled)
                ReturnedType = Helpers.GetTypeInAssembly(_assemblyLocator, this, _assemblySetting, serviceTypeName);
        }

        public Type ReturnedType { get; private set; }

        #endregion
    }
}