using System;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    /// <summary>
    ///     Event arguments for the configuration XmlDocument.
    /// </summary>
    public class ConfigurationFileXmlDocumentLoadedEventArgs : EventArgs
    {
        #region  Constructors

        /// <summary>
        ///     A constructor.
        /// </summary>
        /// <param name="xmlDocument"></param>
        public ConfigurationFileXmlDocumentLoadedEventArgs([NotNull] XmlDocument xmlDocument)
        {
            XmlDocument = xmlDocument;
        }

        #endregion

        #region Member Functions

        /// <summary>
        ///     Event arguments for the configuration XmlDocument.
        ///     Note, the XmlDocument is not yet validated agains schema. Therefore, if the XmlDocument is modified,
        ///     it should be modified in such a way, that it still confirms to schema.
        /// </summary>
        [NotNull]
        public XmlDocument XmlDocument { get; }

        #endregion
    }
}