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
using System.Xml;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class ServiceImplementationElementAbstr : ConfigurationFileElementAbstr, IServiceImplementationElement
    {
        #region  Constructors

        public ServiceImplementationElementAbstr([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent) : base(xmlElement, parent)
        {
        }

        #endregion

        #region IServiceImplementationElement Interface Implementation

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

#if DEBUG
            // Will enable this code and finish implementation in release mode when Autofac implementation for this feature is available.
            WhenInjectedIntoType = null;
            ConditionalInjectionType = ConditionalInjectionType.None;
#endif
        }

        IAssembly IServiceImplementationElement.Assembly => ValueTypeInfo.Assembly as IAssembly;

        public ConditionalInjectionType ConditionalInjectionType { get; private set; }

        public override bool Enabled => base.Enabled && (ValueTypeInfo.Assembly.Plugin?.Enabled ?? true);
        Type IServiceImplementationElement.ImplementationType => ValueTypeInfo.Type;

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            // If the implementation is disabled (because the assembly belongs to disabled to plugin),
            // and it is not under plugin setup element, lets show a warning that the implementation will be ignored.
            if (!Enabled && OwningPluginElement == null)
                MessagesHelper.LogElementDisabledWarning(this, ValueTypeInfo.Assembly, true);
        }

        public Type WhenInjectedIntoType { get; private set; }

        #endregion

        #region Current Type Interface

        public abstract DiResolutionScope ResolutionScope { get; }

        public abstract ITypeInfo ValueTypeInfo { get; }

        #endregion
    }
}