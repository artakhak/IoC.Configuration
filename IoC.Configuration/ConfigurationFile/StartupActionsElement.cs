using System.Collections.Generic;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class StartupActionsElement : ConfigurationFileElementAbstr, IStartupActionsElement
    {
        #region Member Variables

        [NotNull]
        [ItemNotNull]
        private readonly List<IStartupActionElement> _startupActions = new List<IStartupActionElement>();

        #endregion

        #region  Constructors

        public StartupActionsElement([NotNull] XmlElement xmlElement, IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IStartupActionsElement Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IStartupActionElement)
                _startupActions.Add((IStartupActionElement) child);
        }

        public IEnumerable<IStartupActionElement> StartupActions => _startupActions;

        #endregion
    }
}