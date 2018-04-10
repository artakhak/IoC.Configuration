using System;
using System.Collections.Generic;
using DynamicallyLoadedAssembly1.Interfaces;
using DynamicallyLoadedAssembly2;
using IoC.Configuration.DiContainer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;

namespace DynamicImplementations_636583977886378414
{
    public class IActionValidatorFactory1_636583977911860567 : IActionValidatorFactory1
    {
        #region Member Variables

        private readonly IDiContainer _diContainer;
        private readonly List<object[]> _selectorValues;

        #endregion

        #region  Constructors

        public IActionValidatorFactory1_636583977911860567(IDiContainer diContainer, ITypeBasedSimpleSerializerAggregator serializerAggregator)
        {
            _diContainer = diContainer;
            _selectorValues = new List<object[]>(3);
            object deserializedValue;
            {
                var deserializedValues = new object[2];
                _selectorValues.Add(deserializedValues);
                if (!serializerAggregator.TryDeserialize(typeof(int), "1", out deserializedValue))
                    throw new Exception("Failed to convert '1' to 'System.Int32'.");
                deserializedValues[0] = deserializedValue;
                if (!serializerAggregator.TryDeserialize(typeof(string), "project1", out deserializedValue))
                    throw new Exception("Failed to convert 'project1' to 'System.String'.");
                deserializedValues[1] = deserializedValue;
            }
            {
                var deserializedValues = new object[2];
                _selectorValues.Add(deserializedValues);
                if (!serializerAggregator.TryDeserialize(typeof(int), "1", out deserializedValue))
                    throw new Exception("Failed to convert '1' to 'System.Int32'.");
                deserializedValues[0] = deserializedValue;
                if (!serializerAggregator.TryDeserialize(typeof(string), "project2", out deserializedValue))
                    throw new Exception("Failed to convert 'project2' to 'System.String'.");
                deserializedValues[1] = deserializedValue;
            }
            {
                var deserializedValues = new object[1];
                _selectorValues.Add(deserializedValues);
                if (!serializerAggregator.TryDeserialize(typeof(int), "2", out deserializedValue))
                    throw new Exception("Failed to convert '2' to 'System.Int32'.");
                deserializedValues[0] = deserializedValue;
            }
        }

        #endregion

        #region IActionValidatorFactory1 Interface Implementation

        public IEnumerable<IActionValidator> GetInstances(int param1, string param2)
        {
            var returnedValues = new List<IActionValidator>();
            if (param1.Equals(_selectorValues[0][0]) && param2 != null && param2.Equals(_selectorValues[0][1]))
            {
                try
                {
                    returnedValues.Add(_diContainer.Resolve<ActionValidator3>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator3'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                try
                {
                    returnedValues.Add(_diContainer.Resolve<ActionValidator1>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator1'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                return returnedValues;
            }

            if (param1.Equals(_selectorValues[1][0]) && param2 != null && param2.Equals(_selectorValues[1][1]))
            {
                try
                {
                    returnedValues.Add(_diContainer.Resolve<ActionValidator1>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator1'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                try
                {
                    returnedValues.Add(_diContainer.Resolve<ActionValidator2>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator2'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                return returnedValues;
            }

            if (param1.Equals(_selectorValues[2][0]))
            {
                try
                {
                    returnedValues.Add(_diContainer.Resolve<ActionValidator1>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator1'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                try
                {
                    returnedValues.Add(_diContainer.Resolve<ActionValidator2>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator2'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                try
                {
                    returnedValues.Add(_diContainer.Resolve<ActionValidator3>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator3'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                return returnedValues;
            }

            try
            {
                returnedValues.Add(_diContainer.Resolve<ActionValidator2>());
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator2'. Make sure to register the type in configuration file.", e);
                throw;
            }

            try
            {
                returnedValues.Add(_diContainer.Resolve<ActionValidator1>());
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator1'. Make sure to register the type in configuration file.", e);
                throw;
            }

            return returnedValues;
        }

        #endregion
    }
}