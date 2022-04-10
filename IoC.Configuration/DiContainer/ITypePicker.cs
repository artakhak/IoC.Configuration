using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer
{
    /// <summary>
    /// An implementation type picker.
    /// </summary>
    public interface ITypePicker
    {
        /// <summary>
        /// A predicate for implementation type.
        /// </summary>
        bool Predicate([NotNull] Type type);
    }
}