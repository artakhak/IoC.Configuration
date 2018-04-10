using System;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    public interface INamedValue : INamedItem
    {
        #region Current Type Interface

        [CanBeNull]
        string ValueAsString { get; }

        /// <summary>
        ///     Normally for injectedObject element this value is <see cref="ValueInstantiationType.ResolveFromDiContext" />, for
        ///     other elements (i.e., int16, int32, etc), the overridden value will be
        ///     <see cref="ValueInstantiationType.DeserializeFromStringValue" />
        /// </summary>
        ValueInstantiationType ValueInstantiationType { get; }

        /// <summary>
        ///     Can be null only if the parameter is declared with either 'object' or injectedObject elements, and the object type
        ///     referenced
        ///     is in a disabled assembly.
        /// </summary>
        [NotNull]
        Type ValueType { get; }

        #endregion
    }
}