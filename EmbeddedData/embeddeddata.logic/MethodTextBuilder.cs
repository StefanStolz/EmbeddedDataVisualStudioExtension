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
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace embeddeddata.logic
{
    public class MethodTextBuilder
    {
        private readonly string methodName;
        private string returnType = "void";
        private readonly List<ParameterDefinition> parameters = new List<ParameterDefinition>();
        private bool isStatic;
        private MethodVisibility methodVisibility;

        private MethodTextBuilder(string methodName)
        {
            this.methodName = methodName;
        }

        public static MethodTextBuilder Create(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(methodName));

            return new MethodTextBuilder(methodName);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            this.WriteTo(sb);

            return sb.ToString().Trim();
        }

        private void WriteTo(ITargetAdapter target)
        {
            var sb = new StringBuilder();

            this.AppendMethodVisibility(sb);
            if (this.isStatic)
            {
                sb.Append("static ");
            }
            sb.Append($"{this.returnType} {this.methodName}");
            sb.Append("(");
            sb.Append(string.Join(", ", this.parameters));
            sb.Append(")");

            target.WriteLine(sb.ToString());
        }

        private void AppendMethodVisibility(StringBuilder sb)
        {
            switch (this.methodVisibility)
            {
                case MethodVisibility.Private:
                    sb.Append("private ");
                    break;
                case MethodVisibility.Protected:
                    sb.Append("protected ");
                    break;
                case MethodVisibility.Internal:
                    sb.Append("internal ");
                    break;
                case MethodVisibility.Public:
                    sb.Append("public ");
                    break;
            }
        }

        public void WriteTo(StringBuilder targetStringBuilder)
        {
            this.WriteTo(new StringBuilderAdapter(targetStringBuilder));
        }

        public void WriteTo(CodeTextWriter codeTextWriter)
        {
            this.WriteTo(new CodeWriterAdapter(codeTextWriter));
        }

        private class CodeWriterAdapter : ITargetAdapter
        {
            private readonly CodeTextWriter codeTextWriter;

            public CodeWriterAdapter(CodeTextWriter codeTextWriter)
            {
                this.codeTextWriter = codeTextWriter;
            }

            public void WriteLine(string line)
            {
                this.codeTextWriter.WriteLine(line);
            }
        }

        private interface ITargetAdapter
        {
            void WriteLine(string line);
        }

        public class StringBuilderAdapter : ITargetAdapter
        {
            private readonly StringBuilder stringBuilder;

            public StringBuilderAdapter(StringBuilder stringBuilder)
            {
                this.stringBuilder = stringBuilder;
            }

            public void WriteLine(string line)
            {
                this.stringBuilder.AppendLine(line);
            }
        }

        public MethodTextBuilder AddParameter(string typeOfParameter, string nameOfParameter)
        {
            return this.BeginParameter().SetType(typeOfParameter).SetName(nameOfParameter).Finish();
        }

        public ParameterDefinition BeginParameter()
        {
            return new ParameterDefinition(this);
        }

        public class ParameterDefinition
        {
            private readonly MethodTextBuilder parent;
            private readonly List<string> attrbutes = new List<string>();
            private string typeOfParameter;
            private string nameOfParameter;

            public ParameterDefinition(MethodTextBuilder parent)
            {
                this.parent = parent;
            }

            public ParameterDefinition AddAttribute(string attribute)
            {
                this.attrbutes.Add(attribute);

                return this;
            }

            public ParameterDefinition SetType(string typeOfParameter)
            {
                this.typeOfParameter = typeOfParameter;

                return this;
            }

            public ParameterDefinition SetName(string parameterName)
            {
                this.nameOfParameter = parameterName;

                return this;
            }

            public MethodTextBuilder Finish()
            {
                this.parent.parameters.Add(this);

                return this.parent;
            }

            public override string ToString()
            {
                var attribs = string.Join(String.Empty, this.attrbutes.Select(a => $"[{a}]"));

                if (attribs.Any())
                {
                    attribs += " ";
                }

                return $"{attribs}{this.typeOfParameter} {this.nameOfParameter}";
            }
        }

        public MethodTextBuilder SetReturnType(string returnType)
        {
            this.returnType = returnType;

            return this;
        }

        public MethodTextBuilder SetStatic(bool isStatic = true)
        {
            this.isStatic = isStatic;

            return this;
        }

        public MethodTextBuilder SetVisibility(MethodVisibility methodVisibility)
        {
            this.methodVisibility = methodVisibility;

            return this;
        }
    }

    public enum MethodVisibility
    {
        Private,
        Protected,
        Internal,
        Public,
    }
}
