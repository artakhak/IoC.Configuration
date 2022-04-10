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

using IoC.Configuration.DiContainer;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace IoC.Configuration.Extensions.AssemblyScanning
{
    /// <summary>
    /// Scans the assemblies and registers types using self binding.
    /// </summary>
    public class AssemblyScannerForSelfBoundTypesModule : ModuleAbstr
    {
        [NotNull] [ItemNotNull] private readonly IEnumerable<System.Reflection.Assembly> _assembliesToScan;
        [NotNull] private readonly TypeShouldBeSelfBoundDelegate _typeShouldBeSelfBoundDelegate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="assembliesToScan">Assemblies to scan.</param>
        /// <param name="typeShouldBeSelfBoundDelegate">A predicate that determines if a type should be self bound.</param>
        public AssemblyScannerForSelfBoundTypesModule([NotNull, ItemNotNull] IEnumerable<System.Reflection.Assembly> assembliesToScan,
                                                      [NotNull] TypeShouldBeSelfBoundDelegate typeShouldBeSelfBoundDelegate)
        {
            _assembliesToScan = assembliesToScan;
            _typeShouldBeSelfBoundDelegate = typeShouldBeSelfBoundDelegate;
        }

        /// <inheritdoc />
        protected override void AddServiceRegistrations()
        {
            foreach (var assembly in _assembliesToScan)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsAbstract && !type.IsInterface)
                    {
                        var publicConstructors = type.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public); //.Where(x => x.IsPublic && x.IsStatic);

                        if (publicConstructors.Length > 0)
                        {
                            var result = _typeShouldBeSelfBoundDelegate(type, publicConstructors);
                           
                            if (result.shouldBeSelfBound)
                            {
                                var selfBindingRegistrationResult = result.selfBindingRegistrationResult;

                                var binding = Bind(type).OnlyIfNotRegistered().ToSelf().SetResolutionScope(selfBindingRegistrationResult.SelfBindingDiResolutionScope);

                                if (selfBindingRegistrationResult.OnImplementationObjectActivatedAction != null)
                                {
                                    binding.OnImplementationObjectActivated(selfBindingRegistrationResult.OnImplementationObjectActivatedAction);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Self binding registration predicate result.
    /// </summary>
    public class SelfBindingRegistrationResult
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="selfBindingDiResolutionScope">Resolution scope to use for self binding.</param>
        public SelfBindingRegistrationResult(DiResolutionScope selfBindingDiResolutionScope)
        {
            SelfBindingDiResolutionScope = selfBindingDiResolutionScope;
        }

        /// <summary>
        /// Resolution scope to use for self binding.
        /// </summary>
        public DiResolutionScope SelfBindingDiResolutionScope { get; }

        /// <summary>
        /// An action to apply on an instance of bound type is activated.
        /// </summary>
        [CanBeNull]
        public Action<IDiContainer, object> OnImplementationObjectActivatedAction { get; set; }
    }

    /// <summary>
    /// A delegate that determines if a type should be self bound.
    /// </summary>
    /// <param name="type">Type to check. The type is not an interface and is not an abstract class.</param>
    /// <param name="publicConstructors">Non-empty list of  public constructors.</param>
    /// <returns></returns>
    public delegate (bool shouldBeSelfBound, SelfBindingRegistrationResult selfBindingRegistrationResult) TypeShouldBeSelfBoundDelegate([NotNull] Type type, IReadOnlyList<System.Reflection.ConstructorInfo> publicConstructors);
}
