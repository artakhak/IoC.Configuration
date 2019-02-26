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
using System.Linq;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IoC.Configuration.Tests
{
    public class ClassToTestServicesInjection<TService>
    {
        #region Member Variables

        private readonly List<TService> _implementations;

        #endregion

        #region  Constructors

        public ClassToTestServicesInjection([NotNull] IEnumerable<TService> services)
        {
            _implementations = new List<TService>(services);
        }

        #endregion

        #region Member Functions

        [NotNull]
        public IList<TService> Implementations => _implementations;

        public Type ServiceType => typeof(TService);

        public void ValidateDoesNotHaveImplementation(string implementationType)
        {
            Assert.IsFalse(_implementations.Any(x => x.GetType().FullName.Equals(implementationType, StringComparison.Ordinal)));
        }

        public void ValidateHasImplementation(Type implementationType)
        {
            Assert.IsTrue(_implementations.Where(x => x.GetType() == implementationType).ToList().Count == 1);
        }

        public void ValidateImplementationTypes(IEnumerable<string> implementationTypeNames)
        {
            var implementationTypeNamesList = new List<string>(implementationTypeNames);

            Assert.AreEqual(implementationTypeNamesList.Count, Implementations.Count);

            for (var i = 0; i < _implementations.Count; ++i)
                Assert.AreEqual(_implementations[i].GetType().FullName, implementationTypeNamesList[i], false);
        }

        #endregion
    }
}