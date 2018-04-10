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