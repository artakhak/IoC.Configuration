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
using System.Text;
using System.Xml;
using JetBrains.Annotations;
using OROptimizer.Diagnostics.Log;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class ServiceElementBase : ConfigurationFileElementAbstr, IServiceElement, ICanHaveChildElementsThatUsePluginTypeInNonPluginSection
    {
        #region Member Variables

        [NotNull]
        [ItemNotNull]
        private readonly IList<IServiceImplementationElement> _serviceImplementations = new List<IServiceImplementationElement>();

        [NotNull]
        private readonly IValidateServiceUsageInPlugin _validateServiceUsageInPlugin;

        #endregion

        #region  Constructors

        protected ServiceElementBase([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                     [NotNull] IValidateServiceUsageInPlugin validateServiceUsageInPlugin) : base(xmlElement, parent)
        {
            _validateServiceUsageInPlugin = validateServiceUsageInPlugin;
        }

        #endregion

        #region IServiceElement Interface Implementation

        public IEnumerable<IServiceImplementationElement> Implementations => _serviceImplementations;

        public override void Initialize()
        {
            base.Initialize();

            ServiceTypeInfo = GetServiceTypeInfoOnInitialize();
            RegisterIfNotRegistered = this.GetAttributeValue<bool>(ConfigurationFileAttributeNames.RegisterIfNotRegistered);

            _validateServiceUsageInPlugin.Validate(this, ServiceTypeInfo);
        }

        public bool RegisterIfNotRegistered { get; private set; }
       
        public ITypeInfo ServiceTypeInfo { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            ValidateImplementationsAfterChildrenAdded();
        }

        #endregion

        #region Current Type Interface

        protected abstract ITypeInfo GetServiceTypeInfoOnInitialize();

        protected virtual void ValidateImplementationsAfterChildrenAdded()
        {
            foreach (var childElement in Children)
            {
                if (childElement is IServiceImplementationElement serviceImplementation)
                {
                    if (!ServiceTypeInfo.Type.IsAssignableFrom(serviceImplementation.ValueTypeInfo.Type))
                        throw new ConfigurationParseException(serviceImplementation, $"Implementation '{serviceImplementation.ValueTypeInfo.TypeCSharpFullName}' is not valid for service '{ServiceTypeInfo.TypeCSharpFullName}'.", this);

                    var disabledPluginTypeInfo = serviceImplementation.ValueTypeInfo.GetUniquePluginTypes().FirstOrDefault(x => !x.Assembly.Plugin.Enabled);

                    if (disabledPluginTypeInfo == null)
                    {
                        // Note, the same implementation type might appear multiple times.
                        // We do not want to prevent this, since each implementation might have different constructor parameters or injected properties.
                        _serviceImplementations.Add(serviceImplementation);
                    }
                    else if (GetPluginSetupElement() == null)
                    {
                        var warningString = new StringBuilder();
                        warningString.Append($"Implementation '{serviceImplementation.ValueTypeInfo.TypeCSharpFullName}' will be ignored since it");

                        if (serviceImplementation.ValueTypeInfo.GenericTypeParameters.Count > 0)
                            warningString.Append($" uses type '{disabledPluginTypeInfo.TypeCSharpFullName}' which");

                        warningString.Append($" is defined in assembly '{disabledPluginTypeInfo.Assembly.Alias}' that belongs to disabled plugin '{disabledPluginTypeInfo.Assembly.Plugin.Name}'.");

                        LogHelper.Context.Log.Warn(warningString.ToString());
                    }
                }
            }

            // We might have a situation, when the only implementations are types in plugins, and all
            // plugins are disabled. In this case _serviceImplementations will be an empty enumeration, and that would be OK.
            if (_serviceImplementations.Count == 0)
                LogHelper.Context.Log.WarnFormat(MessagesHelper.GetNoImplementationsForServiceMessage(ServiceTypeInfo.Type));
            else if (_serviceImplementations.Count > 1 && RegisterIfNotRegistered)
                throw new ConfigurationParseException(this, MessagesHelper.GetMultipleImplementationsWithRegisterIfNotRegisteredOptionMessage($"attribute '{ConfigurationFileAttributeNames.RegisterIfNotRegistered}'"));
        }

        #endregion
    }
}