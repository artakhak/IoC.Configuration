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
using OROptimizer.DynamicCode;

namespace IoC.Configuration.ConfigurationFile
{
    public class ClassMemberValueInitializer : IClassMemberValueInitializer
    {
        #region Member Variables

        [NotNull]
        private readonly IClassMemberValueInitializerHelper _classMemberValueInitializerHelper;

        [NotNull]
        private readonly IConfigurationFileElement _configurationFileElement;

        #endregion

        #region  Constructors

        public ClassMemberValueInitializer([NotNull] IConfigurationFileElement configurationFileElement,
                                           [NotNull] IClassMemberValueInitializerHelper classMemberValueInitializerHelper,
                                           [NotNull] ClassMemberData classMemberData)
        {
            _configurationFileElement = configurationFileElement;
            _classMemberValueInitializerHelper = classMemberValueInitializerHelper;
            ClassMemberData = classMemberData;
        }

        #endregion

        #region IClassMemberValueInitializer Interface Implementation

        [NotNull]
        public ClassMemberData ClassMemberData { get; }

        /// <summary>
        ///     Generates a code that returns an instance of a value of type specified by property <see cref="ValueType" />.
        /// </summary>
        /// <param name="dynamicAssemblyBuilder">The dynamic assembly builder.</param>
        /// <returns></returns>
        public string GenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            return _classMemberValueInitializerHelper.GenerateValueCSharp(ClassMemberData, dynamicAssemblyBuilder);
        }

        ITypeInfo ITypedItem.ValueTypeInfo => ClassMemberData.MemberTypeInfo;

        #endregion
    }
}