using System;
using System.Collections.Generic;
using IoC.Configuration.DiContainer.BindingsForCode;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.DiContainer
{
    /// <summary>
    ///     Normally, one would subclass from <see cref="ModuleAbstr" /> and override the method
    ///     <see cref="ModuleAbstr.AddServiceRegistrations" />.
    ///     Within the body of <see cref="ModuleAbstr.AddServiceRegistrations" />, one can use statements like:
    ///     Bind &lt;IService1&gt;().OnlyIfNotRegistered().To&lt;Service1&gt;
    ///     ().SetResolutionScope(DiResolutionScope.Singleton);
    /// </summary>
    public abstract class ModuleAbstr : IDiModule
    {
        #region Member Variables

        private readonly List<BindingConfigurationForCode> _serviceBindingConfigurations = new List<BindingConfigurationForCode>();
        private IServiceRegistrationBuilder _serviceRegistrationBuilder;

        #endregion

        #region IDiModule Interface Implementation

        public void Init(IServiceRegistrationBuilder serviceRegistrationBuilder)
        {
            if (_serviceRegistrationBuilder != null)
            {
                var error = $"There can be only a single call to '{GetType().FullName}.{nameof(Init)}({typeof(IServiceRegistrationBuilder).FullName})'";
                LogHelper.Context.Log.Error(error);

                throw new Exception("Method was already executed.");
            }

            _serviceRegistrationBuilder = serviceRegistrationBuilder;
        }

        public IReadOnlyList<BindingConfigurationForCode> ServiceBindingConfigurations => _serviceBindingConfigurations;

        #endregion

        #region Current Type Interface

        /// <summary>
        ///     Override this method to register services. The body of overridden method might have statements like:
        ///     Bind &lt;IService1&gt;().OnlyIfNotRegistered().To&lt;Service1&gt;
        ///     ().SetResolutionScope(DiResolutionScope.Singleton);
        /// </summary>
        protected abstract void AddServiceRegistrations();

        public virtual void Load()
        {
            _serviceRegistrationBuilder.BindingConfigurationAdded += BindingConfigurationAdded;

            AddServiceRegistrations();

            _serviceRegistrationBuilder.BindingConfigurationAdded -= BindingConfigurationAdded;

            foreach (var serviceBindingConfiguration in _serviceBindingConfigurations)
                serviceBindingConfiguration.Validate();
        }

        #endregion

        #region Member Functions

        [NotNull]
        protected IBindingGeneric<TService> Bind<TService>()
        {
            return _serviceRegistrationBuilder.Bind<TService>();
        }

        [NotNull]
        protected IBindingNonGeneric Bind(Type serviceType)
        {
            return _serviceRegistrationBuilder.Bind(serviceType);
        }

        private void BindingConfigurationAdded([CanBeNull] object sender, [NotNull] BindingConfigurationAddedEventArgs e)
        {
            _serviceBindingConfigurations.Add(e.BindingConfiguration);
        }

        #endregion
    }
}