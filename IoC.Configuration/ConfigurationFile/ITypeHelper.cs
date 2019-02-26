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
    public interface ITypeHelper
    {
        #region Current Type Interface

        /// <summary>
        ///     Gets an instance of <see cref="ITypeInfo" /> by looking for the type in assemblies specified in section assemblies
        ///     if
        ///     <paramref name="assemblyAttributeName" /> is null, or if the value of this attribute in
        ///     <paramref name="requestingConfigurationFileElement" />
        ///     is missing. Otherwise, the type is loaded from the assembly in attribute <paramref name="assemblyAttributeName" />
        ///     in
        ///     element <paramref name="requestingConfigurationFileElement" />.
        /// </summary>
        /// <param name="requestingConfigurationFileElement">An element, where the type is specified.</param>
        /// <param name="typeAttributeName">Attribute name that has the value of type full name of the type.</param>
        /// <param name="assemblyAttributeName">
        ///     Attribute name that has the value of the assembly. If the value is null, or
        ///     if <paramref name="requestingConfigurationFileElement" /> does not have an attribute specified in
        ///     <paramref name="assemblyAttributeName" />,
        ///     the type will all assemblies specified in 'iocConfiguration/assemblies' element will be
        /// </param>
        /// <param name="typeRefAttributeName">Attribute name for a referenced type.</param>
        /// <param name="genericTypeParameters">
        ///     If the type specified in attribute <paramref name="typeAttributeName" /> is a generic type,
        ///     this parameter specifies a list of type parameters. Otherwise, the value should be null or empty.
        /// </param>
        /// <exception cref="ConfigurationParseException">Throws this exception, if the type is not found.</exception>
        [NotNull]
        ITypeInfo GetTypeInfo([NotNull] IConfigurationFileElement requestingConfigurationFileElement,
                              [NotNull] string typeAttributeName, [CanBeNull] string assemblyAttributeName,
                              [NotNull] string typeRefAttributeName,
                              [NotNull] [ItemNotNull] IEnumerable<ITypeInfo> genericTypeParameters);

        /// <summary>
        ///     Gets an instance of <see cref="ITypeInfo" /> by looking for the type in assemblies specified in section assemblies
        ///     if
        ///     <paramref name="assemblyAttributeName" /> is null, or if the value of this attribute in
        ///     <paramref name="requestingConfigurationFileElement" />
        ///     is missing. Otherwise, the type is loaded from the assembly in attribute <paramref name="assemblyAttributeName" />
        ///     in
        ///     element <paramref name="requestingConfigurationFileElement" />.
        /// </summary>
        /// <param name="requestingConfigurationFileElement">An element, where the type is specified.</param>
        /// <param name="typeAttributeName">Attribute name that has the value of type full name of the type.</param>
        /// <param name="assemblyAttributeName">
        ///     Attribute name that has the value of the assembly. If the value is null, or
        ///     if <paramref name="requestingConfigurationFileElement" /> does not have an attribute specified in
        ///     <paramref name="assemblyAttributeName" />,
        ///     the type will all assemblies specified in 'iocConfiguration/assemblies' element will be
        /// </param>
        /// <param name="typeRefAttributeName">Attribute name for a referenced type.</param>
        /// <exception cref="ConfigurationParseException">Throws this exception, if the type is not found.</exception>
        [NotNull]
        ITypeInfo GetTypeInfo([NotNull] IConfigurationFileElement requestingConfigurationFileElement,
                              [NotNull] string typeAttributeName, [CanBeNull] string assemblyAttributeName,
                              [NotNull] string typeRefAttributeName);

        /// <summary>
        ///     Gets an instance of <see cref="ITypeInfo" /> by looking for the type in assemblies specified in section assemblies.
        /// </summary>
        /// <param name="requestingConfigurationFileElement">An element, where the type is specified.</param>
        /// <param name="type">Type</param>
        /// <exception cref="ConfigurationParseException">Throws this exception, if the type is not found.</exception>
        [NotNull]
        ITypeInfo GetTypeInfoFromType([NotNull] IConfigurationFileElement requestingConfigurationFileElement, [NotNull] Type type);

        /// <summary>
        ///     Gets an instance of <see cref="ITypeInfo" /> by looking for the type in assemblies specified in section assemblies.
        /// </summary>
        /// <param name="requestingConfigurationFileElement">An element, where the type is specified.</param>
        /// <param name="typeFullName">Type full name.</param>
        /// <exception cref="ConfigurationParseException">Throws this exception, if the type is not found.</exception>
        [NotNull]
        ITypeInfo GetTypeInfoFromTypeFullName([NotNull] IConfigurationFileElement requestingConfigurationFileElement,
                                              [NotNull] string typeFullName);

        #endregion
    }
}