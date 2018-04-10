using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IParameterElement : INamedValueElement
    {
        #region Current Type Interface

        /// <summary>
        ///     The value deserialized from ValueAsString to type in property <see cref="INamedValue.ValueType" />
        /// </summary>
        [CanBeNull]
        object DeserializedValue { get; }

        #endregion
    }
}