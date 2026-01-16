The embedded resource IoC.Configuration.Schema.22B4BA50-3429-48FE-8059-B9C1F855A639.xsd is a copy of file with
similar name in folder IoC.Configuration.Content. 
These two files should be kept in sync.
The reason we maintain two copies of the same file is that IoC.Configuration.Schema.22B4BA50-3429-48FE-8059-B9C1F855A639.xsd
should be an embedded resource ("Build Action" property should be set to "Embedded resource)".

However, we also want this file to be included in package folder, so that the users of the package can use the schema for
configuration files in Visual Studio to use the code completion features that VS provides.

