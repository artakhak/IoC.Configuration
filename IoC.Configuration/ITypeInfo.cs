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

namespace IoC.Configuration
{
    public interface ITypeInfo
    {
        #region Current Type Interface

        /// <summary>
        ///     If the type represented by this class is an array, <see cref="ArrayItemTypeInfo" /> is the type info of array
        ///     items.
        ///     Otherwise, <see cref="ArrayItemTypeInfo" /> is null.
        /// </summary>
        [CanBeNull]
        ITypeInfo ArrayItemTypeInfo { get; }

        [NotNull]
        IAssembly Assembly { get; }

        [NotNull]
        [ItemNotNull]
        IReadOnlyList<ITypeInfo> GenericTypeParameters { get; }

        [NotNull]
        [ItemNotNull]
        IReadOnlyList<ITypeInfo> GetUniquePluginTypes();

        void ProcessTypeAndGenericParameters([NotNull] ProcessTypeInfo typeInfoProcessor, ref bool stopProcessing);

        [NotNull]
        Type Type { get; }

        [NotNull]
        string TypeCSharpFullName { get; }

        [NotNull]
        string TypeInternalFullNameWithAssembly { get; }

        #endregion
    }
}