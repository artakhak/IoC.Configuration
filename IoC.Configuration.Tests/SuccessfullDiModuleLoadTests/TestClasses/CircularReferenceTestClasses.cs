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
namespace IoC.Configuration.Tests.SuccessfullDiModuleLoadTests.TestClasses
{
    public interface ICircularReferenceTestInterface1
    {
        #region Current Type Interface

        ICircularReferenceTestInterface2 Property1 { get; }

        #endregion
    }

    public interface ICircularReferenceTestInterface2
    {
        #region Current Type Interface

        ICircularReferenceTestInterface1 Property1 { get; }

        #endregion
    }

    public class CircularReferenceTestInterface1_Impl : ICircularReferenceTestInterface1
    {
        #region ICircularReferenceTestInterface1 Interface Implementation

        public ICircularReferenceTestInterface2 Property1 { get; set; }

        #endregion
    }

    public class CircularReferenceTestInterface2_Impl : ICircularReferenceTestInterface2
    {
        #region  Constructors

        public CircularReferenceTestInterface2_Impl(ICircularReferenceTestInterface1 param1)
        {
            Property1 = param1;
        }

        #endregion

        #region ICircularReferenceTestInterface2 Interface Implementation

        public ICircularReferenceTestInterface1 Property1 { get; }

        #endregion
    }
}