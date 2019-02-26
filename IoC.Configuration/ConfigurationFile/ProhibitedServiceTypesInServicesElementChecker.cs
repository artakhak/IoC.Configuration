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
using IoC.Configuration.OnApplicationStart;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class ProhibitedServiceTypesInServicesElementChecker : IProhibitedServiceTypesInServicesElementChecker
    {
        #region Member Variables

        [NotNull]
        private readonly HashSet<Type> _prohibitedTypes;

        #endregion

        #region  Constructors

        public ProhibitedServiceTypesInServicesElementChecker([CanBeNull] [ItemNotNull] IEnumerable<Type> additionalProhibitedTypes = null)
        {
            if (additionalProhibitedTypes != null)
                _prohibitedTypes = new HashSet<Type>(additionalProhibitedTypes);
            else
                _prohibitedTypes = new HashSet<Type>();

            _prohibitedTypes.Add(typeof(IPlugin));
            _prohibitedTypes.Add(typeof(IStartupAction));
            _prohibitedTypes.Add(typeof(ISettingsRequestor));
        }

        #endregion

        #region IProhibitedServiceTypesInServicesElementChecker Interface Implementation

        public bool IsServiceTypeAllowed(Type serviceType)
        {
            return !_prohibitedTypes.Contains(serviceType);
        }

        #endregion
    }
}