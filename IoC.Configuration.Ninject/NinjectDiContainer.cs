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
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using Ninject;

namespace IoC.Configuration.Ninject
{
    public class NinjectDiContainer : IDiContainer
    {
        #region Member Variables

        [NotNull]
        private readonly object _lockObject = new object();

        #endregion

        #region  Constructors

        public NinjectDiContainer() : this(new StandardKernel())
        {
        }

        public NinjectDiContainer(IKernel kernel)
        {
            Kernel = kernel;
            MainLifeTimeScope = new NinjectLifeTimeScope();
            CurrentLifeTimeScope = MainLifeTimeScope;

            MainLifeTimeScope.LifeTimeScopeTerminated += (sender, e) => { Kernel.Dispose(); };
        }

        #endregion

        #region IDiContainer Interface Implementation

        [NotNull]
        public ILifeTimeScope CurrentLifeTimeScope { get; private set; }

        public void Dispose()
        {
            MainLifeTimeScope.Dispose();
        }

        [NotNull]
        public ILifeTimeScope MainLifeTimeScope { get; }

        public T Resolve<T>() where T : class
        {
            return (T) Resolve(typeof(T), MainLifeTimeScope);
        }

        public object Resolve(Type type)
        {
            return Resolve(type, MainLifeTimeScope);
        }

        public T Resolve<T>(ILifeTimeScope lifeTimeScope) where T : class
        {
            return (T) Resolve(typeof(T), lifeTimeScope);
        }

        public object Resolve(Type type, ILifeTimeScope lifeTimeScope)
        {
            lock (_lockObject)
            {
                var previousLifeTimeScope = CurrentLifeTimeScope;
                try
                {
                    CurrentLifeTimeScope = lifeTimeScope;
                    return Kernel.Get(type);
                }
                finally
                {
                    CurrentLifeTimeScope = previousLifeTimeScope;
                }
            }
        }

        public ILifeTimeScope StartLifeTimeScope()
        {
            return new NinjectLifeTimeScope();
        }

        public void StartMainLifeTimeScope()
        {
            // Nothing to do
        }

        #endregion

        #region Member Functions

        [NotNull]
        public IKernel Kernel { get; }

        #endregion
    }
}