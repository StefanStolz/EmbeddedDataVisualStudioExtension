#region File Header

// Copyright © AWIN-Software, 2016

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

        private string currentIndentText = string.Empty;
        private int currentIndent = 0;

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
