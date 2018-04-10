using System;
using Autofac;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.Autofac
{
    public class AutofacDiContainer : IDiContainer
    {
        #region Member Variables

        private AutofacLifeTimeScope _currentLifeTimeScope;

        [NotNull]
        private readonly object _lockObject = new object();

        private AutofacLifeTimeScope _mainLifeTimeScope;

        #endregion

        #region  Constructors

        public AutofacDiContainer() : this(new ContainerBuilder())
        {
        }

        public AutofacDiContainer([NotNull] ContainerBuilder containerBuilder)
        {
            ContainerBuilder = containerBuilder;
            ContainerBuilder.RegisterBuildCallback(OnContainerBuilt);
        }

        #endregion

        #region IDiContainer Interface Implementation

        public ILifeTimeScope CurrentLifeTimeScope => _currentLifeTimeScope;

        public void Dispose()
        {
            _mainLifeTimeScope.Dispose();
        }

        public ILifeTimeScope MainLifeTimeScope => _mainLifeTimeScope;

        public T Resolve<T>() where T : class
        {
            return (T) Resolve(typeof(T), _mainLifeTimeScope);
        }

        public object Resolve(Type type)
        {
            return Resolve(type, _mainLifeTimeScope);
        }

        public T Resolve<T>(ILifeTimeScope lifeTimeScope) where T : class
        {
            return (T) Resolve(typeof(T), lifeTimeScope);
        }

        public object Resolve(Type type, ILifeTimeScope lifeTimeScope)
        {
            lock (_lockObject)
            {
                var previousLifeTimeScope = _currentLifeTimeScope;
                try
                {
                    _currentLifeTimeScope = lifeTimeScope as AutofacLifeTimeScope;

                    if (_currentLifeTimeScope == null)
                    {
                        // We should be getting only life time scopes that were created by this class.
                        var errorMessage = string.Format("The value of parameter '{0}' is invalid in '{1}.{2}()'. Expected an object of type '{3}'. Actual type of the object is '{4}'.",
                            nameof(lifeTimeScope), GetType().FullName, nameof(Resolve),
                            typeof(AutofacLifeTimeScope), lifeTimeScope.GetType());

                        LogHelper.Context.Log.Error(errorMessage);

                        throw new ArgumentException(errorMessage, nameof(lifeTimeScope));
                    }

                    return _currentLifeTimeScope.Resolve(type);
                }
                finally
                {
                    _currentLifeTimeScope = previousLifeTimeScope;
                }
            }
        }

        public ILifeTimeScope StartLifeTimeScope()
        {
            lock (_lockObject)
            {
                var lifeTimeScope = new AutofacLifeTimeScope(Container.BeginLifetimeScope());
                return lifeTimeScope;
            }
        }

        public void StartMainLifeTimeScope()
        {
            if (_mainLifeTimeScope != null)
                return;

            lock (_lockObject)
            {
                if (_mainLifeTimeScope != null)
                    return;

                if (Container == null)
                {
                    LogHelper.Context.Log.Error($"The value of {nameof(Container)} is null. Make sure {nameof(ContainerBuilder)}.{nameof(ContainerBuilder.Build)}() is called first.");
                    throw new Exception("Container is not properly initialized.");
                }


                _mainLifeTimeScope = new AutofacLifeTimeScope(Container.BeginLifetimeScope());

                _mainLifeTimeScope.LifeTimeScopeTerminated += (sender, e) => { Container.Dispose(); };

                _currentLifeTimeScope = _mainLifeTimeScope;
            }
        }

        #endregion

        #region Member Functions

        public IContainer Container { get; private set; }

        [NotNull]
        public ContainerBuilder ContainerBuilder { get; }

        private void OnContainerBuilt([NotNull] IContainer container)
        {
            Container = container;
        }

        #endregion
    }
}