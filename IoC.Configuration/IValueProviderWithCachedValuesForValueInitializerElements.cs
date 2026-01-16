using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    /// <summary>
    /// This interface represents a value provider that caches resolved values for value initializer elements identified by their unique GUIDs.
    /// </summary>
    public interface IValueProviderWithCachedValuesForValueInitializerElements
    {
        bool TryResolveValue(Guid valueInitializerElementId, IProvidedValueData providedValueData, out object resolvedValue);
        T ResolveValue<T>(Guid valueInitializerElementId);
    }

    /// <inheritdoc />
    public class ValueProviderWithCachedValuesForValueInitializerElements : IValueProviderWithCachedValuesForValueInitializerElements
    {
        private readonly IValueProvider _valueProvider;
        
        private readonly Dictionary<Guid, object> _valueInitializerElementGuidToProvidedValue = new Dictionary<Guid, object>();
        
        [NotNull]
        private readonly object _lockObject = new object();

        public ValueProviderWithCachedValuesForValueInitializerElements(IValueProvider valueProvider)
        {
            _valueProvider = valueProvider;
        }
        
        /// <inheritdoc />
        public bool TryResolveValue(Guid valueInitializerElementId, IProvidedValueData providedValueData, out object resolvedValue)
        {
            lock (_lockObject)
            {
                if (_valueInitializerElementGuidToProvidedValue.TryGetValue(valueInitializerElementId, out resolvedValue))
                    return true;

                if (_valueProvider.TryResolveValue(providedValueData, out resolvedValue))
                {
                    _valueInitializerElementGuidToProvidedValue[valueInitializerElementId] = resolvedValue;
                    return true;
                }
                
                resolvedValue = null;
                return false;
            }
        }

        /// <inheritdoc />
        public T ResolveValue<T>(Guid valueInitializerElementId)
        {
            lock (_lockObject)
            {
                if (_valueInitializerElementGuidToProvidedValue.TryGetValue(valueInitializerElementId, out var value))
                {
                    if (value is T resolvedValue)
                    {
                        return resolvedValue;
                    }
                }
            }
            
            throw new InvalidCastException($"Failed to resolve value in value initialization element with element identifier Id: {valueInitializerElementId}!");
        }
    }
}