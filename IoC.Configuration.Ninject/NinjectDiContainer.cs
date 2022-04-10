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

using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using Ninject;
using System;

namespace IoC.Configuration.Ninject
{
    public class NinjectDiContainer : IDiContainer
    {
        [NotNull]
        private readonly object _lockObject = new object();

        public NinjectDiContainer() : this(new IoCConfigurationNinjectKernel())
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="kernel">
        /// Kernel to use.
        /// Use an instance of <see cref="IoCConfigurationNinjectKernel"/> to support binding some collection types, such as IEnumerable...(finish)
        /// </param>
        public NinjectDiContainer(IKernel kernel)
        {
            Kernel = kernel;
            MainLifeTimeScope = new NinjectLifeTimeScope();
            CurrentLifeTimeScope = MainLifeTimeScope;

            MainLifeTimeScope.LifeTimeScopeTerminated += (sender, e) => { Kernel.Dispose(); };
        }

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
            return (T)Resolve(typeof(T), MainLifeTimeScope);
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
            // TODO (future improvement): Log resolutions based on configuration in new element
            // <diagnostics><logTypeResolutions> 
            // Also, use an interceptor for IDiContainer (wrapper), which will do additional diagnostics, such us storing the time of the last
            // type that we tried to resolve (and which is still being resolved), and will show a warning, if the object was being resolved for more than couple 
            // of seconds (along with the thread Id, and possibly the stack trace)
            //if (LogHelper.Context.Log.IsDebugEnabled)
            //    LogHelper.Context.Log.Debug($"Resolving type {type.FullName}. Thread Id is {Thread.CurrentThread.ManagedThreadId}.");

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
                    //if (LogHelper.Context.Log.IsDebugEnabled)
                    //    LogHelper.Context.Log.Debug($"Did resolve type {type.FullName}. Thread Id is {Thread.CurrentThread.ManagedThreadId}.");
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

        /* TODO: If the next version of Ninject version is ever available (V5 or higher), then
         IKernel will be replaced with IReadOnlyKernel 
         and the container will be instantiated as follows:       
        
         var kernelConfiguration = new KernelConfiguration(INinjectSettings settings, INinjectModule[] modules);
         IReadOnlyKernel kernel = kernelConfiguration.BuildReadOnlyKernel();
         var service = kernel.Get<T>();
         */
        [NotNull]
        public IKernel Kernel { get; }

    }
}