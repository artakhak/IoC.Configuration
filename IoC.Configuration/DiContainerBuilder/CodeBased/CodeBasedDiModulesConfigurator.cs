using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.DiContainerBuilder.CodeBased
{
    public class CodeBasedDiModulesConfigurator : CodeBasedConfiguratorAbstr, ICodeBasedDiModulesConfigurator
    {
        #region  Constructors

        public CodeBasedDiModulesConfigurator([NotNull] CodeBasedConfiguration codeBasedConfiguration) : base(codeBasedConfiguration)
        {
        }

        #endregion

        #region ICodeBasedDiModulesConfigurator Interface Implementation

        public ICodeBasedDiModulesConfigurator AddDiModules(params IDiModule[] diModules)
        {
            _codeBasedConfiguration.AddDiModules(diModules);
            return this;
        }

        public ICodeBasedDiModulesConfigurator AddNativeModule(string nativeModuleClassFullName, string nativeModuleClassAssemblyFilePath, ParameterInfo[] nativeModuleConstructorParameters)
        {
            _codeBasedConfiguration.AddNativeModule(nativeModuleClassFullName, nativeModuleClassAssemblyFilePath, nativeModuleConstructorParameters);
            return this;
        }

        public ICodeBasedDiModulesConfigurator AddNativeModules(params object[] nativeModules)
        {
            _codeBasedConfiguration.AddNativeModules(nativeModules);
            return this;
        }

        public ICodeBasedContainerStarter RegisterModules()
        {
            _codeBasedConfiguration.RegisterModulesWithDiManager();
            return new CodeBasedContainerStarter(_codeBasedConfiguration);
        }

        #endregion
    }
}