using System;
using System.Collections.Generic;
using System.Reflection;
using IoC.Configuration.DynamicCode;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OROptimizer.DynamicCode;

namespace IoC.Configuration.Tests.ConfigurationFileLoadFailureTests
{
    public class TypesListFactoryTypeGeneratorMock : ITypesListFactoryTypeGenerator
    {
        #region Member Variables

        [NotNull]
        private readonly ITypesListFactoryTypeGenerator _typesListFactoryTypeGenerator;

        private readonly ValidationFailureMethod _validationFailureMethod;

        #endregion

        #region  Constructors

        public TypesListFactoryTypeGeneratorMock([NotNull] ITypesListFactoryTypeGenerator typesListFactoryTypeGenerator,
                                                 ValidationFailureMethod validationFailureMethod)
        {
            _typesListFactoryTypeGenerator = typesListFactoryTypeGenerator;
            _validationFailureMethod = validationFailureMethod;
        }

        #endregion

        #region ITypesListFactoryTypeGenerator Interface Implementation

        public IGeneratedTypeInfo GenerateType(IDynamicAssemblyBuilder dynamicAssemblyBuilder, Type interfaceToImplement, string dynamicImplementationsNamespace, IEnumerable<Type> returnedInstanceTypesForDefaultCase, IEnumerable<string[]> selectorParameterValues, IEnumerable<IEnumerable<Type>> returnedInstanceTypesForSelectorParameterValues)
        {
            if (_validationFailureMethod != ValidationFailureMethod.None)
                Assert.Fail("We should have failed before getting here.");

            return _typesListFactoryTypeGenerator.GenerateType(dynamicAssemblyBuilder, interfaceToImplement, dynamicImplementationsNamespace, returnedInstanceTypesForDefaultCase, selectorParameterValues, returnedInstanceTypesForSelectorParameterValues);
        }

        public bool ValidateImplementedInterface(Type interfaceToImplement, out MethodInfo implementedMethodInfo, out Type returnedItemsType, out string errorMessage)
        {
            if (_validationFailureMethod == ValidationFailureMethod.ValidateImplementedInterface)
            {
                implementedMethodInfo = null;
                returnedItemsType = null;
                errorMessage = $"This is a mock {nameof(ValidateImplementedInterface)} error: Validation of interface '{interfaceToImplement.FullName}' failed.";
                return false;
            }

            return _typesListFactoryTypeGenerator.ValidateImplementedInterface(interfaceToImplement, out implementedMethodInfo, out returnedItemsType, out errorMessage);
        }

        public bool ValidateParameterValues(MethodInfo implementedMethodInfo, IEnumerable<string> selectorParameterValues, out string errorMessage)
        {
            if (_validationFailureMethod == ValidationFailureMethod.ValidateParameterValues)
            {
                errorMessage = $"This is a mock {nameof(ValidateParameterValues)} error: Validation of parameters [{string.Join(',', selectorParameterValues)}] failed.";
                return false;
            }

            return _typesListFactoryTypeGenerator.ValidateParameterValues(implementedMethodInfo, selectorParameterValues, out errorMessage);
        }

        public bool ValidateReturnedType(Type specifiedReturnedType, Type returnedItemsType, out string errorMessage)
        {
            if (_validationFailureMethod == ValidationFailureMethod.ValidateReturnedType)
            {
                errorMessage = $"This is a mock {nameof(ValidateReturnedType)} error: Validation of return type '{specifiedReturnedType.FullName}' failed.";
                return false;
            }

            return _typesListFactoryTypeGenerator.ValidateReturnedType(specifiedReturnedType, returnedItemsType, out errorMessage);
        }

        #endregion

        #region Nested Types

        public enum ValidationFailureMethod
        {
            None,
            ValidateImplementedInterface,
            ValidateReturnedType,
            ValidateParameterValues
        }

        #endregion
    }
}