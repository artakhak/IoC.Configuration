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
using System.Xml;
using JetBrains.Annotations;
using OROptimizer;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class ObjectInstanceElementAbstr<TInstantiatedType> : ConfigurationFileElementAbstr where TInstantiatedType : class
    {
        #region Member Variables

        [NotNull]
        private readonly IAssemblyLocator _assemblyLocator;

        private IAssembly _assemblySetting;

        [CanBeNull]
        private IParameters _parameters;

        #endregion

        #region  Constructors

        public ObjectInstanceElementAbstr([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                          [NotNull] IAssemblyLocator assemblyLocator) : base(xmlElement, parent)
        {
            _assemblyLocator = assemblyLocator;
        }

        #endregion

        #region Current Type Interface

        protected virtual bool ShouldBeEnabled => true;

        #endregion

        #region Member Functions

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IParameters)
            {
                _parameters = (IParameters) child;

                foreach (var parameter in _parameters.AllParameters)
                    if (parameter.ValueInstantiationType == ValueInstantiationType.ResolveFromDiContext)
                        throw new ConfigurationParseException(parameter, $"Injected parameters cannot be used in element '{ElementName}'", this);
            }
        }

        public override bool Enabled => base.Enabled && _assemblySetting.Enabled;

        public override void Initialize()
        {
            base.Initialize();
            _assemblySetting = Helpers.GetAssemblySettingByAssemblyAlias(this, this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Assembly));

            if (!Enabled && ShouldBeEnabled)
                throw new ConfigurationParseException(this, $"The element '{ElementName}' is disabled. Either make sure the assembly '{_assemblySetting}' is enabled, or delete this element.");
        }

        protected TInstantiatedType Instance { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            var typeFullName = this.GetAttributeValue<string>(ConfigurationFileAttributeNames.Type);

            if (Enabled)
            {
                var typeInAssembly = Helpers.GetTypeInAssembly(_assemblyLocator, this, _assemblySetting, typeFullName);

                var instance = GlobalsCoreAmbientContext.Context.CreateInstance(typeof(TInstantiatedType), typeInAssembly, _parameters == null ? new ParameterInfo[0] : _parameters.GetParameterValues(), out var errorMessage);

                if (instance == null)
                    throw new ConfigurationParseException(this, errorMessage);

                Instance = (TInstantiatedType)instance;
                
            }
        }

        #endregion
    }
}