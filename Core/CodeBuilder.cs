using System;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public abstract class CodeBuilder
    {
        private readonly StringBuilder code = new StringBuilder();
        public string Code => code.ToString();
        public readonly string IndentCharacters;
        public readonly string LineTerminator;
        public int CurrentIndentDepth { get; private set; }
        public string CurrentIndent => IndentCharacters.Repeat(CurrentIndentDepth);

        protected CodeBuilder(string indentCharacters)
            : this(indentCharacters, Environment.NewLine)
        {
        }

        protected CodeBuilder(string indentCharacters, string lineTerminator)
        {
            IndentCharacters = indentCharacters;
            LineTerminator = lineTerminator;
        }

        public virtual void AppendIndent()
        {
            code.Insert(code.Length, IndentCharacters, CurrentIndentDepth);
        }

        public virtual void BeginLine(string value)
        {
            AppendIndent();
            code.Append(value);
        }

        public virtual void Append(string value)
        {
            code.Append(value);
        }

        public virtual void EndLine(string value)
        {
            code.Append(value);
            code.Append(LineTerminator);
        }

        public virtual void EndLine()
        {
            code.Append(LineTerminator);
        }

        public virtual void AppendLine(string value)
        {
            AppendIndent();
            code.Append(value);
            code.Append(LineTerminator);
        }

        public virtual void BlankLine()
        {
            code.Append(LineTerminator);
        }

        public virtual void BeginBlock()
        // ensures CurrentIndentDepth == old(CurrentIndentDepth) + 1
        {
            CurrentIndentDepth++;
        }

        public virtual void EndBlock()
        // requires CurrentIndentDepth > 0
        // ensures CurrentIndentDepth == old(CurrentIndentDepth)-1
        {
            if (CurrentIndentDepth <= 0)
                throw new InvalidOperationException("Can't end block when indent depth is zero.");
            CurrentIndentDepth--;
        }
    }
}
