using System;
using Autofac;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.Autofac
{
    public class AutofacLifeTimeScope : LifeTimeScopeStandard
    {
        #region Member Variables

        [NotNull]
        private readonly ILifetimeScope _lifeTimeScope;

        #endregion

        #region  Constructors

        public AutofacLifeTimeScope([NotNull] ILifetimeScope lifeTimeScope)
        {
            _lifeTimeScope = lifeTimeScope;
        }

        #endregion

        #region Member Functions

        public override void Dispose()
        {
            base.Dispose();

            _lifeTimeScope.Dispose();
        }

        public T Resolve<T>() where T : class
        {
            return _lifeTimeScope.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _lifeTimeScope.Resolve(type);
        }

        #endregion
    }
}