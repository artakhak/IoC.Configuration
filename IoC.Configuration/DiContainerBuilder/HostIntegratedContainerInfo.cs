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
using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.DiContainerBuilder
{
    /// <summary>
    /// Represents the integration of a host with a container for dependency injection.
    /// This class provides access to both the host instance and the container information.
    /// </summary>
    /// <typeparam name="THost">
    /// The type of the host, required to be a class that implements <see cref="IHost"/>.
    /// </typeparam>
    public class HostIntegratedContainerInfo<THost>: IHostIntegratedContainerInfo<THost> where THost: class, IHost
    {
        /// <summary>
        /// Represents the integration of an application host with a dependency injection container.
        /// Provides access to both the host instance and information about the container.
        /// </summary>
        /// <typeparam name="THost">
        /// The type of the host, which must be a class implementing <see cref="IHost"/>.
        /// </typeparam>
        public HostIntegratedContainerInfo(THost host, IContainerInfo containerInfo)
        {
            Host = host;
            ContainerInfo = containerInfo;
        }

        /// <summary>
        /// Gets the host instance of type <typeparamref name="THost"/> that is associated with the container.
        /// </summary>
        /// <remarks>
        /// The host represents the primary application entry point, typically used
        /// in dependency injection setups where the host integrates with a container.
        /// It can be used to interact with or query the application lifecycle or services registered within the DI container.
        /// </remarks>
        public THost Host { get; }

        /// <summary>
        /// Gets the container information instance associated with the host.
        /// </summary>
        /// <remarks>
        /// Represents details and context of the dependency injection container instance
        /// that is integrated with the host. This provides the necessary information for
        /// interacting with or managing the container lifecycle.
        /// </remarks>
        public IContainerInfo ContainerInfo { get; }
    }
}
