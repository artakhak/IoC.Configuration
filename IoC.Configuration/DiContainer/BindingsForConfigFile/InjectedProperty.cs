using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    public class InjectedProperty : NamedValue, IInjectedProperty
    {
        #region  Constructors

        public InjectedProperty([NotNull] IInjectedPropertyElement injectedPropertyElement) : base(injectedPropertyElement)
        {
        }

        #endregion
    }
}