using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer
{
    // TODO: either finish IoC.Configuration.DiContainer.TypeScannerModule and make this interface public, or get rid off this interface.

    /// <inheritdoc />
    public class ScannedTypeRegistrationInfo: IScannedTypeRegistrationInfo
    {
        // ReSharper disable once NotNullMemberIsNotInitialized
        public ScannedTypeRegistrationInfo([NotNull] Type serviceType)
        {
            this.ServiceType = serviceType;
        }

        public ScannedTypeRegistrationInfo(Type serviceType, IEnumerable<ITypePicker> additionalFilters) : this(serviceType)
        {
            this.AdditionalImplementationTypeFilters = additionalFilters;
        }

        /// <summary>
        /// Interface type.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// If the list is null, all non-abstract implementations of type <see cref="IScannedTypeRegistrationInfo.ServiceType"/> with public constructors
        /// will be picked.
        /// Otherwise, filters in this list will be applied to implementation type.
        /// </summary>
        public IEnumerable<ITypePicker> AdditionalImplementationTypeFilters { get; }
    }
}