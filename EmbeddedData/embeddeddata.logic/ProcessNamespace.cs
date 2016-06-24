#region File Header

// The MIT License (MIT)
// 
// Copyright (c) 2016 Stefan Stolz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#region using directives

using System;

#endregion

namespace embeddeddata.logic
{
    public class ProcessNamespace
    {
        public ProcessNamespaceResult Execute(string targetNamespace)
        {
            var result = new ProcessNamespaceResult.Builder();

            var indexOfTestData = targetNamespace.IndexOf("testdata", StringComparison.OrdinalIgnoreCase);

            if (indexOfTestData >= 0)
            {
                result.HasTestData = true;
                result.Namespace = GetBaseNamespace(targetNamespace);
                result.SubNamespace = GetSubNamspace(targetNamespace);
            }
            else
            {
                result.Namespace = targetNamespace;
                result.HasTestData = false;
                result.SubNamespace = string.Empty;
            }

            return result.Build();
        }

        internal static string GetBaseNamespace(string targetNamespace)
        {
            var indexOfTestData = targetNamespace.IndexOf("testdata", StringComparison.OrdinalIgnoreCase);

            if (indexOfTestData >= 0)
            {
                var testDataValue = targetNamespace.Substring(indexOfTestData, "testdata".Length);

                var baseValue = targetNamespace.Substring(0, indexOfTestData);

                return baseValue + testDataValue;
            }
            else
            {
                throw new ArgumentException("Namespace doen't contain TestData", nameof(targetNamespace));
            }
        }

        internal static string GetSubNamspace(string targetNamespace)
        {
            var indexOfTestData = targetNamespace.IndexOf("testdata", StringComparison.OrdinalIgnoreCase);

            if (indexOfTestData >= 0)
            {
                var startIndex = indexOfTestData + "testdata".Length + 1;

                if (startIndex < targetNamespace.Length)
                {
                    return targetNamespace.Substring(startIndex);
                }

                return string.Empty;
            }
            else
            {
                throw new ArgumentException("Namespace doen't contain TestData", nameof(targetNamespace));
            }
        }
    }
}
