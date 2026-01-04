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
using Microsoft.Extensions.Hosting;

namespace IoC.Configuration.DiContainerBuilder.FileBased
{
    public interface IIntegratesWithHostBuilder
    {
        /// <summary>
        /// Sets host <paramref name="hostBuilder"/> to register DI with in <see cref="IRegisterModulesWithHostBuilder{THost}"/>.
        /// Do not call <see cref="IHostBuilder"/> methods for setting up dependency injection or building the host in application, if this  using method is called,
        /// since IoC.Configuration will be responsible for this.
        /// </summary>
        /// <param name="hostBuilder">Host builder.</param>
        /// <returns>Returns an instance of <see cref="IHostIntegratedContainerInfo{THost}" /> with built host and container.</returns>
        IRegisterModulesWithHostBuilder<THost> WithHostBuilder<THost>([NotNull] IApplicationHostBuilder<THost> hostBuilder) where THost: class, IHost;
    }
}