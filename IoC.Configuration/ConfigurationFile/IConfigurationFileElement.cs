using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IConfigurationFileElement
    {
        #region Current Type Interface

        /// <summary>
        /// </summary>
        /// <param name="child"></param>
        /// <exception cref="ConfigurationParseException"></exception>
        void AddChild([NotNull] IConfigurationFileElement child);

        [NotNull]
        [ItemNotNull]
        IReadOnlyList<IConfigurationFileElement> Children { get; }

        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        string ElementName { get; }

        bool Enabled { get; }

        string GenerateElementError([NotNull] string message, IConfigurationFileElement parentElement = null);

        [CanBeNull]
        string GetAttributeValue([NotNull] string attributeName);

        /// <summary>
        /// </summary>
        /// <exception cref="ConfigurationParseException"></exception>
        void Initialize();

        /// <summary>
        ///     If not null, specifies the plugin to which the element is applicable. Otherwise, the element does not belong to any
        ///     plugin.
        /// </summary>
        [CanBeNull]
        IPluginElement OwningPluginElement { get; }

        [CanBeNull]
        IConfigurationFileElement Parent { get; }

        /// <summary>
        /// </summary>
        /// <exception cref="ConfigurationParseException"></exception>
        void ValidateAfterChildrenAdded();

        void ValidateOnTreeConstructed();

        [NotNull]
        string XmlElementToString();

        #endregion
    }
}