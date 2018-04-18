using System;
using System.Collections.Generic;
namespace DynamicImplementations_636595161441212446
{
public class IResourceAccessValidatorFactory_636595161448747916: TestPluginAssembly1.Interfaces.IResourceAccessValidatorFactory
{
private List<object[]> _selectorValues;
private IoC.Configuration.DiContainer.IDiContainer _diContainer;
public IResourceAccessValidatorFactory_636595161448747916(IoC.Configuration.DiContainer.IDiContainer diContainer, OROptimizer.Serializer.ITypeBasedSimpleSerializerAggregator serializerAggregator)
{
_diContainer = diContainer;
_selectorValues = new List<object[]>(2);
object deserializedValue;
{
var deserializedValues = new object[1];
_selectorValues.Add(deserializedValues);
if (!serializerAggregator.TryDeserialize(typeof(System.String), "public_pages", out deserializedValue))
 throw new Exception("Failed to convert 'public_pages' to 'System.String'.");
deserializedValues[0]=deserializedValue;
}
{
var deserializedValues = new object[1];
_selectorValues.Add(deserializedValues);
if (!serializerAggregator.TryDeserialize(typeof(System.String), "admin_pages", out deserializedValue))
 throw new Exception("Failed to convert 'admin_pages' to 'System.String'.");
deserializedValues[0]=deserializedValue;
}
}
public System.Collections.Generic.IEnumerable<TestPluginAssembly1.Interfaces.IResourceAccessValidator> GetValidators(System.String resourceName)
{
var returnedValues = new System.Collections.Generic.List<TestPluginAssembly1.Interfaces.IResourceAccessValidator>();
if ((resourceName != null && resourceName.Equals(_selectorValues[0][0])))
{
try
{
returnedValues.Add(_diContainer.Resolve<TestPluginAssembly1.Interfaces.ResourceAccessValidator1>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'TestPluginAssembly1.Interfaces.ResourceAccessValidator1'. Make sure to register the type in configuration file.", e);
throw;
}
return returnedValues;
}
if ((resourceName != null && resourceName.Equals(_selectorValues[1][0])))
{
try
{
returnedValues.Add(_diContainer.Resolve<TestPluginAssembly1.Interfaces.ResourceAccessValidator1>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'TestPluginAssembly1.Interfaces.ResourceAccessValidator1'. Make sure to register the type in configuration file.", e);
throw;
}
try
{
returnedValues.Add(_diContainer.Resolve<TestPluginAssembly1.Interfaces.ResourceAccessValidator2>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'TestPluginAssembly1.Interfaces.ResourceAccessValidator2'. Make sure to register the type in configuration file.", e);
throw;
}
return returnedValues;
}
try
{
returnedValues.Add(_diContainer.Resolve<TestPluginAssembly1.Interfaces.ResourceAccessValidator2>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'TestPluginAssembly1.Interfaces.ResourceAccessValidator2'. Make sure to register the type in configuration file.", e);
throw;
}
try
{
returnedValues.Add(_diContainer.Resolve<TestPluginAssembly1.Interfaces.ResourceAccessValidator1>());
}
catch(Exception e)
{
OROptimizer.Diagnostics.Log.LogHelper.Context.Log.Error("Could not resolve the type 'TestPluginAssembly1.Interfaces.ResourceAccessValidator1'. Make sure to register the type in configuration file.", e);
throw;
}
return returnedValues;
}
}
}
