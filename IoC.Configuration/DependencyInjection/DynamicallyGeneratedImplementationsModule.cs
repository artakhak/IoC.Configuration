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
using OROptimizer;

namespace IoC.Configuration.DependencyInjection
{
    public class DynamicallyGeneratedImplementationsModule : ModuleAbstr
    {
        #region Member Variables

        [NotNull]
        private readonly string _dynamicallyGeneratedAssemblyFilePath;

        [NotNull]
        [ItemNotNull]
        private readonly IEnumerable<InterfaceImplementationInfo> _interfaceImplementationsInfo;

        #endregion

        #region  Constructors

        public DynamicallyGeneratedImplementationsModule([NotNull] [ItemNotNull] IEnumerable<InterfaceImplementationInfo> interfaceImplementationsInfo, [NotNull] string dynamicallyGeneratedAssemblyFilePath)
        {
            _interfaceImplementationsInfo = interfaceImplementationsInfo;
            _dynamicallyGeneratedAssemblyFilePath = dynamicallyGeneratedAssemblyFilePath;
        }

        #endregion

        #region Member Functions

        protected override void AddServiceRegistrations()
        {
            var assembly = GlobalsCoreAmbientContext.Context.LoadAssembly(_dynamicallyGeneratedAssemblyFilePath);

            foreach (var interfaceImplementationInfo in _interfaceImplementationsInfo)
            {
                var implementationType = assembly.GetType(interfaceImplementationInfo.ImplementingClassName);
                Bind(interfaceImplementationInfo.InterfaceType).To(implementationType).SetResolutionScope(DiResolutionScope.Singleton);
            }
        }

        #endregion

        #region Nested Types

        public class InterfaceImplementationInfo
        {
            #region  Constructors

            public InterfaceImplementationInfo([NotNull] Type interfaceType, [NotNull] string implementingClassName)
            {
                InterfaceType = interfaceType;
                ImplementingClassName = implementingClassName;
            }

            #endregion

            #region Member Functions

            [NotNull]
            public string ImplementingClassName { get; }

            [NotNull]
            public Type InterfaceType { get; }

            #endregion
        }

        #endregion
    }
}