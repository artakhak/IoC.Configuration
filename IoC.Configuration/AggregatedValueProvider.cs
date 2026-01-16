using System.Collections.Generic;

namespace IoC.Configuration
{
    /// <summary>
    /// Default implementation of <see cref="IValueProvider"/> that always returns false.
    /// </summary>
    public class AggregatedValueProvider : IValueProvider
    {
        private readonly IReadOnlyList<IValueProvider> _valueProviders;
        
        public AggregatedValueProvider(IReadOnlyList<IValueProvider> valueProviders)
        {
            _valueProviders = valueProviders;
        }
        
        /// <inheritdoc />
        public bool TryResolveValue(IProvidedValueData providedValueData, out object resolvedValue)
        {
            foreach (var valueProvider in _valueProviders)
            {
                if (valueProvider.TryResolveValue(providedValueData, out resolvedValue))
                    return true;
            }
            
            resolvedValue = null;
            return false;
        }
    }
}