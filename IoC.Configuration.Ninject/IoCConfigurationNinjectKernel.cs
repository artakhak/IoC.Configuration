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
using System.Linq;
using Ninject;
using Ninject.Activation;
using Ninject.Infrastructure.Introspection;
using Ninject.Modules;
using Ninject.Planning.Bindings;

namespace IoC.Configuration.Ninject
{
    // Ninject 3.3.4 has the bug described below:
    // If we configure binding for collection type (i.e., IReadOnlyLost<T>, IList<T>, ICollection<T>, 
    // IEnumerable<T>, or T[]), the returned collection in Ninject V 3.3.4 is empty list
    // Example of binding configuration that results in this issue:
    //kernel.Bind<IEnumerable<IDog>>().ToMethod(context =>
    //    new IDog[]
    //    {
    //        new Dog(10), new Dog(20)
    //    }).InSingletonScope();
    // To configure collection bindings we have to bind multiple times as displayed below, in which case Ninject generates a binding for IEnumerable<T>. 
    // kernel.Bind<IDog>()..ToMethod(context => new Dog(10));
    // kernel.Bind<IDog>()..ToMethod(context => new Dog(20));
    // This implementation of KernelBase supports binding of collections using code like
    // kernel.Bind<IEnumerable<T>>().ToMethod(context => someList).
    // The mentioned issue has been resolved in latest source code at https://github.com/ninject/Ninject, however no new Niuget package have been published yet for this fix.
    // This class will be deprecated when next version of Ninject is available on Nuget.org (supposedly V5).
    // When next version of Ninject is available, we will change the dependency to latest Ninject and will deprecate this class.
    // At that point IKernel will be replaced with IReadOnlyKernel (IKernel is deprecated in latest Ninject)
    // and the container will be built and services will be resolved as follows:
    // var kernelConfiguration = new KernelConfiguration(INinjectSettings settings, INinjectModule[] modules);
    // IReadOnlyKernel readOnlyKernel = kernelConfiguration.BuildReadOnlyKernel();
    // var service = readOnlyKernel.Get<T>();
    public class IoCConfigurationNinjectKernel : StandardKernel
    {
        private readonly IBindingPrecedenceComparer bindingPrecedenceComparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardKernel"/> class.
        /// </summary>
        /// <param name="modules">The modules to load into the kernel.</param>
        public IoCConfigurationNinjectKernel(params INinjectModule[] modules)
            : base(modules)
        {
            this.bindingPrecedenceComparer = this.Components.Get<IBindingPrecedenceComparer>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardKernel"/> class.
        /// </summary>
        /// <param name="settings">The configuration to use.</param>
        /// <param name="modules">The modules to load into the kernel.</param>
        public IoCConfigurationNinjectKernel(INinjectSettings settings, params INinjectModule[] modules)
            : base(settings, modules)
        {
            this.bindingPrecedenceComparer = this.Components.Get<IBindingPrecedenceComparer>();
        }


        public override IEnumerable<object> Resolve(IRequest request)
        {
            var doSpecialHandling = request.Service.IsArray;

            if (!doSpecialHandling && request.Service.IsGenericType &&
                request.Service.GenericTypeArguments.Length == 1)
            {
                var genericTypeDefinition = request.Service.GetGenericTypeDefinition();

                // These are all the types (+ array) that are handled pecially in Ninject 3.3.4
                if (genericTypeDefinition == typeof(IEnumerable<>) ||
                    genericTypeDefinition == typeof(List<>) || genericTypeDefinition == typeof(IList<>) ||
                    genericTypeDefinition == typeof(ICollection<>))
                    doSpecialHandling = true;
            }

            if (!doSpecialHandling)
                return base.Resolve(request);

            var satisfiedBindings = this.GetBindings(request.Service)
                .Where(this.SatifiesRequest(request));


            var satisfiedBindingEnumerator = satisfiedBindings.GetEnumerator();

            if (!satisfiedBindingEnumerator.MoveNext())
                return base.Resolve(request);

            return this.Resolve(request, satisfiedBindings, satisfiedBindingEnumerator);
        }

        // This method is copied and modified from KernelBase.Resolve(IRequest request, bool handleMissingBindings)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="satisfiedBindings"></param>
        /// <param name="satisfiedBindingEnumerator">An enumerator of <see cref="IBinding"/>.
        /// satisfiedBindingEnumerator.MoveNext() was called and returned true before calling this method, therefore 
        /// satisfiedBindingEnumerator.Current is not null.</param>
        /// <returns></returns>
        /// <exception cref="ActivationException"></exception>
        private IEnumerable<object> Resolve(IRequest request, //bool handleMissingBindings,
            IEnumerable<IBinding> satisfiedBindings, IEnumerator<IBinding> satisfiedBindingEnumerator)
        {
            if (request.IsUnique)
            {
                var selectedBinding = satisfiedBindingEnumerator.Current;

                if (satisfiedBindingEnumerator.MoveNext() &&
                    this.bindingPrecedenceComparer.Compare(selectedBinding, satisfiedBindingEnumerator.Current) == 0)
                {
                    if (request.IsOptional && !request.ForceUnique)
                    {
                        return Enumerable.Empty<object>();
                    }

                    var formattedBindings =
                        from binding in satisfiedBindings
                        let context = this.CreateContext(request, binding)
                        select binding.Format(context);

                    throw new ActivationException(ExceptionFormatter.CouldNotUniquelyResolveBinding(
                        request,
                        formattedBindings.ToArray()));
                }

                return new[] {this.CreateContext(request, selectedBinding).Resolve()};
            }

            if (satisfiedBindings.Any(binding => !binding.IsImplicit))
            {
                satisfiedBindings = satisfiedBindings.Where(binding => !binding.IsImplicit);
            }

            return satisfiedBindings
                .Select(binding => this.CreateContext(request, binding).Resolve());
        }
    }
}