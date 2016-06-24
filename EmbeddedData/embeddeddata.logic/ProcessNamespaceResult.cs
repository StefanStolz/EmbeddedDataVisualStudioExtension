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
    public class ProcessNamespaceResult
    {
        private ProcessNamespaceResult(Builder builder)
        {
            this.Namespace = builder.Namespace;
            this.SubNamespace = builder.SubNamespace;
            this.HasTestData = builder.HasTestData;
        }

        public bool HasTestData { get; }
        public string Namespace { get; }
        public string SubNamespace { get; }

        public class Builder
        {
            public bool HasTestData { get; set; }
            public string Namespace { get; set; }
            public string SubNamespace { get; set; }

            public ProcessNamespaceResult Build()
            {
                if (string.IsNullOrWhiteSpace(this.Namespace)) throw new InvalidOperationException("Namespace darf nicht leer sein.");
                if (this.SubNamespace == null) throw new InvalidOperationException("SubNamespace darf nicht null sein.");

                return new ProcessNamespaceResult(this);
            }
        }
    }
}
