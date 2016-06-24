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
using System.Linq;
using System.Text;

#endregion

namespace embeddeddata.logic
{
    public class CodeTextWriter
    {
        private readonly string indentText;
        private readonly StringBuilder sb = new StringBuilder();
        private int currentIndent = 0;

        private string currentIndentText = string.Empty;

        public CodeTextWriter(string indentText)
        {
            if (indentText == null) throw new ArgumentNullException(nameof(indentText));

            this.indentText = indentText;
        }

        public CodeTextWriter()
            : this("  ")
        {}

        public CodeTextWriter DecreaseIndent()
        {
            this.currentIndent = Math.Max(this.currentIndent - 1, 0);

            this.UpdateIndentText();

            return this;
        }

        public StringBuilder GetStringBuilder()
        {
            return this.sb;
        }

        public CodeTextWriter IncreaseIndent()
        {
            this.currentIndent++;

            this.UpdateIndentText();

            return this;
        }

        public IDisposable Indent()
        {
            this.IncreaseIndent();

            return new Trigger(() => this.DecreaseIndent());
        }

        public override string ToString()
        {
            return this.sb.ToString();
        }

        private void UpdateIndentText()
        {
            this.currentIndentText = string.Join(string.Empty, Enumerable.Repeat(this.indentText, this.currentIndent));
        }

        public IDisposable WriteBlock()
        {
            this.WriteLine("{");
            this.IncreaseIndent();

            return new Trigger(
                () =>
                {
                    this.DecreaseIndent();
                    this.WriteLine("}");
                });
        }

        public IDisposable WriteBlock(string blockPrefix)
        {
            this.WriteLine($"{blockPrefix}");
            this.WriteLine("{");
            this.IncreaseIndent();

            return new Trigger(
                () =>
                {
                    this.DecreaseIndent();
                    this.WriteLine("}");
                });
        }

        public IDisposable WriteBlock(string blockPrefix, string blockSuffix)
        {
            this.WriteLine($"{blockPrefix} {{");
            this.IncreaseIndent();

            return new Trigger(
                () =>
                {
                    this.DecreaseIndent();
                    this.WriteLine($"}}{blockSuffix}");
                });
        }

        public CodeTextWriter WriteLine()
        {
            this.sb.Append(this.currentIndentText);
            this.sb.AppendLine();

            return this;
        }

        public CodeTextWriter WriteLine(string text)
        {
            this.sb.Append(this.currentIndentText);
            this.sb.AppendLine(text);

            return this;
        }

        public CodeTextWriter WriteLine(string format, params object[] args)
        {
            this.sb.Append(this.currentIndentText);
            this.sb.AppendFormat(format, args).AppendLine();

            return this;
        }

        private class Trigger : IDisposable
        {
            private readonly Action action;

            public Trigger(Action action)
            {
                this.action = action;
            }

            public void Dispose()
            {
                this.action();
            }
        }
    }
}
