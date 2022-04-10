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

using JetBrains.Annotations;
using OROptimizer.Serializer;
using System.Text;

namespace IoC.Configuration.DiContainer
{
    /// <summary>
    ///     A helper class that specific implementations (such as Autofac or Ninject) can use.
    /// </summary>
    public class DiManagerImplementationHelper
    {
        /// <summary>
        ///     Adds the code for on DI container ready method.
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
    }
}