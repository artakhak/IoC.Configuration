The embedded resource IoC.Configuration.Schema.7579ADB2-0FBD-4210-A8CA-EE4B4646DB3F.xsd is a copy of file with 
similar name in folder IoC.Configuration.Content. 
These two files should be kept in sync.
The reason we maintain two copies of the same file is that IoC.Configuration.Schema.7579ADB2-0FBD-4210-A8CA-EE4B4646DB3F.xsd 
should be an embedded resource ("Build Action" property should be set to "Embedded resource)".

However, we also want this file to be included in package folder, so that the users of the package can use the schema for
configuration files in Visual Studio to use the code completion features that VS provides.

