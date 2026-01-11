using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    /// <summary>
    /// This 
    /// </summary>
    public interface IValueProviderWithCachedValuesForValueInitializerElements //: IValueProvider
    {
        bool TryResolveValue(Guid valueInitializerElementGuid, IProvidedValueData providedValueData, out object resolvedValue);
        T ResolveValue<T>(Guid valueInitializerElementGuid);
    }

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
        public bool TryResolveValue(Guid valueInitializerElementGuid, IProvidedValueData providedValueData, out object resolvedValue)
        {
            lock (_lockObject)
            {
                if (_valueInitializerElementGuidToProvidedValue.TryGetValue(valueInitializerElementGuid, out resolvedValue))
                    return true;

                if (_valueProvider.TryResolveValue(providedValueData, out resolvedValue))
                {
                    _valueInitializerElementGuidToProvidedValue[valueInitializerElementGuid] = resolvedValue;
                    return true;
                }
                
                resolvedValue = null;
                return false;
            }
        }

        /// <inheritdoc />
        public T ResolveValue<T>(Guid valueInitializerElementGuid)
        {
            lock (_lockObject)
            {
                if (_valueInitializerElementGuidToProvidedValue.TryGetValue(valueInitializerElementGuid, out var value))
                {
                    if (value is T resolvedValue)
                    {
                        return resolvedValue;
                    }
                }
            }
            
            throw new InvalidCastException($"Failed to resolve value in value initialization element with type Id: {valueInitializerElementGuid}!");
        }
    }
}