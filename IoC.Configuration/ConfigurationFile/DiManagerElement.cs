// This software is part of the IoC.Configuration library
// Copyright � 2018 IoC.Configuration Contributors
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

using System.Xml;
using IoC.Configuration.DiContainer;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class DiManagerElement : ObjectInstanceElementAbstr<IDiManager>, IDiManagerElement
    {
        #region Member Variables

        [NotNull]
        private readonly IValidateDiManagerCompatibility _validateDiManagerCompatibility;

        #endregion

        #region  Constructors

        public DiManagerElement([NotNull] XmlElement xmlElement, [NotNull] IConfigurationFileElement parent,
                                [NotNull] ITypeHelper typeHelper,
                                [NotNull] ICreateInstanceFromTypeAndConstructorParameters createInstanceFromTypeAndConstructorParameters,
                                [NotNull] IValidateDiManagerCompatibility validateDiManagerCompatibility) :
            base(xmlElement, parent, typeHelper, createInstanceFromTypeAndConstructorParameters)
        {
            _validateDiManagerCompatibility = validateDiManagerCompatibility;
        }

        #endregion

        #region IDiManagerElement Interface Implementation

        public IDiManager DiManager => Instance;

        public override void Initialize()
        {
            base.Initialize();
            Name = this.GetNameAttributeValue();
        }

        public string Name { get; private set; }

        public override void ValidateAfterChildrenAdded()
        {
            base.ValidateAfterChildrenAdded();

            _validateDiManagerCompatibility.Validate(this);
        }

        #endregion
    }
}