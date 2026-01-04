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
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public interface IRegisterModulesWithHostBuilder<out THost> where THost: class, IHost
    {
        /// <summary>
        /// Registers the modules using <see cref="IHostBuilder"/>, builds the host by calling <see cref="IHostBuilder.Build()"/> and starts the container.
        /// </summary>
        /// <param name="servicesWereRegistered">
        /// A callback action that will be executed after services are registered.
        /// At this point the DI container is not yet built, and cannot be used to resolve services (container will be
        /// ready after the call to the method is complete), however the caller of <see cref="RegisterServiceProviderAndBuildApp"/>
        /// can use this callback to perform additional operations after services are registered, such as adding REST API services, etc.
        /// </param>
        /// <returns>Returns an instance of <see cref="IHostIntegratedContainerInfo{THost}" /> with built host and container.</returns>
        IHostIntegratedContainerInfo<THost> RegisterServiceProviderAndBuildApp([CanBeNull] Action servicesWereRegistered = null);
    }
}