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

namespace IoC.Configuration.DiContainer
{
    /// <summary>
    ///     A DI container abstraction used to resolve services. An example implementations of this interface can be found in
    ///     IoC.Configuration.Autofac and IoC.Configuration.Ninject Nuget packages.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IDiContainer : IDisposable
    {
        #region Current Type Interface

        /// <summary>
        ///     Gets the current life time scope.
        /// </summary>
        /// <value>
        ///     The current life time scope.
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

        /// <summary>
        ///     Resolved the type using the life time scope <see cref="CurrentLifeTimeScope" />.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class;

        /// <summary>
        ///     Resolved the type using the life time scope <see cref="CurrentLifeTimeScope" />.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        object Resolve(Type type);

        /// <summary>
        ///     Sets the value of <see cref="IDiContainer.CurrentLifeTimeScope" /> to the value of parameter
        ///     <paramref name="lifeTimeScope" />,
        ///     resolved the object using<paramref name="lifeTimeScope" />, and restores the value of
        ///     <see cref="IDiContainer.CurrentLifeTimeScope" /> to its
        ///     previous value, after the type resolution is complete.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lifeTimeScope"></param>
        /// <returns></returns>
        T Resolve<T>(ILifeTimeScope lifeTimeScope) where T : class;

        /// <summary>
        ///     Sets the value of <see cref="CurrentLifeTimeScope" /> to the value of parameter <paramref name="lifeTimeScope" />,
        ///     resolved the object using <paramref name="lifeTimeScope" />, and restores the value of
        ///     <see cref="CurrentLifeTimeScope" /> to its
        ///     previous value, after the type resolution is complete.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="lifeTimeScope"></param>
        /// <returns></returns>
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