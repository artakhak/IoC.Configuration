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

namespace IoC.Configuration.ConfigurationFile
{
    /// <inheritdoc />
    public class ValidateDiManagerCompatibility : IValidateDiManagerCompatibility
    {
        #region IValidateDiManagerCompatibility Interface Implementation

        /// <inheritdoc />
        public void Validate(IDiManagerElement diManagerElement)
        {
            var diManagerAssembly = diManagerElement.DiManager.GetType().Assembly;

            var diManagerAssemblyVersion = diManagerAssembly.GetName().Version;
            Version minSupportedVersion = null;

            var diManagerTypeName = diManagerElement.DiManager.GetType().FullName;

            switch (diManagerTypeName)
            {
                case "IoC.Configuration.Autofac.AutofacDiManager":
                    minSupportedVersion = new Version(2, 0, 0, 0);
                    break;
                case "IoC.Configuration.Ninject.NinjectDiManager":
                    minSupportedVersion = new Version(2, 0, 0, 0);
                    break;
            }

            if (minSupportedVersion != null && diManagerAssemblyVersion < minSupportedVersion)
                throw new ConfigurationParseException(diManagerElement,
                    string.Format("'{0}, {1}' is not compatible with '{2}, {3}'. Minimum compatible version for '{2}, {3}' is '{0}, {4}'. Please get a newer version of '{0}' from 'https://www.nuget.org'.",
                        diManagerAssembly.GetName().Name, diManagerAssemblyVersion,
                        GetType().Assembly.GetName().Name, GetType().Assembly.GetName().Version,
                        minSupportedVersion));
        }

        #endregion
    }
}