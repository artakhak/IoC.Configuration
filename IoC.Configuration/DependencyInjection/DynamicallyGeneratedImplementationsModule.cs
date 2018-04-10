using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.DependencyInjection
{
    public class DynamicallyGeneratedImplementationsModule : ModuleAbstr
    {
        #region Member Variables

        [NotNull]
        private readonly string _dynamicallyGeneratedAssemblyFilePath;

        [NotNull]
        [ItemNotNull]
        private readonly IEnumerable<InterfaceImplementationInfo> _interfaceImplementationsInfo;

        #endregion

        #region  Constructors

        public DynamicallyGeneratedImplementationsModule([NotNull] [ItemNotNull] IEnumerable<InterfaceImplementationInfo> interfaceImplementationsInfo, [NotNull] string dynamicallyGeneratedAssemblyFilePath)
        {
            _interfaceImplementationsInfo = interfaceImplementationsInfo;
            _dynamicallyGeneratedAssemblyFilePath = dynamicallyGeneratedAssemblyFilePath;
        }

        #endregion

        #region Member Functions

        protected override void AddServiceRegistrations()
        {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(_dynamicallyGeneratedAssemblyFilePath);

            foreach (var interfaceImplementationInfo in _interfaceImplementationsInfo)
            {
                var implementationType = assembly.GetType(interfaceImplementationInfo.ImplementingClassName);
                Bind(interfaceImplementationInfo.InterfaceType).To(implementationType).SetResolutionScope(DiResolutionScope.Singleton);
            }
        }

        #endregion

        #region Nested Types

        public class InterfaceImplementationInfo
        {
            #region  Constructors

            public InterfaceImplementationInfo([NotNull] Type interfaceType, [NotNull] string implementingClassName)
            {
                InterfaceType = interfaceType;
                ImplementingClassName = implementingClassName;
            }

            #endregion

            #region Member Functions

            [NotNull]
            public string ImplementingClassName { get; }

            [NotNull]
            public Type InterfaceType { get; }

            #endregion
        }

        #endregion
    }
}