using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace IoC.Configuration.Tests
{
    public static class DiManagerHelpers
    {
        #region Member Variables

        [NotNull]
        private static readonly Dictionary<DiImplementationType, DiImplementationInfo> _diImplementationTypeToDiImplementationInfo = new Dictionary<DiImplementationType, DiImplementationInfo>();

        public static readonly string DynamicallyLoadedDllsFolder;

        public static readonly string ThirdPartyLibsFolder;

        #endregion

        #region  Constructors

        static DiManagerHelpers()
        {
            var testDllsFolderPath = Helpers.GetTestDllsFolderPath();

            ThirdPartyLibsFolder = Path.Combine(testDllsFolderPath, "ThirdPartyLibs");
            DynamicallyLoadedDllsFolder = Path.Combine(testDllsFolderPath, "DynamicallyLoadedDlls");

            ImplementationTypes = new List<DiImplementationType>(Enum.GetValues(typeof(DiImplementationType)).Cast<DiImplementationType>());

            _diImplementationTypeToDiImplementationInfo[DiImplementationType.Autofac] =
                new DiImplementationInfo(DiImplementationType.Autofac,
                    Path.Combine(testDllsFolderPath, "ContainerImplementations", "Autofac"),
                    Path.Combine(testDllsFolderPath, "ContainerImplementations", "Autofac", "IoC.Configuration.Autofac.dll"),
                    "IoC.Configuration.Autofac.AutofacDiManager",
                    "IoC.Configuration.Autofac.AutofacDiContainer");

            _diImplementationTypeToDiImplementationInfo[DiImplementationType.Ninject] =
                new DiImplementationInfo(DiImplementationType.Ninject,
                    Path.Combine(testDllsFolderPath, "ContainerImplementations", "Ninject"),
                    Path.Combine(testDllsFolderPath, "ContainerImplementations", "Ninject", "IoC.Configuration.Ninject.dll"),
                    "IoC.Configuration.Ninject.NinjectDiManager",
                    "IoC.Configuration.Ninject.NinjectDiContainer");
        }

        #endregion

        #region Member Functions

        public static DiImplementationInfo GetDiImplementationInfo(DiImplementationType diImplementationType)
        {
            return _diImplementationTypeToDiImplementationInfo[diImplementationType];
        }

        public static IEnumerable<DiImplementationInfo> ImplementationInfos => _diImplementationTypeToDiImplementationInfo.Values;

        public static IReadOnlyList<DiImplementationType> ImplementationTypes { get; }

        #endregion
    }
}