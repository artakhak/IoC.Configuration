//using JetBrains.Annotations;

//namespace IoC.Configuration.DiContainerBuilder.CodeBased
//{
//    public class RegisterModulesWithHostBuilder : CodeBasedConfiguratorAbstr, IRegisterModulesWithHostBuilder
//    {
//        /// <summary>
//        ///     Initializes a new instance of the <see cref="CodeBasedConfiguratorAbstr" /> class.
//        /// </summary>
//        /// <param name="codeBasedConfiguration">The code based configuration.</param>
//        public RegisterModulesWithHostBuilder([NotNull] CodeBasedConfiguration codeBasedConfiguration) : base(codeBasedConfiguration)
//        {
//        }

//        /// <inheritdoc />
//        public IHostIntegratedContainerInfo RegisterServiceProviderAndBuildApp()
//        {
//            return this.CodeBasedConfiguration.RegisterServiceProviderAndBuildApp();
//        }
//    }
//}