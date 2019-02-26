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
using System.Text;
using JetBrains.Annotations;

namespace IoC.Configuration.ConfigurationFile
{
    public class TypeParser : ITypeParser
    {
        #region Member Variables

        private readonly char ArraySymbolChar = '#';

        private static readonly string InvalidTypeNameErrorMessage;

        #endregion

        #region  Constructors

        static TypeParser()
        {
            var errorMessage = new StringBuilder();
            errorMessage.AppendLine("Type names can only contain letters a through z, capital letters A through Z, numbers, a period and an underscore. Generic type names follow the same rules, and can include also characters '[', ']', and ',' for type parameters.");
            errorMessage.AppendLine("Examples of valid type names are: IoC.Configuration._Class1, IoC.Configuration.IMyType[namespace1.IInterface1, namespace2.IInterface2, namespace2.Type2[namespace2.IInterface3]].");

            InvalidTypeNameErrorMessage = errorMessage.ToString();
        }

        #endregion

        #region ITypeParser Interface Implementation

        public ITypeData Parse([NotNull] string typeFullName)
        {
            var typesDataStack = new Stack<TypeData>();

            TypeData currTypeData = null;
            TypeData parenTypeData = null;

            var currentIndex = 0;
            var currChar = '\0';

            void tryHandleArraySymbol()
            {
                SkipWhiteSpaceChars(typeFullName, ref currentIndex);

                if (currentIndex == typeFullName.Length)
                    return;

                currChar = typeFullName[currentIndex];

                if (currChar == ArraySymbolChar)
                {
                    currTypeData.IsArray = true;
                    ++currentIndex;
                }
            }

            while (true)
            {
                SkipWhiteSpaceChars(typeFullName, ref currentIndex);

                if (currentIndex == typeFullName.Length)
                    break;

                currChar = '\0';

                currChar = typeFullName[currentIndex];

                var typeName = ParseNextTypeName(typeFullName, ref currentIndex);

                currTypeData = new TypeData(typeName);

                if (parenTypeData != null)
                    parenTypeData.AddGenericTypeParameter(currTypeData);

                if (currentIndex == typeFullName.Length)
                    break;

                tryHandleArraySymbol();

                SkipWhiteSpaceChars(typeFullName, ref currentIndex);

                if (currentIndex == typeFullName.Length)
                    break;

                currChar = typeFullName[currentIndex];

                if (currChar == ',')
                {
                    if (parenTypeData == null)
                        throw new ParseTypeException($"Invalid character {currChar}.{Environment.NewLine}{InvalidTypeNameErrorMessage}.", currentIndex);

                    ++currentIndex;
                }
                else if (currChar == '[')
                {
                    if (currTypeData.IsArray)
                        throw new ParseTypeException($"Invalid character {currChar}. Array symbol cannot be followed by '['.", currentIndex);

                    parenTypeData = currTypeData;
                    currTypeData = null;
                    typesDataStack.Push(parenTypeData);

                    ++currentIndex;
                }
                else if (currChar == ']')
                {
                    // Keep reading the closing brackets.
                    while (currChar == ']')
                    {
                        if (typesDataStack.Count == 0)
                            throw new ParseTypeException($"Invalid character {currChar}.{Environment.NewLine}{InvalidTypeNameErrorMessage}.", currentIndex);

                        currTypeData = typesDataStack.Pop();

                        if (typesDataStack.Count > 0)
                            parenTypeData = typesDataStack.Peek();
                        else
                            parenTypeData = null;

                        ++currentIndex;

                        tryHandleArraySymbol();

                        SkipWhiteSpaceChars(typeFullName, ref currentIndex);

                        if (currentIndex == typeFullName.Length)
                            break;

                        currChar = typeFullName[currentIndex];
                    }

                    if (currChar == ',')
                    {
                        if (parenTypeData == null)
                            throw new ParseTypeException($"Invalid character {currChar}.{Environment.NewLine}{InvalidTypeNameErrorMessage}.", currentIndex);

                        ++currentIndex;
                    }
                }
                else
                {
                    throw new ParseTypeException($"Invalid character '{currChar}'.{Environment.NewLine}{InvalidTypeNameErrorMessage}.", currentIndex);
                }
            }

            if (currTypeData == null || typesDataStack.Count > 0)
                throw new ParseTypeException($"Missing symbol ']'.{Environment.NewLine}{InvalidTypeNameErrorMessage}.", currentIndex);

            return currTypeData;
        }

        #endregion

        #region Member Functions

        private string ParseNextTypeName(string typeFullName, ref int currentIndex)
        {
            // Regex.IsMatch(typeAttributeValue, "^([a-zA-Z_]+[0-9]*)+(.[a-zA-Z_]+[0-9]*)*$"))

            var typeName = new StringBuilder();

            var startIndex = currentIndex;

            var currChar = '\0';
            var newSegmentStarted = true;

            for (; currentIndex < typeFullName.Length; currentIndex++)
            {
                currChar = typeFullName[currentIndex];

                if (char.IsWhiteSpace(currChar))
                    break;

                if (currChar == '[' || currChar == ']' || currChar == ',' || currChar == ArraySymbolChar)
                    break;

                if (char.IsNumber(currChar) || currChar == '.')
                {
                    if (newSegmentStarted)
                        throw new ParseTypeException($"Type name cannot start with a number, or a period sign.{Environment.NewLine}{InvalidTypeNameErrorMessage}.", currentIndex);

                    if (currChar == '.')
                        newSegmentStarted = true;
                }
                else if (currChar == '_' || char.IsLetter(currChar))
                {
                    newSegmentStarted = false;
                }
                else
                    throw new ParseTypeException($"Invalid character '{currChar}'.{Environment.NewLine}{InvalidTypeNameErrorMessage}.", currentIndex);

                typeName.Append(currChar);
            }

            if (currChar == '.')
            {
                throw new ParseTypeException($"Invalid character '{currChar}'. Dot should be followed by a letter or an underscore.",
                    currentIndex);
            }

            if (typeName.Length == 0)
            {
                throw new ParseTypeException($"Type name missing.{Environment.NewLine}{InvalidTypeNameErrorMessage}.", startIndex);
            }

            return typeName.ToString();
        }

        private void SkipWhiteSpaceChars(string typeFullName, ref int currentIndex)
        {
            while (currentIndex < typeFullName.Length)
            {
                var currChar = typeFullName[currentIndex];

                if (!char.IsWhiteSpace(currChar) || currentIndex == typeFullName.Length)
                    break;

                ++currentIndex;
            }
        }

        #endregion
    }
}