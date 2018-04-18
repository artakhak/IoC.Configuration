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

namespace IoC.Configuration.Tests.ConfigurationFileLoadFailureTests
{
    /// <summary>
    ///     Sets the value of <see cref="IoCServiceFactoryAmbientContext.Context" /> temporarily t
    ///     to a <see cref="IoCServiceFactoryMock" /> object. The <see cref="Dispose" />() method switches
    ///     the context back to the default value.
    /// </summary>
    public class IoCServiceFactoryStaticContextMockSwicth : IDisposable
    {
        #region  Constructors

        /// <summary>
        ///     The constructor sets <see cref="IoCServiceFactoryAmbientContext.Context" />
        ///     to a <see cref="IoCServiceFactoryMock" /> object. The <see cref="Dispose" />() method switches
        ///     the context back to the default value.
        /// </summary>
        public IoCServiceFactoryStaticContextMockSwicth(TypesListFactoryTypeGeneratorMock.ValidationFailureMethod typesListFactoryValidationFailureMethod)
        {
            IoCServiceFactoryAmbientContext.Context = new IoCServiceFactoryMock(IoCServiceFactoryAmbientContext.Context, typesListFactoryValidationFailureMethod);
        }

        #endregion

        #region IDisposable Interface Implementation

        public void Dispose()
        {
            IoCServiceFactoryAmbientContext.SetDefaultContext();
        }

        #endregion
    }
}