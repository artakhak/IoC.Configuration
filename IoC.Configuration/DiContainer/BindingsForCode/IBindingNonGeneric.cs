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
    public interface IBindingNonGeneric
    {
        #region Current Type Interface

        /// <summary>
        ///     If called, the binding will be registered only if binding for the service has not been already registered.
        ///     Note, multiple implementations can still be registered using this binding, if the binding for service was not
        ///     registered.
        /// </summary>
        /// <returns>Returns <see cref="IBindingNonGeneric" /></returns>
        [NotNull]
        IBindingNonGeneric OnlyIfNotRegistered();

        /// <summary>
        ///     Bind service to implementation of type <paramref name="implementationType" />.
        /// </summary>
        /// <param name="implementationType">The type of the implementation</param>
        /// <returns>Returns <see cref="BindingImplementationNonGeneric" /></returns>
        [NotNull]
        BindingImplementationNonGeneric To([NotNull] Type implementationType);

        /// <summary>
        ///     Bind the service to the result of function call <paramref name="resolverFunc" />(<see cref="IDiContainer" />).
        /// </summary>
        /// <param name="resolverFunc">The resolver function.</param>
        /// <returns>Returns <see cref="BindingImplementationNonGeneric" /></returns>
        [NotNull]
        BindingImplementationNonGeneric To([NotNull] Func<IDiContainer, object> resolverFunc);

        /// <summary>
        ///     Bind the service  to itself.
        /// </summary>
        /// <returns>Returns <see cref="IBindingImplementationNonGeneric" /></returns>
        [NotNull]
        IBindingImplementationNonGeneric ToSelf();

        #endregion
    }
}