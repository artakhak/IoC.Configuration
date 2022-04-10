using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer
{
    // TODO: either finish IoC.Configuration.DiContainer.TypeScannerModule and make this interface public, or get rid off this interface.
    internal interface IScannedTypeRegistrationInfo
    {
        /// <summary>
        /// Interface type.
        /// </summary>
        [NotNull]
        Type ServiceType { get; }

        /// <summary>
        /// If the list is null, all non-abstract implementations of type <see cref="ServiceType"/> with public constructors
        /// will be picked.
        /// Otherwise, filters in this list will be applied to implementation type.
        /// </summary>
        [NotNull, ItemNotNull]
        IEnumerable<ITypePicker> AdditionalImplementationTypeFilters { get; }
    }
}