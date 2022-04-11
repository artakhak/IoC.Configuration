// Copyright (c) IoC.Configuration Project. All rights reserved.
// Licensed under the MIT License. See LICENSE in the solution root for license information.

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
