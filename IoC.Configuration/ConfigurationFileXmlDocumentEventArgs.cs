// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

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
        ///     Note, the XmlDocument is not yet validated against schema. Therefore, if the XmlDocument is modified,
        ///     it should be modified in such a way, that it still confirms to schema.
        /// </summary>
        [NotNull]
        public XmlDocument XmlDocument { get; }

        #endregion
    }
}