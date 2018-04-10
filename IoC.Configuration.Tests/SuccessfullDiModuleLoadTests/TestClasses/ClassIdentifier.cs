using System;
using System.Collections.Generic;

namespace IoC.Configuration.Tests.SuccessfullDiModuleLoadTests.TestClasses
{
    public class ClassIdentifier
    {
        #region Member Variables

        private static readonly Dictionary<Type, List<object>> _instancesOfType = new Dictionary<Type, List<object>>();

        #endregion

        #region Member Functions

        public static int GetIndex(object instance)
        {
            if (!_instancesOfType.TryGetValue(instance.GetType(), out var instances))
                return -1;

            return instances.IndexOf(instance);
        }

        public static void Register(object instance)
        {
            if (!_instancesOfType.TryGetValue(instance.GetType(), out var instances))
            {
                instances = new List<object>();
                _instancesOfType[instance.GetType()] = instances;
            }

            instances.Add(instance);
        }

        #endregion
    }
}