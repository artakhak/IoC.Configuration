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
using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    [Obsolete("Will be removed after 5/31/2019")]
    public class TypeFactoryReturnedType : ConfigurationFileElementAbstr, ITypeFactoryReturnedType
    {
        #region Member Variables

        private ITypeInfo _returnedTypeInfo;

        [NotNull]
        private readonly ITypeHelper _typeHelper;

        #endregion

        #region  Constructors

        public TypeFactoryReturnedType([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                       [NotNull] ITypeHelper typeHelper) : base(xmlElement, parent)
        {
            _typeHelper = typeHelper;
        }

        #endregion

        #region ITypeFactoryReturnedType Interface Implementation

        public override bool Enabled => base.Enabled && ((_returnedTypeInfo.Assembly as IAssembly)?.Enabled ?? true);

        public override void Initialize()
        {
            base.Initialize();

            _returnedTypeInfo = _typeHelper.GetTypeInfo(this, ConfigurationFileAttributeNames.Type, ConfigurationFileAttributeNames.Assembly, ConfigurationFileAttributeNames.TypeRef);

            ReturnedType = _returnedTypeInfo.Type;

            if (!Enabled)
                MessagesHelper.LogElementDisabledWarning(this, _returnedTypeInfo.Assembly, true);
        }

        public Type ReturnedType { get; private set; }

        #endregion
    }
}