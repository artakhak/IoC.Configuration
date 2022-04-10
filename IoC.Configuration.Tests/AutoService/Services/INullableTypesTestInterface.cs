using System;
using System.Collections.Generic;

namespace IoC.Configuration.Tests.AutoService.Services
{
    public interface INullableTypesTestInterface
    {
        int? GetNullableInt();
        IReadOnlyList<Int32?> GetNullablesList();

        int MethodWithNullableParameter(double? value);

        int MethodWithParameterAsListOfNullableValues(IReadOnlyList<double?> value);
    }
}
