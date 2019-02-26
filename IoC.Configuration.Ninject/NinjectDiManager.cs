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
using IoC.Configuration.DiContainer;
using IoC.Configuration.DiContainer.BindingsForConfigFile;
using JetBrains.Annotations;
using Ninject;
using Ninject.Modules;
using OROptimizer;
using OROptimizer.DynamicCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IoC.Configuration.Ninject
{
    public class NinjectDiManager : IDiManager
    {
        #region IDiManager Interface Implementation

        public void BuildServiceProvider(IDiContainer diContainer, IEnumerable<object> modules)
        {
            var ninjectContainer = ConvertToNinjectContainer(diContainer);
            LoadModules(ninjectContainer.Kernel, modules);
        }

        public IDiContainer CreateDiContainer()
        {
            return new NinjectDiContainer();
        }

        public string DiContainerName => "Ninject";

        public string GenerateModuleClassCode(IDynamicAssemblyBuilder dynamicAssemblyBuilder, IAssemblyLocator assemblyLocator, string moduleClassNamespace, string moduleClassName,
                                              IEnumerable<BindingConfigurationForFile> moduleServiceConfigurationElements)
        {
            // Lets add this assembly, since we are referencing DiHelper in auto-generated code.
            dynamicAssemblyBuilder.AddReferencedAssembly(typeof(NinjectDiManager));
            dynamicAssemblyBuilder.AddReferencedAssembly(typeof(NinjectModule));

            var moduleClassContents = new StringBuilder();

            moduleClassContents.AppendLine("using System.Linq;");

            moduleClassContents.AppendLine($"namespace {moduleClassNamespace}");
            moduleClassContents.AppendLine("{");

            moduleClassContents.AppendLine($"public class {moduleClassName} : {typeof(NinjectModule).FullName}");
            moduleClassContents.AppendLine("{");

            DiManagerImplementationHelper.AddCodeForOnDiContainerReadyMethod(moduleClassContents);
            // Add Load() method
            moduleClassContents.AppendLine($"public override void Load()");
            moduleClassContents.AppendLine("{");

            foreach (var service in moduleServiceConfigurationElements)
                AddServiceBindings(moduleClassContents, service, dynamicAssemblyBuilder);

            moduleClassContents.AppendLine("}");
            // End Load() method

            moduleClassContents.AppendLine("}");
            moduleClassContents.AppendLine("}");
            return moduleClassContents.ToString();
        }

        public object GenerateNativeModule(IDiModule module)
        {
            return new NinjectModuleWrapper(module);
        }

        public object GetRequiredBindingsModule()
        {
            // Return NinjectModule module to set some required bindings.
            return null;
        }

        public Type ModuleType => typeof(NinjectModule);

        public void StartServiceProvider(IDiContainer diContainer)
        {
            // Do nothing. Already started.
        }

        #endregion

        #region Member Functions

        private void AddServiceBindings([NotNull] StringBuilder moduleClassContents, [NotNull] BindingConfigurationForFile serviceElement, [NotNull] IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            if (serviceElement.RegisterIfNotRegistered)
            {
                moduleClassContents.AppendLine($"if (!Kernel.GetBindings(typeof({serviceElement.ServiceType.GetTypeNameInCSharpClass()})).Any())");
                moduleClassContents.AppendLine("{");
            }

            foreach (var serviceImplementation in serviceElement.Implementations)
            {
                moduleClassContents.Append($"Bind<{serviceElement.ServiceType.GetTypeNameInCSharpClass()}>()");

                ITypeBasedBindingImplementationConfigurationForFile typeBasedBindingImplementationConfigurationForFile = null;
                if (serviceImplementation is ITypeBasedBindingImplementationConfigurationForFile)
                {
                    typeBasedBindingImplementationConfigurationForFile = (ITypeBasedBindingImplementationConfigurationForFile)serviceImplementation;

                    if (typeBasedBindingImplementationConfigurationForFile.Parameters == null)
                    {
                        if (serviceElement.IsSelfBoundService)
                            moduleClassContents.Append(".ToSelf()");
                        else
                            moduleClassContents.Append($".To<{typeBasedBindingImplementationConfigurationForFile.ImplementationType.GetTypeNameInCSharpClass()}>()");
                    }
                    else
                    {
                        moduleClassContents.Append($".ToMethod(context => new {typeBasedBindingImplementationConfigurationForFile.ImplementationType.GetTypeNameInCSharpClass()}(");

                        for (var i = 0; i < typeBasedBindingImplementationConfigurationForFile.Parameters.Length; ++i)
                        {
                            if (i > 0)
                                moduleClassContents.Append(", ");

                            var parameter = typeBasedBindingImplementationConfigurationForFile.Parameters[i];

                            moduleClassContents.Append(parameter.GenerateValueCSharp(
                                //(serviceType) => $"({serviceType.FullName})context.Kernel.GetService(typeof({serviceType.FullName}))", 
                                dynamicAssemblyBuilder));
                        }

                        moduleClassContents.Append("))");
                    }

                }
                else if (serviceImplementation is ValueBasedBindingImplementationConfigurationForFile valueBasedBindingImplementationConfigurationForFile)
                {
                    moduleClassContents.Append($".ToMethod(context => {valueBasedBindingImplementationConfigurationForFile.ValueInitializer.GenerateValueCSharp(dynamicAssemblyBuilder)})");
                }
                else if (serviceImplementation is IServiceToProxyBindingImplementationConfigurationForFile serviceToProxyBindingImplementationConfigurationForFile)
                {
                    moduleClassContents.AppendFormat(".ToMethod(context => ({0})context.Kernel.GetService(typeof({0})))",
                        serviceImplementation.ImplementationType.GetTypeNameInCSharpClass());
                }

                // Add WhenInjected code as in example below
                switch (serviceImplementation.ConditionalInjectionType)
                {
                    case ConditionalInjectionType.None:
                        // No conditional injection.
                        break;
                    case ConditionalInjectionType.WhenInjectedInto:
                        moduleClassContents.Append($".WhenInjectedInto<{serviceImplementation.WhenInjectedIntoType.GetTypeNameInCSharpClass()}>()");
                        break;
                    case ConditionalInjectionType.WhenInjectedExactlyInto:
                        moduleClassContents.Append($".WhenInjectedExactlyInto<{serviceImplementation.WhenInjectedIntoType.GetTypeNameInCSharpClass()}>()");
                        break;
                    default:
                        throw new UnsupportedEnumValueException(serviceImplementation.ConditionalInjectionType);
                }

                // Add resolution
                moduleClassContents.Append(".");
                switch (serviceImplementation.ResolutionScope)
                {
                    case DiResolutionScope.Singleton:
                        moduleClassContents.Append("InSingletonScope()");
                        break;
                    case DiResolutionScope.Transient:
                        moduleClassContents.Append("InTransientScope()");
                        break;
                    case DiResolutionScope.ScopeLifetime:
                        moduleClassContents.Append($"InScope(context => _diContainer.CurrentLifeTimeScope)");
                        break;
                    // Thread scope is not supported by Autofac, and therefore not used in Ninject as well.
                    //case DiResolutionScope.Thread:
                    //    moduleClassContents.Append("InThreadScope()");
                    //    break;
                    default:
                        throw new UnsupportedResolutionScopeException(serviceImplementation);
                }

                // Add injected properties
                if (typeBasedBindingImplementationConfigurationForFile != null)
                {
                    if (typeBasedBindingImplementationConfigurationForFile.InjectedProperties?.Any() ?? false)
                    {
                        moduleClassContents.AppendLine($".OnActivation<{typeBasedBindingImplementationConfigurationForFile.ImplementationType.GetTypeNameInCSharpClass()}>(activatedObject =>");
                        moduleClassContents.AppendLine("{");

                        foreach (var injectedProperty in typeBasedBindingImplementationConfigurationForFile.InjectedProperties)
                        {
                            moduleClassContents.Append($"activatedObject.{injectedProperty.Name}=");

                            moduleClassContents.Append(injectedProperty.GenerateValueCSharp(//(serviceType) => $"_diContainer.Resolve<{serviceType.FullName}>()",
                                dynamicAssemblyBuilder));

                            moduleClassContents.Append(";");

                            moduleClassContents.AppendLine();
                        }

                        moduleClassContents.Append("})");
                    }
                }

                moduleClassContents.AppendLine(";");
            }

            if (serviceElement.RegisterIfNotRegistered)
                moduleClassContents.AppendLine("}");
        }

        private NinjectDiContainer ConvertToNinjectContainer([NotNull] IDiContainer diContainer)
        {
            var ninjectDiContainer = diContainer as NinjectDiContainer;

            if (ninjectDiContainer == null)
                throw new ArgumentException($"Invalid value of parameter '{nameof(diContainer)}' in '{GetType().FullName}.{nameof(BuildServiceProvider)}(...)'. Expected an object of type '{typeof(NinjectDiContainer).FullName}'. Actual object type is {diContainer.GetType().FullName}.");

            return ninjectDiContainer;
        }

        private void LoadModules([NotNull] IKernel kernel, [NotNull] [ItemNotNull] IEnumerable<object> modules)
        {
            var ninjectModulesList = new List<INinjectModule>();

            foreach (var moduleObject in modules)
            {
                var ninjectModule = moduleObject as INinjectModule;
                if (ninjectModule == null)
                    throw new Exception($"Invalid type of module object: '{moduleObject.GetType().FullName}'. Expected an object of type '{typeof(INinjectModule)}'.");

                ninjectModulesList.Add(ninjectModule);
            }

            if (ninjectModulesList.Count > 0)
                kernel.Load(ninjectModulesList);
        }

        #endregion
    }
}