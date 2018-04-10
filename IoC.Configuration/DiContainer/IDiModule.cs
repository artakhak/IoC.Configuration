using System.Collections.Generic;
using IoC.Configuration.DiContainer.BindingsForCode;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer
{
    /// <summary>
    ///     Look at <see cref="ModuleAbstr" /> for implementation example for this method.
    ///     Normally, one would subclass from <see cref="ModuleAbstr" /> and override the method
    ///     <see cref="ModuleAbstr.AddServiceRegistrations" />.
    ///     Within the body of <see cref="ModuleAbstr.AddServiceRegistrations" />, one can use statements like:
    ///     Bind &lt;IService1&gt;().OnlyIfNotRegistered().To&lt;Service1&gt;
    ///     ().SetResolutionScope(DiResolutionScope.Singleton);
    /// </summary>
    public interface IDiModule
    {
        #region Current Type Interface

        void Init([NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder);

        /// <summary>
        ///     Adds bindings and validates the added bindings.
        /// </summary>
        void Load();


        [NotNull]
        [ItemNotNull]
        IReadOnlyList<BindingConfigurationForCode> ServiceBindingConfigurations { get; }

        #endregion
    }
}