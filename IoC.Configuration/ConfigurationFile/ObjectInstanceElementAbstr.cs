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

using System.Linq;
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public abstract class ObjectInstanceElementAbstr<TInstantiatedType> : ConfigurationFileElementAbstr where TInstantiatedType : class
    {
        #region Member Variables

        [NotNull]
        private readonly ICreateInstanceFromTypeAndConstructorParameters _createInstanceFromTypeAndConstructorParameters;

        [CanBeNull]
        private IParameters _parameters;

        [NotNull]
        private readonly ITypeHelper _typeHelper;

        #endregion

        #region  Constructors

        public ObjectInstanceElementAbstr([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                          [NotNull] ITypeHelper typeHelper,
                                          [NotNull] ICreateInstanceFromTypeAndConstructorParameters createInstanceFromTypeAndConstructorParameters) : base(xmlElement, parent)
        {
            _typeHelper = typeHelper;
            _createInstanceFromTypeAndConstructorParameters = createInstanceFromTypeAndConstructorParameters;
        }

        #endregion

        #region Member Functions

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IParameters)
            {
                _parameters = (IParameters) child;

                foreach (var parameter in _parameters.AllParameters)
                    if (parameter.IsResolvedFromDiContainer)
                        throw new ConfigurationParseException(parameter, $"Injected parameters cannot be used in element '{ElementName}'", this);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            ValueTypeInfo = _typeHelper.GetTypeInfo(this, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly, ConfigurationFileAttributeNames.TypeRef);

            if (OwningPluginElement == null)
            {
                var disabledPluginTypeInfo = ValueTypeInfo.GetUniquePluginTypes().FirstOrDefault(x => !x.Assembly.Plugin.Enabled);

                if (disabledPluginTypeInfo != null)
                    throw new DisabledPluginTypeUsedConfigurationParseException(this, ValueTypeInfo, disabledPluginTypeInfo);
            }
        }

        protected TInstantiatedType Instance { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            Instance = (TInstantiatedType) _createInstanceFromTypeAndConstructorParameters.CreateInstance(this, typeof(TInstantiatedType), ValueTypeInfo.Type, _parameters?.AllParameters ?? new IParameterElement[0]);
        }

        public ITypeInfo ValueTypeInfo { get; private set; }

        #endregion
    }
}