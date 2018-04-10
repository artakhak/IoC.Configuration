using System;
using System.Collections.Generic;
using System.Linq;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    public sealed class BindingImplementationConfigurationForFile : BindingImplementationConfiguration
    {
        #region  Constructors

        public BindingImplementationConfigurationForFile([NotNull] IServiceImplementationElement serviceImplementationElement)
            : base(GetTargetImplementationType(serviceImplementationElement), serviceImplementationElement.ImplementationType)
        {
            ResolutionScope = serviceImplementationElement.ResolutionScope;

            if (!serviceImplementationElement.Enabled)
                throw new Exception($"The value of '{serviceImplementationElement}.{nameof(IServiceImplementationElement.Enabled)}' cannot be null.");

            if (serviceImplementationElement.Parameters != null)
            {
                if (!serviceImplementationElement.Parameters.Enabled)
                    throw new Exception($"The value of '{serviceImplementationElement}.{nameof(IServiceImplementationElement.Parameters)}.{nameof(IConfigurationFileElement.Enabled)}' cannot be null.");

                var parameters = new LinkedList<IParameter>();

                foreach (var parameter in serviceImplementationElement.Parameters.AllParameters)
                    parameters.AddLast(new Parameter(parameter));

                Parameters = parameters.ToArray();
            }

            if (serviceImplementationElement.InjectedProperties != null)
            {
                if (!serviceImplementationElement.InjectedProperties.Enabled)
                    throw new Exception($"The value of '{serviceImplementationElement}.{nameof(IServiceImplementationElement.InjectedProperties)}.{nameof(IConfigurationFileElement.Enabled)}' cannot be null.");

                var injectedProperties = new LinkedList<IInjectedProperty>();

                foreach (var injectedProperty in serviceImplementationElement.InjectedProperties.AllProperties)
                    injectedProperties.AddLast(new InjectedProperty(injectedProperty));

                InjectedProperties = injectedProperties;
            }

#if DEBUG
// Will enable this code in release mode when Autofac implementation for this feature is available.
            //if (serviceImplementationElement.ConditionalInjectionType != ConditionalInjectionType.None)
            //    SetWhenInjectedIntoData(serviceImplementationElement.ConditionalInjectionType, serviceImplementationElement.WhenInjectedIntoType); 
#endif
        }

        #endregion

        #region Member Functions

        private static TargetImplementationType GetTargetImplementationType([NotNull] IServiceImplementationElement serviceImplementationElement)
        {
            if (serviceImplementationElement is ISelfBoundServiceElement)
                return TargetImplementationType.Self;

            return TargetImplementationType.Type;
        }

        [CanBeNull]
        public IEnumerable<IInjectedProperty> InjectedProperties { get; }

        /// <summary>
        ///     If the value is null, the parameters will be injected.
        ///     Otherwise, a constructor which matches the parameters by type and name will
        ///     be used to create an implementation.
        /// </summary>
        [CanBeNull]
        [ItemNotNull]
        public IParameter[] Parameters { get; }

        public override string ToString()
        {
            return $"{GetType().FullName}, TargetImplementationType :{TargetImplementationType}, ImplementationType: {ImplementationType}.";
        }

        #endregion
    }
}