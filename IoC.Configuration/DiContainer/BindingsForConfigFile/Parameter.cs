using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    public class Parameter : NamedValue, IParameter
    {
        #region  Constructors

        public Parameter([NotNull] IParameterElement parameterElement) : base(parameterElement)
        {
        }

        #endregion
    }
}