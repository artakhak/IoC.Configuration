// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace IoC.Configuration.DiContainer
{
    // Non standard features will not be used to avoid situations when the configuration file works for one container, but not the other.
    // If non-standard features are needed, use native module (e.g., Autofac or Ninject module).
    [Obsolete]
    public enum NonStandardFeatures
    {
        /// <summary>
        ///     Allows registering a service, only if the service is not registered, when there is only one implementation for the
        ///     service.
        /// </summary>
        RegisterServiceWithSingleImplementationsOnlyIfServiceNotRegistered,

        /// <summary>
        ///     Allows registering a service, only if the service is not registered, when multiple implementations are registered
        ///     for the service.
        ///     The specific IoC container (e.g., Autofac, Ninject, etc),
        ///     should support registering multiple implementations of the service, if the service was not registered.
        ///     For example Autofac currently supports this for single implementation (using IfNotRegistered(Type)), but there is
        ///     no way to do this for
        ///     multiple implementations of a service (i.e., check if service was registered, and register all implementations if
        ///     it was not).
        /// </summary>
        RegisterServiceWithMultipleImplementationsOnlyIfServiceNotRegistered,

        /// <summary>
        ///     Specific implementation will be applied only if the implementation is injected into a specific type or its
        ///     subclasses.
        /// </summary>
        WhenInjectedIntoClassOrSubClassesConditionalInjection,

        /// <summary>
        ///     Specific implementation will be applied only if the implementation is injected into a specific type.
        /// </summary>
        WhenInjectedExactlyIntoClassConditionalInjection
    }
}