#region File Header

// Copyright © AWIN-Software, 2016

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
    }
}
