using System;
using System.Collections.Generic;
namespace DynamicImplementations_636595161441212446
{
public class IActionValidatorFactory1_636595161448675911: DynamicallyLoadedAssembly2.IActionValidatorFactory1
{
private List<object[]> _selectorValues;
private IoC.Configuration.DiContainer.IDiContainer _diContainer;
public IActionValidatorFactory1_636595161448675911(IoC.Configuration.DiContainer.IDiContainer diContainer, OROptimizer.Serializer.ITypeBasedSimpleSerializerAggregator serializerAggregator)
{
_diContainer = diContainer;
_selectorValues = new List<object[]>(3);
object deserializedValue;
{
var deserializedValues = new object[2];
_selectorValues.Add(deserializedValues);
if (!serializerAggregator.TryDeserialize(typeof(System.Int32), "1", out deserializedValue))
 throw new Exception("Failed to convert '1' to 'System.Int32'.");
deserializedValues[0]=deserializedValue;
if (!serializerAggregator.TryDeserialize(typeof(System.String), "project1", out deserializedValue))
 throw new Exception("Failed to convert 'project1' to 'System.String'.");
deserializedValues[1]=deserializedValue;
}
{
var deserializedValues = new object[2];
_selectorValues.Add(deserializedValues);
if (!serializerAggregator.TryDeserialize(typeof(System.Int32), "1", out deserializedValue))
 throw new Exception("Failed to convert '1' to 'System.Int32'.");
deserializedValues[0]=deserializedValue;
if (!serializerAggregator.TryDeserialize(typeof(System.String), "project2", out deserializedValue))
 throw new Exception("Failed to convert 'project2' to 'System.String'.");
deserializedValues[1]=deserializedValue;
}
{
var deserializedValues = new object[1];
_selectorValues.Add(deserializedValues);
if (!serializerAggregator.TryDeserialize(typeof(System.Int32), "2", out deserializedValue))
 throw new Exception("Failed to convert '2' to 'System.Int32'.");
deserializedValues[0]=deserializedValue;
}
}
public System.Collections.Generic.IEnumerable<DynamicallyLoadedAssembly1.Interfaces.IActionValidator> GetInstances(System.Int32 param1, System.String param2)
{
var returnedValues = new System.Collections.Generic.List<DynamicallyLoadedAssembly1.Interfaces.IActionValidator>();
if ((param1.Equals(_selectorValues[0][0])) && (param2 != null && param2.Equals(_selectorValues[0][1])))
{
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly2.ActionValidator3>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator3'. Make sure to register the type in configuration file.", e);
throw;
}
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly2.ActionValidator1>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator1'. Make sure to register the type in configuration file.", e);
throw;
}
return returnedValues;
}
if ((param1.Equals(_selectorValues[1][0])) && (param2 != null && param2.Equals(_selectorValues[1][1])))
{
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly2.ActionValidator1>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator1'. Make sure to register the type in configuration file.", e);
throw;
}
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly2.ActionValidator2>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator2'. Make sure to register the type in configuration file.", e);
throw;
}
return returnedValues;
}
if ((param1.Equals(_selectorValues[2][0])))
{
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly2.ActionValidator1>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator1'. Make sure to register the type in configuration file.", e);
throw;
}
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly2.ActionValidator2>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator2'. Make sure to register the type in configuration file.", e);
throw;
}
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly2.ActionValidator3>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator3'. Make sure to register the type in configuration file.", e);
throw;
}
return returnedValues;
}
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly2.ActionValidator2>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator2'. Make sure to register the type in configuration file.", e);
throw;
}
try
{
returnedValues.Add(_diContainer.Resolve<DynamicallyLoadedAssembly2.ActionValidator1>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'DynamicallyLoadedAssembly2.ActionValidator1'. Make sure to register the type in configuration file.", e);
throw;
}
return returnedValues;
}
}
}
