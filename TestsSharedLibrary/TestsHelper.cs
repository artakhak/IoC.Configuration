using OROptimizer.Diagnostics.Log;
using TestsSharedLibrary.Diagnostics.Log;
using System.Collections.Generic;
using System.IO;
using IoC.Configuration.DiContainerBuilder;
using JetBrains.Annotations;
using TestsSharedLibrary.DependencyInjection;
using System.Linq;
using OROptimizer;

namespace TestsSharedLibrary
{
    public static class TestsHelper
    {
        #region Member Functions        
        /// <summary>
        /// Sets ups the logger and sets the log level to <see cref="LogLevel.Error"/>.
        /// Also, resets the test statistics
        /// </summary>
        public static void SetupLogger()
        {
            LogHelper.RemoveContext();
            //if (!LogHelper.IsContextInitialized)
            LogHelper.RegisterContext(new LogHelper4TestsContext());
            Log4Tests.LogLevel = LogLevel.Error;
            Log4Tests.ResetLogStatistics();
        }

        /// <summary>
        /// Creates the IoC container and load the modules specified in <paramref name="modules"/> parameter.
        /// No configuration file needed.
        /// </summary>
        /// <param name="diPackagesData">The DI packages data.</param>
        /// <param name="modules">The modules.</param>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="additionalProbingPaths">Additional probing paths.</param>
        /// <returns></returns>
        public static IContainerInfo StartIoCContainer([NotNull] DiPackagesData diPackagesData,
                                                        [NotNull, ItemNotNull] IEnumerable<IoC.Configuration.DiContainer.IDiModule> diModules,
                                                        DiImplementationType implementationType = DiImplementationType.Ninject,
                                                        [CanBeNull] IEnumerable<string> additionalProbingPaths = null)
        {
            LinkedList<string> additionalProbingPaths2 = null;
            if (additionalProbingPaths != null)
                additionalProbingPaths2 = new LinkedList<string>(additionalProbingPaths);
            else
                additionalProbingPaths2 = new LinkedList<string>();

            DiImplementationInfo diImplementationInfo;

            string diManagerFolder = null;

            switch (implementationType)
            {
                case DiImplementationType.Autofac:
                    additionalProbingPaths2.AddLast(GetPackageDllFolder(diPackagesData.NugetRoot, diPackagesData.Autofac));
                    additionalProbingPaths2.AddLast(GetPackageDllFolder(diPackagesData.NugetRoot, diPackagesData.AutofacExtensionsDepencyInjection));

                    diManagerFolder = GetPackageDllFolder(diPackagesData.NugetRoot, diPackagesData.IocConfigurationAutofac);
                   
                    diImplementationInfo = new DiImplementationInfo(implementationType, diManagerFolder, 
                        Path.Combine(diManagerFolder, "IoC.Configuration.Autofac.dll"),
                        "IoC.Configuration.Autofac.AutofacDiManager", "IoC.Configuration.Autofac.AutofacDiContainer");
                    
                    break;

                case DiImplementationType.Ninject:
                        additionalProbingPaths2.AddLast(GetPackageDllFolder(diPackagesData.NugetRoot, diPackagesData.Ninject));

                        diManagerFolder = GetPackageDllFolder(diPackagesData.NugetRoot, diPackagesData.IocConfigurationNinject);

                        diImplementationInfo = new DiImplementationInfo(implementationType, diManagerFolder,
                            Path.Combine(diManagerFolder, "IoC.Configuration.Ninject.dll"),
                            "IoC.Configuration.Ninject.NinjectDiManager", "IoC.Configuration.Ninject.NinjectDiContainer");
                    break;
                default:
                    throw new System.Exception($"Invalid value: {implementationType}.");
            }

            additionalProbingPaths2.AddLast(diImplementationInfo.DiManagerFolder);

            return new DiContainerBuilder()
                   .StartCodeBasedDi(diImplementationInfo.DiManagerClassName,
                                  diImplementationInfo.DiManagerAssemblyPath,
                                  new ParameterInfo[0],
                                  System.AppDomain.CurrentDomain.BaseDirectory,
                                  additionalProbingPaths2.ToArray())
                   .WithoutPresetDiContainer()
                   .AddDiModules(diModules.ToArray())
                   .RegisterModules()
                   .Start();
        }

        private static string GetPackageDllFolder([NotNull]string nugetRoot, [NotNull] string packageRelativePath)
        {
            packageRelativePath = packageRelativePath.Trim();
            if (packageRelativePath.StartsWith($"{Path.PathSeparator}"))
                packageRelativePath = packageRelativePath.Substring(1);

            return Path.Combine(nugetRoot, packageRelativePath);
        }
        #endregion
    }
}