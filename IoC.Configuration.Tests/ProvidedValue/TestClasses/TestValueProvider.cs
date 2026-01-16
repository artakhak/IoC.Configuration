using System.Diagnostics.CodeAnalysis;

namespace IoC.Configuration.Tests.ProvidedValue.TestClasses;

public delegate bool TryResolveValueDelegate(IProvidedValueData providedValueData, [NotNullWhen(true)] out object resolvedValue);

public class TestValueProvider: IValueProvider
{
    private readonly TryResolveValueDelegate _tryResolveValueDelegate;

    public TestValueProvider(TryResolveValueDelegate tryResolveValueDelegate)
    {
        _tryResolveValueDelegate = tryResolveValueDelegate;
    }
    public bool TryResolveValue(IProvidedValueData providedValueData, out object resolvedValue)
    {
        return _tryResolveValueDelegate.Invoke(providedValueData, out resolvedValue);
    }
}