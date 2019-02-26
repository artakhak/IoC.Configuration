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
using System.Reflection;

namespace IoC.Configuration.ConfigurationFile
{
    public class TypeMemberLookupHelper : ITypeMemberLookupHelper
    {
        #region ITypeMemberLookupHelper Interface Implementation

        public FieldInfo GetFieldInfoInClassOrParents(Type type, string fieldName, Type fieldType)
        {
            FieldInfo matchedFieldInfo = null;

            void processType(Type type2, ref bool stopProcessing2)
            {
                if (matchedFieldInfo != null)
                    return;

                foreach (var fieldInfo in type2.GetFields())
                {
                    if (fieldInfo.Name.Equals(fieldName, StringComparison.Ordinal) &&
                        fieldInfo.FieldType == fieldType && fieldInfo.IsPublic)
                    {
                        stopProcessing2 = true;
                        matchedFieldInfo = fieldInfo;
                        return;
                    }
                }
            }

            var stopProcessing = false;
            ProcessTypeImplementedInterfacesAndBaseTypes(type, processType, ref stopProcessing);
            return matchedFieldInfo;
        }
       
        public void ProcessTypeImplementedInterfacesAndBaseTypes(Type type, TypeProcessorDelegate typeProcessor, ref bool stopProcessing)
        {
            typeProcessor(type, ref stopProcessing);

            if (stopProcessing)
                return;

            if (!type.IsInterface)
            {
                var baseType = type.BaseType;

                if (baseType != null && !baseType.IsInterface)
                {
                    typeProcessor(baseType, ref stopProcessing);

                    if (stopProcessing)
                        return;
                }
            }

            foreach (var implementedInterface in type.GetInterfaces())
            {
                typeProcessor(implementedInterface, ref stopProcessing);

                if (stopProcessing)
                    return;
            }
        }

        #endregion
    }
}