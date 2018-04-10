using System;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface ITypeFactoryReturnedType : IConfigurationFileElement
    {
        #region Current Type Interface

        /// <summary>
        ///     Null only of element is disabled
        /// </summary>
        [NotNull]
        Type ReturnedType { get; }

        #endregion
    }
}