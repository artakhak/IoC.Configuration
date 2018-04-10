using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class TypeFactoryReturnedTypesSelector : ConfigurationFileElementAbstr, ITypeFactoryReturnedTypesSelector
    {
        #region Member Variables

        private readonly IList<ITypeFactoryReturnedType> _returnedTypes = new List<ITypeFactoryReturnedType>();

        #endregion

        #region  Constructors

        public TypeFactoryReturnedTypesSelector([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region ITypeFactoryReturnedTypesSelector Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is ITypeFactoryReturnedType)
                _returnedTypes.Add((ITypeFactoryReturnedType) child);
        }

        public IEnumerable<ITypeFactoryReturnedType> ReturnedTypes => _returnedTypes;

        #endregion
    }
}