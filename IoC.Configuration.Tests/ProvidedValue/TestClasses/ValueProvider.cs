using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.Tests.ProvidedValue.TestClasses;


/// <inheritdoc />
public class ValueProvider : IValueProvider
{
    private readonly ILog _logger;

    public ValueProvider(ILog logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public bool TryResolveValue(IProvidedValueData providedValueData, out object resolvedValue)
    {
        resolvedValue = null;
        
        if (providedValueData.Type == typeof(ILog))
        {
            resolvedValue = _logger;
            return true;
        }

        if (providedValueData.Name != null)
        {
            switch (providedValueData.ProvidedValueTargetType)
            {
                case ProvidedValueTargetType.ConstructorParameter:
                    switch (providedValueData.Name)
                    {
                        case "param1":
                            if (providedValueData.Type == typeof (long))
                                resolvedValue = (long)17;
                            else if (providedValueData.Type == typeof(int))
                                resolvedValue = (int) 101;

                            return resolvedValue != null;
                        
                        case "diModule3_param1":
                            resolvedValue = 37;
                            return true;
                        
                        case "color":
                            resolvedValue = 150;
                            return true;
                    }

                    break;

                case ProvidedValueTargetType.Property:
                    switch (providedValueData.Name)
                    {
                        case "Property2":
                            resolvedValue = (long)27;
                            return true;
                        
                        case "Height":
                            resolvedValue = 90.1;
                            return true;
                    }

                    break;

                case ProvidedValueTargetType.Setting:
                    switch (providedValueData.Name)
                    {
                        case "Int32Setting1":
                            resolvedValue = 57;
                            return true;
                        
                        case "StringSetting1":
                            resolvedValue = "String Setting1 Value";
                            return true;
                    }
                    
                    break;
            }
        }
        
        return false;
    }
}