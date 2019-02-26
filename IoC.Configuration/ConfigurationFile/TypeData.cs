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
using System.Collections.Generic;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class TypeData : ITypeData
    {
        #region Member Variables

        private readonly List<ITypeData> _genericTypeParameters;

        #endregion

        #region  Constructors

        public TypeData([NotNull] string typeFullNameWithoutGenericParameters)
        {
            TypeFullNameWithoutGenericParameters = typeFullNameWithoutGenericParameters;
            _genericTypeParameters = new List<ITypeData>();
        }

        #endregion

        #region ITypeData Interface Implementation

        // For now this is always null since we do not have this in type full name
        public string AssemblyAlias { get; }

        public IReadOnlyList<ITypeData> GenericTypeParameters => _genericTypeParameters;

        public bool IsArray { get; set; }
        public string TypeFullNameWithoutGenericParameters { get; }

        #endregion

        #region Member Functions

        public void AddGenericTypeParameter(ITypeData typeData)
        {
            _genericTypeParameters.Add(typeData);
        }


        public override bool Equals(object comparedObject)
        {
            if (comparedObject == null || !(comparedObject is ITypeData comparedTypeData))
                return false;

            if (!TypeFullNameWithoutGenericParameters.Equals(comparedTypeData.TypeFullNameWithoutGenericParameters, StringComparison.Ordinal))
                return false;

            if (string.Compare(AssemblyAlias, comparedTypeData.AssemblyAlias, StringComparison.Ordinal) != 0)
                return false;

            if (_genericTypeParameters.Count != comparedTypeData.GenericTypeParameters.Count)
                return false;

            for (var i = 0; i < _genericTypeParameters.Count; ++i)
            {
                if (!_genericTypeParameters[i].Equals(comparedTypeData.GenericTypeParameters[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            // Better hash code would be to generate the full type name with generic parameters in a constructor, and get the hash
            // code of full type name.
            // However, the class has AddGenericTypeParameter() method, which might change the hash code after the hash code is generated
            // TODO: come back to this later if necessary (not a high priority).
            return TypeFullNameWithoutGenericParameters.GetHashCode();
        }

        public int IndexInTypeFullName { get; }

        #endregion
    }
}