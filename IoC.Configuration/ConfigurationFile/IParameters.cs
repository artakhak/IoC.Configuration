using System.Collections.Generic;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.ConfigurationFile
{
    public interface IParameters : IConfigurationFileElement
    {
        #region Current Type Interface

        [NotNull]
        [ItemNotNull]
        IEnumerable<IParameterElement> AllParameters { get; }

        ParameterInfo[] GetParameterValues();

        #endregion
    }
}