namespace TestsSharedLibrary.DependencyInjection
{
    public class DiPackagesData
    {
        /// <summary>
        /// Nuget root folder.
        /// </summary>
        public string NugetRoot { get; set; }

        /// <summary>
        /// Autofac package relative path.
        /// </summary>
        public string Autofac { get; set; }

        /// <summary>
        /// Autofac.Extensions.DepencyInjection package relative path.
        /// </summary>
        public string AutofacExtensionsDepencyInjection { get; set; }

        /// <summary>
        /// Ninject package relative path.
        /// </summary>
        public string Ninject { get; set; }

        /// <summary>
        /// IoC.Configuration.Aautofac package relative path.
        /// </summary>
        public string IocConfigurationAutofac { get; set; }

        /// <summary>
        /// IoC.Configuration.Ninject package relative path.
        /// </summary>
        public string IocConfigurationNinject { get; set; }
    }
}