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

namespace IoC.Configuration.Tests.SuccessfulDiModuleLoadTests.TestClasses
{
    public class ClassIdentifier
    {
        #region Member Variables

        private static readonly Dictionary<Type, List<object>> _instancesOfType = new Dictionary<Type, List<object>>();

        #endregion

        #region Member Functions

        public static int GetIndex(object instance)
        {
            if (!_instancesOfType.TryGetValue(instance.GetType(), out var instances))
                return -1;

            return instances.IndexOf(instance);
        }

        public static void Register(object instance)
        {
            if (!_instancesOfType.TryGetValue(instance.GetType(), out var instances))
            {
                instances = new List<object>();
                _instancesOfType[instance.GetType()] = instances;
            }

            instances.Add(instance);
        }

        #endregion
    }
}