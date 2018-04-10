using System;
using System.Collections.Generic;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.Tests
{
    public class PrimitiveTypeDefaultBindingsModule : ModuleAbstr
    {
        #region Member Variables

        [NotNull]
        private readonly Dictionary<Type, object> _typeToDefaultValueMap = new Dictionary<Type, object>();

        #endregion

        #region  Constructors

        public PrimitiveTypeDefaultBindingsModule(DateTime defaultDateTime, double defaultDouble,
                                                  short defaultInt16, int defaultInt32)
        {
            _typeToDefaultValueMap[typeof(DateTime)] = defaultDateTime;
            _typeToDefaultValueMap[typeof(double)] = defaultDouble;
            _typeToDefaultValueMap[typeof(short)] = defaultInt16;
            _typeToDefaultValueMap[typeof(int)] = defaultInt32;
        }

        #endregion

        #region Member Functions

        protected override void AddServiceRegistrations()
        {
            Bind<DateTime>().To(x => GetDefaultValue<DateTime>());
            Bind<double>().To(x => GetDefaultValue<double>());
            Bind<short>().To(x => GetDefaultValue<short>());
            Bind<int>().To(x => GetDefaultValue<int>());
        }

        private T GetDefaultValue<T>() where T : struct
        {
            if (_typeToDefaultValueMap.TryGetValue(typeof(T), out var defaultValueObject))
                return (T) defaultValueObject;

            return default(T);
        }

        #endregion
    }
}