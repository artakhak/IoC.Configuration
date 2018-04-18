// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Text;
using IoC.Configuration.ConfigurationFile;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using JetBrains.Annotations;
using OROptimizer.Serializer;

namespace IoC.Configuration.DiContainer
{
    /// <summary>
    /// A helper class that specific implementations (such as Autofac or Ninject) can use.
    /// </summary>
    public class DiManagerImplementationHelper
    {
        #region Member Functions        
        /// <summary>
        /// Adds the code for on di container ready method.
        /// </summary>
        /// <param name="moduleClassContents">The module class contents.</param>
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

        /// <summary>
        /// Generates the code for deserialized parameter value.
        /// </summary>
        /// <param name="namedValue">The named value.</param>
        /// <returns></returns>
        public static string GenerateCodeForDeserializedParameterValue([NotNull] INamedValue namedValue)
        {
            var code = new StringBuilder();

            if (namedValue.ValueAsString == null)
                code.Append($"default({namedValue.ValueType.FullName})");
            else
                code.Append($"_parameterSerializer.Deserialize<{namedValue.ValueType.FullName}>(\"{namedValue.ValueAsString}\")");

            return code.ToString();
        }

        /// <summary>
        /// Throws the unsuported enumeration value.
        /// </summary>
        /// <param name="enumValue">The enum value.</param>
        /// <exception cref="System.ArgumentException"></exception>
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