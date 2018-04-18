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
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.Tests
{
    public class PrimitiveTypeDefaultBindingsModule : ModuleAbstr
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<Type, object> _typeToDefaultValueMap = new Dictionary<Type, object>();

        #endregion

        #region  Constructors

        public PrimitiveTypeDefaultBindingsModule(DateTime defaultDateTime, double defaultDouble,
                                                  short defaultInt16, int defaultInt32)
        {
            _typeToDefaultValueMap[typeof(DateTime)] = defaultDateTime;
            _typeToDefaultValueMap[typeof(double)] = defaultDouble;
            _typeToDefaultValueMap[typeof(short)] = defaultInt16;
            _typeToDefaultValueMap[typeof(int)] = defaultInt32;
        }

        #endregion

        #region Member Functions

        protected override void AddServiceRegistrations()
        {
            Bind<DateTime>().To(x => GetDefaultValue<DateTime>());
            Bind<double>().To(x => GetDefaultValue<double>());
            Bind<short>().To(x => GetDefaultValue<short>());
            Bind<int>().To(x => GetDefaultValue<int>());
        }

        private T GetDefaultValue<T>() where T : struct
        {
            if (_typeToDefaultValueMap.TryGetValue(typeof(T), out var defaultValueObject))
                return (T) defaultValueObject;

            return default(T);
        }

        #endregion
    }
}