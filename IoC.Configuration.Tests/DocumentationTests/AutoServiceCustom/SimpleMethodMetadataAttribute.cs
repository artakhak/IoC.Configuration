// Copyright (c) IoC.Configuration Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.

using System;

namespace IoC.Configuration.Tests.DocumentationTests.AutoServiceCustom;

[AttributeUsage(AttributeTargets.Method)]
public class SimpleMethodMetadataAttribute : System.Attribute
{
    public SimpleMethodMetadataAttribute(int returnedValue)
    {
        ReturnedValue = returnedValue;
    }
    
    public int ReturnedValue { get; }
}