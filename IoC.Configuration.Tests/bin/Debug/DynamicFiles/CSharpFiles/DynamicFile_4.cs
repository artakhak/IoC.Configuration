using System;
using System.Collections.Generic;
using IoC.Configuration.DiContainer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;
using TestPluginAssembly1.Interfaces;

namespace DynamicImplementations_636583977886378414
{
    public class IResourceAccessValidatorFactory_636583977914007766 : IResourceAccessValidatorFactory
    {
        #region Member Variables

        private readonly IDiContainer _diContainer;
        private readonly List<object[]> _selectorValues;

        #endregion

        #region  Constructors

        public IResourceAccessValidatorFactory_636583977914007766(IDiContainer diContainer, ITypeBasedSimpleSerializerAggregator serializerAggregator)
        {
            _diContainer = diContainer;
            _selectorValues = new List<object[]>(2);
            object deserializedValue;
            {
                var deserializedValues = new object[1];
                _selectorValues.Add(deserializedValues);
                if (!serializerAggregator.TryDeserialize(typeof(string), "public_pages", out deserializedValue))
                    throw new Exception("Failed to convert 'public_pages' to 'System.String'.");
                deserializedValues[0] = deserializedValue;
            }
            {
                var deserializedValues = new object[1];
                _selectorValues.Add(deserializedValues);
                if (!serializerAggregator.TryDeserialize(typeof(string), "admin_pages", out deserializedValue))
                    throw new Exception("Failed to convert 'admin_pages' to 'System.String'.");
                deserializedValues[0] = deserializedValue;
            }
        }

        #endregion

        #region IResourceAccessValidatorFactory Interface Implementation

        public IEnumerable<IResourceAccessValidator> GetValidators(string resourceName)
        {
            var returnedValues = new List<IResourceAccessValidator>();
            if (resourceName != null && resourceName.Equals(_selectorValues[0][0]))
            {
                try
                {
                    returnedValues.Add(_diContainer.Resolve<ResourceAccessValidator1>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'TestPluginAssembly1.Interfaces.ResourceAccessValidator1'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                return returnedValues;
            }

            if (resourceName != null && resourceName.Equals(_selectorValues[1][0]))
            {
                try
                {
                    returnedValues.Add(_diContainer.Resolve<ResourceAccessValidator1>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'TestPluginAssembly1.Interfaces.ResourceAccessValidator1'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                try
                {
                    returnedValues.Add(_diContainer.Resolve<ResourceAccessValidator2>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'TestPluginAssembly1.Interfaces.ResourceAccessValidator2'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                return returnedValues;
            }

            try
            {
                returnedValues.Add(_diContainer.Resolve<ResourceAccessValidator2>());
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Error("Could not resolve the type 'TestPluginAssembly1.Interfaces.ResourceAccessValidator2'. Make sure to register the type in configuration file.", e);
                throw;
            }

            try
            {
                returnedValues.Add(_diContainer.Resolve<ResourceAccessValidator1>());
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Error("Could not resolve the type 'TestPluginAssembly1.Interfaces.ResourceAccessValidator1'. Make sure to register the type in configuration file.", e);
                throw;
            }

            return returnedValues;
        }

        #endregion
    }
}