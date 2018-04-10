using System;
using IoC.Configuration.ConfigurationFile;
using JetBrains.Annotations;

namespace IoC.Configuration.DiContainer.BindingsForConfigFile
{
    public abstract class NamedValue : INamedValue
    {
        #region Member Variables

        [NotNull]
        private readonly INamedValueElement _namedValueElement;

        #endregion

        #region  Constructors

        protected NamedValue([NotNull] INamedValueElement namedValueElement)
        {
            _namedValueElement = namedValueElement;
        }

        #endregion

        #region INamedValue Interface Implementation

        public string Name => _namedValueElement.Name;
        public string ValueAsString => _namedValueElement.ValueAsString;
        public ValueInstantiationType ValueInstantiationType => _namedValueElement.ValueInstantiationType;

        public Type ValueType => _namedValueElement.ValueType;

        #endregion
    }
}