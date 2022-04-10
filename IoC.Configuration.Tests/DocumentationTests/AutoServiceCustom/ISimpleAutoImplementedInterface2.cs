// Copyright (c) IoC.Configuration Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.

namespace IoC.Configuration.Tests.DocumentationTests.AutoServiceCustom;

public interface ISimpleAutoImplementedInterface2
{
    [SimpleMethodMetadata(20)]
    int GetSomeOtherValue();
}