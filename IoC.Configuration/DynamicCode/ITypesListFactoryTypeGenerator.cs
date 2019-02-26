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
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.DynamicCode
{
    [Obsolete("Will be removed after 5/31/2019")]
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
        /// <param name="dynamicAssemblyBuilder">The dynamic assembly builder.</param>
        /// <param name="interfaceToImplement">The interface to implement.</param>
        /// <param name="dynamicImplementationsNamespace">Namespace to use for generated classes.</param>
        /// <param name="returnedInstanceTypesForDefaultCase">The returned instance types for default case.</param>
        /// <param name="selectorParameterValues">The selector parameter values.</param>
        /// <param name="returnedInstanceTypesForSelectorParameterValues">
        ///     The returned instance types for selector parameter
        ///     values.
        /// </param>
        /// <returns>Returns generated type information, such as class full name and C# file contents.</returns>
        /// ///
        /// <exception cref="System.Exception">Throws exception if the implementation generation fails.</exception>
        [NotNull]
        IGeneratedTypeInfo GenerateType([NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder, [NotNull] Type interfaceToImplement,
                                        [NotNull] string dynamicImplementationsNamespace,
                                        [NotNull] [ItemNotNull] IEnumerable<Type> returnedInstanceTypesForDefaultCase,
                                        [CanBeNull] [ItemNotNull] IEnumerable<string[]> selectorParameterValues,
                                        [CanBeNull] [ItemNotNull] IEnumerable<IEnumerable<Type>> returnedInstanceTypesForSelectorParameterValues);

        /// <summary>
        ///     Validates the implemented interface.
        /// </summary>
        /// <param name="interfaceToImplement">The interface to implement.</param>
        /// <param name="implementedMethodInfo">The implemented method information.</param>
        /// <param name="returnedItemsType">Type of the returned items.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        bool ValidateImplementedInterface([NotNull] Type interfaceToImplement,
                                          out MethodInfo implementedMethodInfo, out Type returnedItemsType,
                                          out string errorMessage);

        /// <summary>
        ///     Validates the parameter values.
        /// </summary>
        /// <param name="implementedMethodInfo">The implemented method information.</param>
        /// <param name="selectorParameterValues">The selector parameter values.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        bool ValidateParameterValues(MethodInfo implementedMethodInfo, IEnumerable<string> selectorParameterValues, out string errorMessage);

        /// <summary>
        ///     Validates the type of the returned.
        /// </summary>
        /// <param name="specifiedReturnedType">Specified type of the returned.</param>
        /// <param name="returnedItemsType">Type of the returned items.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        bool ValidateReturnedType([NotNull] Type specifiedReturnedType, [NotNull] Type returnedItemsType, out string errorMessage);

        #endregion
    }
}