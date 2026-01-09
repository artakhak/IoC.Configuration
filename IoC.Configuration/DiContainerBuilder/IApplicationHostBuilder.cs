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
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.DiContainerBuilder
{
    /// <summary>
    /// Defines methods for building an application host with support for dependency injection configuration and
    /// service provider customization.
    /// </summary>
    public interface IApplicationHostBuilder
    {
        /// <summary>
        /// Configures the builder to use a specific service provider factory for creating the service provider.
        /// This allows customization of the underlying dependency injection container.
        /// </summary>
        /// <typeparam name="TContainerBuilder">The type of the container builder used by the service provider factory.</typeparam>
        /// <param name="factory">The service provider factory that will be used to create the dependency injection container.</param>
        void UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : class;


        /// <summary>
        /// Configures the container used by the application host with the specified delegate.
        /// This method allows customization of the dependency injection container by modifying its configuration.
        /// </summary>
        /// <typeparam name="TContainerBuilder">The type of the container builder used to configure the dependency injection container.</typeparam>
        /// <param name="configureDelegate">A delegate that provides the logic for configuring the container.
        /// The delegate receives the <see cref="HostBuilderContext"/> and an instance of <typeparamref name="TContainerBuilder"/> to modify its behavior.</param>
        void ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate);
    }

    /// <summary>
    /// Represents a builder for constructing and configuring an application host with dependency injection
    /// and service provider customization.
    /// </summary>
    public interface IApplicationHostBuilder<out THost>: IApplicationHostBuilder where THost: class, IHost
    {
        /// <summary>
        /// Builds and returns the host instance configured by the application host builder.
        /// This method finalizes the configuration process and creates the initialized host.
        /// </summary>
        /// <returns>The built host instance of type <typeparamref name="THost"/>.</returns>
        THost Build();
    }
}
