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
using OROptimizer.ServiceResolver;

namespace IoC.Configuration.DiContainer
{
    /// <summary>
    ///     A DI container abstraction used to resolve services. An example implementations of this interface can be found in
    ///     IoC.Configuration.Autofac and IoC.Configuration.Ninject Nuget packages.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IDiContainer : IServiceResolver, IDisposable
    {
        #region Current Type Interface

        /// <summary>
        ///     Gets the current life time scope.
        /// </summary>
        /// <value>
        ///     The current life time scope.
        ///     Note, the methods <see cref="IServiceResolver.Resolve(Type)"/> and <see cref="IServiceResolver.Resolve{T}"/> use this
        ///     scope to resolve services.
        /// </value>
        [CanBeNull]
        ILifeTimeScope CurrentLifeTimeScope { get; }

        /// <summary>
        ///     Gets the main life time scope.
        /// </summary>
        /// <value>
        ///     The main life time scope.
        /// </value>
        [CanBeNull]
        ILifeTimeScope MainLifeTimeScope { get; }

        /// 
        /// <summary>
        ///     Resolves an instance of type <typeparamref name="T"/> from DI container.
        ///     Sets the value of <see cref="IDiContainer.CurrentLifeTimeScope" /> to the value of parameter
        ///     <paramref name="lifeTimeScope" />,
        ///     resolved the object using<paramref name="lifeTimeScope" />, and restores the value of
        ///     <see cref="IDiContainer.CurrentLifeTimeScope" /> to its
        ///     previous value, after the type resolution is complete.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="lifeTimeScope">Lifetime scope.</param>
        /// <exception cref="Exception">Throws an exception if type resolution fails.</exception>
        [NotNull]
        T Resolve<T>(ILifeTimeScope lifeTimeScope) where T : class;

        /// <summary>
        ///     Resolves an instance of type <paramref name="type"/> from DI container. 
        ///     Sets the value of <see cref="CurrentLifeTimeScope" /> to the value of parameter <paramref name="lifeTimeScope" />,
        ///     resolved the object using <paramref name="lifeTimeScope" />, and restores the value of
        ///     <see cref="CurrentLifeTimeScope" /> to its
        ///     previous value, after the type resolution is complete.
        /// </summary>
        /// <param name="type">Service type.</param>
        /// <param name="lifeTimeScope">Lifetime scope.</param>
        /// <exception cref="Exception">Throws an exception if type resolution fails.</exception>
        [NotNull]
        object Resolve(Type type, ILifeTimeScope lifeTimeScope);

        /// <summary>
        ///     Starts a new life time scope.
        /// </summary>
        /// <returns></returns>
        [NotNull]
        ILifeTimeScope StartLifeTimeScope();

        /// <summary>
        ///     Starts a new life time scope and assigns it to <see cref="MainLifeTimeScope" />.
        /// </summary>
        void StartMainLifeTimeScope();

        #endregion
    }
}