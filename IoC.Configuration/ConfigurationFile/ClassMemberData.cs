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

using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IoC.Configuration.ConfigurationFile
{
    public class ClassMemberData
    {
        #region  Constructors

        public ClassMemberData([NotNull] ITypeInfo classInfo, [NotNull] ITypeInfo memberTypeInfo, [NotNull] MemberInfo classMemberInfo,
                               [NotNull, ItemNotNull] IEnumerable<IParameter> parameters,
                               bool isInjectedClassMember, ClassMemberCategory classMemberCategory)
        {
            ClassInfo = classInfo;
            MemberTypeInfo = memberTypeInfo;
            ClassMemberInfo = classMemberInfo;
            Parameters = parameters.ToList();
            IsInjectedClassMember = isInjectedClassMember;
            ClassMemberCategory = classMemberCategory;
        }

        #endregion

        #region Member Functions

        [NotNull]
        public ITypeInfo ClassInfo { get; }

        public ClassMemberCategory ClassMemberCategory { get; }

        [NotNull]
        public MemberInfo ClassMemberInfo { get; }

        public bool IsInjectedClassMember { get; }

        [NotNull]
        public ITypeInfo MemberTypeInfo { get; }

        [NotNull, ItemNotNull]
        public IReadOnlyList<IParameter> Parameters { get; }

        #endregion
    }
}