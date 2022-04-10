// Copyright (c) IoC.Configuration Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.

using System.Xml;
using IoC.Configuration.DiContainerBuilder.FileBased;
using JetBrains.Annotations;

namespace IoC.Configuration.AttributeValueTransformer
{
    /// <summary>
    /// Replaces the value of an attribute in configuration file to a different value.
    /// This interface can be handy to set some attribute values at runtime. An instance of this class should be added
    /// to <see cref="FileBasedConfigurationParameters.AttributeValueTransformers"/>.
    /// See the usage of implementation IoC.Configuration.Tests.RuntimeAttributeValueProviderTests.AppDataDirRuntimeAttributeValueProvider in tests project 'IoC.Configuration.Tests'.
    ///  </summary>
    public interface IAttributeValueTransformer
    {
        /// <summary>
        /// Returns true, if attribute provider provides a new value that should replace the value of attribute <paramref name="xmlAttribute"/> in XML configuration file
        /// element with path <paramref name="elementPath"/>.
        /// Returns false if the attribute value is not transformed.
        /// </summary>
        /// <param name="elementPath">Element path being checked. Example: "/iocConfiguration/appDataDir" or
        /// "/iocConfiguration/dependencyInjection/services/service/valueImplementation/constructedValue/parameters/string"
        /// </param>
        /// <param name="xmlAttribute">Attribute being checked. Use <see cref="XmlAttribute.OwnerElement"/> to get more context data on attribute element if necessary.
        /// Also, use the properties <see cref="XmlAttribute.Name"/> and <see cref="XmlAttribute.Value"/> if necessary.
        /// </param>
        /// <param name="newAttributeValue">Provided new attribute value that should replace the current value of the attribute <paramref name="xmlAttribute"/>.<see cref="XmlAttribute.Value"/> in XML configuration file.
        /// The value is not null if the returned value is true. Otherwise, the value is null.
        /// </param>
        bool TryGetAttributeValue([NotNull] string elementPath, [NotNull] XmlAttribute xmlAttribute, out string newAttributeValue);
    }
}
