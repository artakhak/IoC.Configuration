using System;
using JetBrains.Annotations;

namespace IoC.Configuration
{
    public static class MessagesHelper
    {
        #region Member Functions

        /// <summary>
        ///     Returns a string: The value of <paramref name="registerIfNotRegisteredSubjectName" /> can be true only if there is
        ///     a single implementation for the service.
        /// </summary>
        /// <param name="registerIfNotRegisteredSubjectName"></param>
        [NotNull]
        public static string GetMultipleImplementationsWithRegisterIfNotRegisteredOptionMessage([NotNull] string registerIfNotRegisteredSubjectName)
        {
            return $"The value of {registerIfNotRegisteredSubjectName} can be true only if there is a single implementation for the service.";
        }

        [NotNull]
        public static string GetNoImplementationsForServiceMessage([NotNull] Type serviceType)
        {
            return $"No implementation is provided for service '{serviceType.FullName}'.";
        }

        public static string GetServiceImplmenentationTypeAssemblyBelongsToPluginMessage([NotNull] Type implementationType, [NotNull] string assemblyAlias, [NotNull] string pluginName)
        {
            return $"The settings requestor type '{implementationType.FullName}' is defined in assembly '{assemblyAlias}' which belongs to plugin '{pluginName}'. The assembly where the type is defined should not be associated with any plugin.";
        }

        #endregion
    }
}