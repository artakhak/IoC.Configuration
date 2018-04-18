using System;
using System.Collections.Generic;
namespace DynamicImplementations_636595161441212446
{
public class ICleanupJobFactory_636595161448697463: SharedServices.Interfaces.ICleanupJobFactory
{
private List<object[]> _selectorValues;
private IoC.Configuration.DiContainer.IDiContainer _diContainer;
public ICleanupJobFactory_636595161448697463(IoC.Configuration.DiContainer.IDiContainer diContainer, OROptimizer.Serializer.ITypeBasedSimpleSerializerAggregator serializerAggregator)
{
_diContainer = diContainer;
_selectorValues = new List<object[]>(2);
object deserializedValue;
{
var deserializedValues = new object[1];
_selectorValues.Add(deserializedValues);
if (!serializerAggregator.TryDeserialize(typeof(System.Int32), "1", out deserializedValue))
 throw new Exception("Failed to convert '1' to 'System.Int32'.");
deserializedValues[0]=deserializedValue;
}
{
var deserializedValues = new object[1];
_selectorValues.Add(deserializedValues);
if (!serializerAggregator.TryDeserialize(typeof(System.Int32), "2", out deserializedValue))
 throw new Exception("Failed to convert '2' to 'System.Int32'.");
deserializedValues[0]=deserializedValue;
}
}
public System.Collections.Generic.IEnumerable<SharedServices.Interfaces.ICleanupJob> GetCleanupJobs(System.Int32 projectId)
{
var returnedValues = new System.Collections.Generic.List<SharedServices.Interfaces.ICleanupJob>();
if ((projectId.Equals(_selectorValues[0][0])))
{
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly1.Implementations.CleanupJob1>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly1.Implementations.CleanupJob1'. Make sure to register the type in configuration file.", e);
throw;
}
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly1.Implementations.CleanupJob2>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly1.Implementations.CleanupJob2'. Make sure to register the type in configuration file.", e);
throw;
}
return returnedValues;
}
if ((projectId.Equals(_selectorValues[1][0])))
{
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly1.Implementations.CleanupJob2>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly1.Implementations.CleanupJob2'. Make sure to register the type in configuration file.", e);
throw;
}
return returnedValues;
}
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly1.Implementations.CleanupJob1>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly1.Implementations.CleanupJob1'. Make sure to register the type in configuration file.", e);
throw;
}
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly1.Implementations.CleanupJob3>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly1.Implementations.CleanupJob3'. Make sure to register the type in configuration file.", e);
throw;
}
return returnedValues;
}
}
}
