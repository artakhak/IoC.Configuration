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
    public class SettingValueInitializer : IValueInitializer
    {
        #region Member Variables

        [NotNull]
        private readonly ISettingElement _settingElement;

        #endregion

        #region  Constructors

        public SettingValueInitializer([NotNull] ISettingElement settingElement)
        {
            _settingElement = settingElement;
        }

        #endregion

        #region IValueInitializer Interface Implementation

        /// <summary>
        ///     Generates a code that returns an instance of a value of type specified by property <see cref="ValueType" />.
        /// </summary>
        /// <param name="dynamicAssemblyBuilder">The dynamic assembly builder.</param>
        /// <returns></returns>
        public string GenerateValueCSharp(IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            return _settingElement.GenerateValueCSharp(dynamicAssemblyBuilder);
        }

        public ITypeInfo ValueTypeInfo => _settingElement.ValueTypeInfo;

        #endregion
    }
}