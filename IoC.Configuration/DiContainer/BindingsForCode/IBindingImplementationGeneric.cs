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
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForCode
{
    public interface IBindingImplementationGeneric<TService, TImplementation> : IBindingImplementation
    {
        /// <summary>
        ///     Use this member to add multiple implementations for the same service.
        /// </summary>
        [NotNull]
        IBindingGeneric<TService> Service { get; }

        /// <summary>
        /// React to an event that occurs when the implementation is instantiated.
        /// </summary>
        /// <param name="onImplementationActivated"><see cref="Action"/> that will be executed on implementation activated event.</param>
        /// <returns>Returns an instance of <see cref="IBindingImplementationGeneric{TService, TImplementation}"/></returns>
        [NotNull]
        IBindingImplementationGeneric<TService, TImplementation> OnImplementationObjectActivated([NotNull] Action<IDiContainer, TImplementation> onImplementationActivated);

        /// <summary>
        /// Sets the resolution scope.
        /// </summary>
        /// <param name="resolutionScope">The resolution scope.</param>
        /// <returns>Returns an instance of <see cref="IBindingImplementationGeneric{TService, TImplementation}"/></returns>
        [NotNull]
        IBindingImplementationGeneric<TService, TImplementation> SetResolutionScope(DiResolutionScope resolutionScope);

#if DEBUG
// Will enable this code in release mode when Autofac implementation for this feature is available.
        //[NotNull]
        //IBindingImplementationGeneric<TService, TImplementation> WhenInjectedInto(Type targetType, bool considerAlsoTargetTypeSubclasses);

        //[NotNull]
        //IBindingImplementationGeneric<TService, TImplementation> WhenInjectedInto<T>(bool considerAlsoTargetTypeSubclasses);
#endif
    }
}