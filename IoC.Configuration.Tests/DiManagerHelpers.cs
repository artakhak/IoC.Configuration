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
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace IoC.Configuration.Tests
{
    public static class DiManagerHelpers
    {
        #region Member Variables

        [NotNull]
        private static readonly Dictionary<DiImplementationType, DiImplementationInfo> _diImplementationTypeToDiImplementationInfo = new Dictionary<DiImplementationType, DiImplementationInfo>();

        public static readonly string DynamicallyLoadedDllsFolder;

        public static readonly string ThirdPartyLibsFolder;

        #endregion

        #region  Constructors

        static DiManagerHelpers()
        {
            var testDllsFolderPath = Helpers.GetTestDllsFolderPath();

            ThirdPartyLibsFolder = Path.Combine(testDllsFolderPath, "ThirdPartyLibs");
            DynamicallyLoadedDllsFolder = Path.Combine(testDllsFolderPath, "DynamicallyLoadedDlls");

            ImplementationTypes = new List<DiImplementationType>(Enum.GetValues(typeof(DiImplementationType)).Cast<DiImplementationType>());

            _diImplementationTypeToDiImplementationInfo[DiImplementationType.Autofac] =
                new DiImplementationInfo(DiImplementationType.Autofac,
                    Path.Combine(testDllsFolderPath, "ContainerImplementations", "Autofac"),
                    Path.Combine(testDllsFolderPath, "ContainerImplementations", "Autofac", "IoC.Configuration.Autofac.dll"),
                    "IoC.Configuration.Autofac.AutofacDiManager",
                    "IoC.Configuration.Autofac.AutofacDiContainer");

            _diImplementationTypeToDiImplementationInfo[DiImplementationType.Ninject] =
                new DiImplementationInfo(DiImplementationType.Ninject,
                    Path.Combine(testDllsFolderPath, "ContainerImplementations", "Ninject"),
                    Path.Combine(testDllsFolderPath, "ContainerImplementations", "Ninject", "IoC.Configuration.Ninject.dll"),
                    "IoC.Configuration.Ninject.NinjectDiManager",
                    "IoC.Configuration.Ninject.NinjectDiContainer");
        }

        #endregion

        #region Member Functions

        public static DiImplementationInfo GetDiImplementationInfo(DiImplementationType diImplementationType)
        {
            return _diImplementationTypeToDiImplementationInfo[diImplementationType];
        }

        public static IEnumerable<DiImplementationInfo> ImplementationInfos => _diImplementationTypeToDiImplementationInfo.Values;

        public static IReadOnlyList<DiImplementationType> ImplementationTypes { get; }

        #endregion
    }
}