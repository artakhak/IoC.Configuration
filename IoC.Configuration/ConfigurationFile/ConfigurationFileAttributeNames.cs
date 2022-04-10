// This software is part of the IoC.Configuration library
// Copyright © 2018 IoC.Configuration Contributors
// http://oroptimizer.com
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace IoC.Configuration.ConfigurationFile
{
    public static class ConfigurationFileAttributeNames
    {
        #region Member Variables

        public const string ActiveDiManagerName = "activeDiManagerName";
        public const string Alias = "alias";
        public const string Assembly = "assembly";
        public const string CollectionType = "collectionType";

        public const string DeclaringClass = "class";
        public const string DeclaringClassRef = "classRef";
        public const string DeclaringInterface = "declaringInterface";
        public const string Enabled = "enabled";
        public const string Interface = "interface";
        public const string InterfaceRef = "interfaceRef";

        public const string ItemType = "itemType";
        public const string ItemTypeAssembly = "itemTypeAssembly";
        public const string ItemTypeRef = "itemTypeRef";
        public const string MemberName = "memberName";
        public const string Name = "name";
        public const string OverrideDirectory = "overrideDirectory";
        public const string ParamName = "paramName";
        public const string Path = "path";
        public const string Plugin = "plugin";
        public const string PluginsDirPath = "pluginsDirPath";
        public const string RegisterIfNotRegistered = "registerIfNotRegistered";

        public const string ReturnType = "returnType";
        public const string ReturnTypeRef = "returnTypeRef";
        public const string ReuseValue = "reuseValue";
        public const string Scope = "scope";
        public const string SerializerAggregatorType = "serializerAggregatorType";
        public const string SerializerAggregatorTypeRef = "serializerAggregatorTypeRef";

        public const string SettingName = "settingName";
        public const string Type = "type";
        public const string TypeRef = "typeRef";
        public const string Value = "value";

        #endregion
    }
}