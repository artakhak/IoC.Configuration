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
using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IConfigurationFileElement
    {
        #region Current Type Interface

        void AddChild([NotNull] IConfigurationFileElement child);

        void BeforeChildInitialize([NotNull] IConfigurationFileElement child);

        [NotNull]
        [ItemNotNull]
        IReadOnlyList<IConfigurationFileElement> Children { get; }

        [NotNull]
        IConfiguration Configuration { get; }

        [NotNull]
        string ElementName { get; }

        bool Enabled { get; }

        [Obsolete("Will be removed after 5/31/2019. Use ErrorHelperAmbientContext.Context.GenerateElementError")]
        string GenerateElementError([NotNull] string message, IConfigurationFileElement parentElement = null);

        [CanBeNull]
        string GetAttributeValue([NotNull] string attributeName);

        [Obsolete("Will be removed after 5/31/2019. Use GetPluginSetupElement")]
        [CanBeNull]
        IPluginSetup GetParentPluginSetupElement();

        [CanBeNull]
        IPluginSetup GetPluginSetupElement();

        bool HasAttribute([NotNull] string attributeName);

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