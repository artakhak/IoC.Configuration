using System;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer
{
    public interface IDiContainer : IDisposable
    {
        #region Current Type Interface

        [CanBeNull]
        ILifeTimeScope CurrentLifeTimeScope { get; }

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

        [NotNull]
        ILifeTimeScope StartLifeTimeScope();

        /// <summary>
        ///     Starts a new life time scope and assigns it to <see cref="MainLifeTimeScope" />.
        /// </summary>
        void StartMainLifeTimeScope();

        #endregion
    }
}