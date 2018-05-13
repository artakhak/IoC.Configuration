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
namespace IoC.Configuration
{
    public static class HelpersIoC
    {
        #region Member Variables

        // TODO: Add an element Version to configuration file and use to convert to latest version. 
        // Will do in next version of IoC.Configuration that will introduce changes to configuration file.
        public const string ConfigurationFileVersion = "2F7CE7FF-CB22-40B0-9691-EAC689C03A36";
        public const string IoCConfigurationSchemaName = "IoC.Configuration.Schema." + ConfigurationFileVersion + ".xsd";
        public const string OnDiContainerReadyMethodName = "OnDiContainerReady";
        public const string SchemaFileFolderRelativeLocation = "IoC.Configuration.Content";
        #endregion
    }
}