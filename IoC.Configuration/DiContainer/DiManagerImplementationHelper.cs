using System;
using System.Text;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration.DiContainer
{
    public class DiManagerImplementationHelper
    {
        #region Member Functions

        public static void AddCodeForOnDiContainerReadyMethod([NotNull] StringBuilder moduleClassContents)
        {
            moduleClassContents.AppendLine($"private {typeof(IDiContainer).FullName} _diContainer;");
            moduleClassContents.AppendLine($"private {typeof(ITypeBasedSimpleSerializerAggregator).FullName} _parameterSerializer;");

            moduleClassContents.AppendLine($"public void {HelpersIoC.OnDiContainerReadyMethodName}({typeof(IDiContainer).FullName} diContainer)");

            moduleClassContents.AppendLine("{");
            moduleClassContents.AppendLine("_diContainer=diContainer;");
            moduleClassContents.AppendLine($"_parameterSerializer = _diContainer.{nameof(IDiContainer.Resolve)}<{typeof(ITypeBasedSimpleSerializerAggregator).FullName}>();");
            moduleClassContents.AppendLine("}");
        }

        public static string GenerateCodeForDeserializedParameterValue([NotNull] INamedValue namedValue)
        {
            var code = new StringBuilder();

            if (namedValue.ValueAsString == null)
                code.Append($"default({namedValue.ValueType.FullName})");
            else
                code.Append($"_parameterSerializer.Deserialize<{namedValue.ValueType.FullName}>(\"{namedValue.ValueAsString}\")");

            return code.ToString();
        }

        public static void ThrowUnsuportedEnumerationValue(Enum enumValue)
        {
            throw new ArgumentException($"Unsupported value: {enumValue}.");
        }

        /// <summary>
        ///     Throws n exception, if the feature specified in parameter <paramref name="nonStandardFeatures" /> is not supported
        ///     by container implementation specified in parameter <paramref name="diManager" />.
        /// </summary>
        /// <param name="nonStandardFeatures"></param>
        /// <param name="diManager"></param>
        /// <exception cref="Exception">Always throws this exception.</exception>
        [Obsolete("Non-standard features will not be used.")]
        public static void ThrowUnsupportedFeature(NonStandardFeatures nonStandardFeatures, [CanBeNull] IDiManager diManager = null)
        {
            var errorMessage = new StringBuilder();
            errorMessage.Append($"The feature '{typeof(NonStandardFeatures).FullName}.{nonStandardFeatures}' is not supported by dependency injection container");

            if (diManager != null)
                errorMessage.Append($" {diManager.DiContainerName}");

            errorMessage.Append(".");

            throw new Exception(errorMessage.ToString());
        }

        /// <summary>
        ///     Throw an exception, if the value of <see cref="IServiceImplementationElement.ResolutionScope" /> is unsupported by
        ///     the implementation of <see cref="IDiManager" />.
        /// </summary>
        /// <param name="serviceImplementationElement"></param>
        /// <exception cref="Exception">Always throws this exception.</exception>
        public static void ThrowUnsupportedResolutionScope(BindingImplementationConfigurationForFile serviceImplementationElement)
        {
            throw new ArgumentException($"Unsupported value of '{typeof(DiResolutionScope)}'. The value is '{serviceImplementationElement.ResolutionScope}' for service implementation '{serviceImplementationElement}'.");
        }

        #endregion
    }
}