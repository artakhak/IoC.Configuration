using System;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IConfigurationFileElementFactory
    {
        #region Current Type Interface

        [NotNull]
        IConfiguration CreateConfiguration(XmlElement xmlElement);

        /// <summary>
        /// </summary>
        /// <param name="xmlElement"></param>
        /// <param name="parentConfigurationFileElement"></param>
        /// <exception cref="Exception">Throws an exception if can't create an element.</exception>
        /// <returns></returns>
        /// <exception cref="ConfigurationParseException"></exception>
        [NotNull]
        IConfigurationFileElement CreateConfigurationFileElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parentConfigurationFileElement);

        #endregion
    }
}