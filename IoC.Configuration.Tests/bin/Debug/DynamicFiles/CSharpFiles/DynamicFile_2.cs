using System;
using System.Collections.Generic;
using DynamicallyLoadedAssembly1.Implementations;
using IoC.Configuration.DiContainer;
using OROptimizer.Diagnostics.Log;
using OROptimizer.Serializer;
using SharedServices.Interfaces;

namespace DynamicImplementations_636583977886378414
{
    public class ICleanupJobFactory_636583977911882917 : ICleanupJobFactory
    {
        #region Member Variables

        private readonly IDiContainer _diContainer;
        private readonly List<object[]> _selectorValues;

        #endregion

        #region  Constructors

        public ICleanupJobFactory_636583977911882917(IDiContainer diContainer, ITypeBasedSimpleSerializerAggregator serializerAggregator)
        {
            _diContainer = diContainer;
            _selectorValues = new List<object[]>(2);
            object deserializedValue;
            {
                var deserializedValues = new object[1];
                _selectorValues.Add(deserializedValues);
                if (!serializerAggregator.TryDeserialize(typeof(int), "1", out deserializedValue))
                    throw new Exception("Failed to convert '1' to 'System.Int32'.");
                deserializedValues[0] = deserializedValue;
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

        #region ICleanupJobFactory Interface Implementation

        public IEnumerable<ICleanupJob> GetCleanupJobs(int projectId)
        {
            var returnedValues = new List<ICleanupJob>();
            if (projectId.Equals(_selectorValues[0][0]))
            {
                try
                {
                    returnedValues.Add(_diContainer.Resolve<CleanupJob1>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly1.Implementations.CleanupJob1'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                try
                {
                    returnedValues.Add(_diContainer.Resolve<CleanupJob2>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly1.Implementations.CleanupJob2'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                return returnedValues;
            }

            if (projectId.Equals(_selectorValues[1][0]))
            {
                try
                {
                    returnedValues.Add(_diContainer.Resolve<CleanupJob2>());
                }
                catch (Exception e)
                {
                    LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly1.Implementations.CleanupJob2'. Make sure to register the type in configuration file.", e);
                    throw;
                }

                return returnedValues;
            }

            try
            {
                returnedValues.Add(_diContainer.Resolve<CleanupJob1>());
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly1.Implementations.CleanupJob1'. Make sure to register the type in configuration file.", e);
                throw;
            }

            try
            {
                returnedValues.Add(_diContainer.Resolve<CleanupJob3>());
            }
            catch (Exception e)
            {
                LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly1.Implementations.CleanupJob3'. Make sure to register the type in configuration file.", e);
                throw;
            }

            return returnedValues;
        }

        #endregion
    }
}