// This software is part of the IoC.Configuration library
// Copyright � 2018 IoC.Configuration Contributors
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

using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface ITypeDefinitionsElement : IConfigurationFileElement
    {
        #region Current Type Interface
        /// <summary>
        /// Returns a list of type definitions under element <see cref="ConfigurationFileElementNames.TypeDefinitions"/>.
        /// If the element belongs to plugin section, will not include the type defintions in non-plugin section. 
        /// </summary>
        [NotNull]
        [ItemNotNull]
        IReadOnlyList<INamedTypeDefinitionElement> AllTypeDefinitions { get; }

        /// <summary>
        /// If the the definitions element is in plugin, will try to get the type definition defined
        /// in plugin in <see cref="ConfigurationFileElementNames.TypeDefinitions"/> element.
        /// Otherwise, if the element <see cref="ConfigurationFileElementNames.TypeDefinitions"/> is a non-plugin element,
        /// or if the type definition was not found, will lookup the type definition
        /// in element <see cref="ConfigurationFileElementNames.TypeDefinitions"/> element in non-plugin section.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        [CanBeNull]
        INamedTypeDefinitionElement GetTypeDefinition(string alias);

        #endregion
    }
}