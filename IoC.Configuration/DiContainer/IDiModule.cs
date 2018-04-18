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
using System.Collections.Generic;
using IoC.Configuration.DiContainer.BindingsForCode;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer
{
    /// <summary>
    ///     Look at <see cref="ModuleAbstr" /> for implementation example for this method.
    ///     Normally, one would subclass from <see cref="ModuleAbstr" /> and override the method
    ///     <see cref="ModuleAbstr.AddServiceRegistrations" />.
    ///     Within the body of <see cref="ModuleAbstr.AddServiceRegistrations" />, one can use statements like:
    ///     Bind &lt;IService1&gt;().OnlyIfNotRegistered().To&lt;Service1&gt;
    ///     ().SetResolutionScope(DiResolutionScope.Singleton);
    /// </summary>
    public interface IDiModule
    {
        #region Current Type Interface        
        /// <summary>
        /// Initializes the specified service registration builder.
        /// </summary>
        /// <param name="serviceRegistrationBuilder">The service registration builder.</param>
        void Init([NotNull] IServiceRegistrationBuilder serviceRegistrationBuilder);

        /// <summary>
        ///     Adds bindings and validates the added bindings.
        /// </summary>
        void Load();

        /// <summary>
        /// Gets the service binding configurations.
        /// </summary>
        /// <value>
        /// The service binding configurations.
        /// </value>
        [NotNull, ItemNotNull]
        IReadOnlyList<BindingConfigurationForCode> ServiceBindingConfigurations { get; }

        #endregion
    }
}