using System;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IServiceImplementationElement : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        IAssembly Assembly { get; }

        ConditionalInjectionType ConditionalInjectionType { get; }

        [NotNull]
        Type ImplementationType { get; }

        [CanBeNull]
        IInjectedProperties InjectedProperties { get; }

        [CanBeNull]
        IParameters Parameters { get; }

        DiResolutionScope ResolutionScope { get; }

        [CanBeNull]
        Type WhenInjectedIntoType { get; }

        #endregion
    }
}