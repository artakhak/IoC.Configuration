using System;
using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class DiManagersElement : ConfigurationFileElementAbstr, IDiManagersElement
    {
        #region Member Variables

        [NotNull]
        private string _activeDiManagerName = string.Empty;

        [NotNull]
        private readonly Dictionary<string, IDiManagerElement> _diManagerNameToDiManagerMap = new Dictionary<string, IDiManagerElement>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region  Constructors

        public DiManagersElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IDiManagersElement Interface Implementation

        public IDiManagerElement ActiveDiManagerElement { get; private set; }

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IDiManagerElement)
            {
                var diManagerElement = (IDiManagerElement) child;

                if (_diManagerNameToDiManagerMap.ContainsKey(diManagerElement.Name))
                    throw new ConfigurationParseException(diManagerElement, $"Multiple occurrences of '{ConfigurationFileElementNames.DiManager}' elements with the same value for attribute '{ConfigurationFileAttributeNames.Name}'. The value of attibute is '{diManagerElement.Name}'.", this);

                _diManagerNameToDiManagerMap[diManagerElement.Name] = diManagerElement;
                if (_activeDiManagerName.Equals(diManagerElement.Name, StringComparison.OrdinalIgnoreCase))
                    ActiveDiManagerElement = diManagerElement;
            }
        }

        public IEnumerable<IDiManagerElement> AllDiManagers => _diManagerNameToDiManagerMap.Values;

        public override void Initialize()
        {
            base.Initialize();
            _activeDiManagerName = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.ActiveDiManagerName);
        }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            if (ActiveDiManagerElement == null)
                throw new ConfigurationParseException(this, $"No dependency injection manager named '{_activeDiManagerName}' was found.");
        }

        #endregion
    }
}