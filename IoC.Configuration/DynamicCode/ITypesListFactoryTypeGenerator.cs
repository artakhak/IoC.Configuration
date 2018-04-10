using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.DynamicCode
{
    public interface ITypesListFactoryTypeGenerator
    {
        #region Current Type Interface

        /// <summary>
        ///     Generates a C# file for a type that is an implementation of interface specified in
        ///     <paramref name="interfaceToImplement" /> parameter.
        ///     Calls <see cref=" IDynamicAssemblyBuilder.AddCSharpFile" /> to add the generated C# file to an assembly being
        ///     built.
        ///     Interface <paramref name="interfaceToImplement" /> should have exactly one method that has arbitrary number
        ///     of parameters, and returns <see cref="IEnumerable&lt;T&gt;" />, where T is an interface.
        /// </summary>
        /// <param name="dynamicAssemblyBuilder"></param>
        /// <param name="interfaceToImplement"></param>
        /// <param name="dynamicImplementationsNamespace">Namespace to use for generated classes.</param>
        /// <param name="returnedInstanceTypesForDefaultCase"></param>
        /// <param name="selectorParameterValues"></param>
        /// <param name="returnedInstanceTypesForSelectorParameterValues"></param>
        /// <returns>Returns generated type information, such as class full name and C# file contents.</returns>
        /// <exception cref="System.Exception">Throws exception if the implementation generation fails.</exception>
        [NotNull]
        IGeneratedTypeInfo GenerateType([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder, [NotNull] Type interfaceToImplement,
                                        [NotNull] string dynamicImplementationsNamespace,
                                        [NotNull] [ItemNotNull] IEnumerable<Type> returnedInstanceTypesForDefaultCase,
                                        [CanBeNull] [ItemNotNull] IEnumerable<string[]> selectorParameterValues,
                                        [CanBeNull] [ItemNotNull] IEnumerable<IEnumerable<Type>> returnedInstanceTypesForSelectorParameterValues);

        bool ValidateImplementedInterface([NotNull] Type interfaceToImplement,
                                          out MethodInfo implementedMethodInfo, out Type returnedItemsType,
                                          out string errorMessage);

        bool ValidateParameterValues(MethodInfo implementedMethodInfo, IEnumerable<string> selectorParameterValues, out string errorMessage);

        bool ValidateReturnedType([NotNull] Type specifiedReturnedType, [NotNull] Type returnedItemsType, out string errorMessage);

        #endregion
    }
}