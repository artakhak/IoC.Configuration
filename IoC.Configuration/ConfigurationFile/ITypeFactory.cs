using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public interface ITypeFactory : IConfigurationFileElement
    {
        #region Current Type Interface

        /// <summary>
        ///     Null only of element is disabled
        /// </summary>
        [NotNull]
        MethodInfo ImplementedMethodInfo { get; }

        [NotNull]
        [ItemNotNull]
        IEnumerable<ITypeFactoryReturnedTypesIfSelector> ReturnedIfTypeSelectorsForIfCase { get; }

        [NotNull]
        Type ReturnedItemsType { get; }

        [NotNull]
        ITypeFactoryReturnedTypesSelector ReturnedTypeSelectorForDefaultCase { get; }

        #endregion
    }
}