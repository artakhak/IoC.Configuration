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

using System.Xml;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    /// <summary>
    ///     Used for elements for constructed values. For example the value might be defined by specifying type and constructor
    ///     parameters
    ///     or a type, method name, and method parameters (using <see cref="ConfigurationFileElementNames.Parameters" />).
    ///     If no <see cref="ConfigurationFileElementNames.Parameters" /> child element exists, the type should have a default
    ///     constructor, or the
    ///     method should not use any parameters.
    /// </summary>
    public abstract class ConstructedValueElementBase : ValueInitializerElement
    {
        #region Member Variables

        [NotNull]
        private readonly IImplementedTypeValidator _implementedTypeValidator;

        #endregion

        #region  Constructors

        protected ConstructedValueElementBase([NotNull] XmlElement xmlElement, IConfigurationFileElement parent,
                                              [NotNull] ITypeHelper typeHelper,
                                              [NotNull] IImplementedTypeValidator implementedTypeValidator) :
            base(xmlElement, parent, typeHelper)
        {
            _implementedTypeValidator = implementedTypeValidator;
        }

        #endregion

        #region Member Functions

        public override void AddChild(IConfigurationFileElement child)
        {
            base.AddChild(child);

            if (child is IParameters parameters)
                Parameters = parameters;
        }

        public override void Initialize()
        {
            base.Initialize();

            _implementedTypeValidator.ValidateImplementationType(this, ValueTypeInfo);
        }

        public override bool IsResolvedFromDiContainer => false;


        [CanBeNull]
        [ItemNotNull]
        protected IParameters Parameters { get; private set; }

        #endregion
    }
}