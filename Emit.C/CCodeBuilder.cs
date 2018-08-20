using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class CCodeBuilder : CodeBuilder
    {
        public new const string LineTerminator = "\n";

        /// Whether we need a black separator line before the next declaration
        public bool NeedsDeclarationSeparatorLine { get; private set; }

        /// Whether we need a blank separator line before the next statement
        public bool NeedsStatementSeparatorLine { get; private set; }

        public CCodeBuilder(string indentCharacters = "    ")
            : base(indentCharacters, LineTerminator)
        {
        }

        public override void EndLine(string value)
        {
            base.EndLine(value);
            NeedsDeclarationSeparatorLine = true;
            NeedsStatementSeparatorLine = false;
        }

        public override void EndLine()
        {
            base.EndLine();
            NeedsDeclarationSeparatorLine = true;
            NeedsStatementSeparatorLine = false;
        }

        public override void AppendLine(string value)
        {
            base.AppendLine(value);
            NeedsDeclarationSeparatorLine = true;
            NeedsStatementSeparatorLine = false;
        }

        public override void BlankLine()
        {
            base.BlankLine();
            NeedsDeclarationSeparatorLine = false;
            NeedsStatementSeparatorLine = false;
        }

        public virtual void DeclarationSeparatorLine()
        {
            if (!NeedsDeclarationSeparatorLine) return;
            base.BlankLine();
            NeedsDeclarationSeparatorLine = false;
            NeedsStatementSeparatorLine = false;
        }

        public virtual void StatementSeparatorLine()
        {
            if (!NeedsStatementSeparatorLine) return;
            base.BlankLine();
            NeedsDeclarationSeparatorLine = false;
            NeedsStatementSeparatorLine = false;
        }

        public override void BeginBlock()
        // ensures CurrentIndentDepth == old(CurrentIndentDepth) + 1
        {
            base.AppendLine("{");
            base.BeginBlock();
            NeedsDeclarationSeparatorLine = false;
            NeedsStatementSeparatorLine = false;
        }

        public virtual void BeginBlockWith(string value)
        // ensures CurrentIndentDepth == old(CurrentIndentDepth) + 1
        {
            base.AppendLine(value);
            base.BeginBlock();
            NeedsDeclarationSeparatorLine = false;
            NeedsStatementSeparatorLine = false;
        }

        public override void EndBlock()
        // requires CurrentIndentDepth > 0
        // ensures CurrentIndentDepth == old(CurrentIndentDepth) - 1
        {
            base.EndBlock();
            base.AppendLine("}");
            NeedsDeclarationSeparatorLine = true;
            NeedsStatementSeparatorLine = true;
        }

        public virtual void EndBlockWithSemicolon()
        // requires CurrentIndentDepth > 0
        // ensures CurrentIndentDepth == old(CurrentIndentDepth) - 1
        {
            base.EndBlock();
            base.AppendLine("};");
            NeedsDeclarationSeparatorLine = true; // These end classes and things that need a separator
            NeedsStatementSeparatorLine = false; // Statements shouldn't come after blocks with semicolons
        }

        public virtual void EndBlockWith(string value)
        // requires CurrentIndentDepth > 0
        // ensures CurrentIndentDepth == old(CurrentIndentDepth) - 1
        {
            base.EndBlock();
            base.AppendLine(value);
            NeedsDeclarationSeparatorLine = true;
            NeedsStatementSeparatorLine = false; // TODO
        }
    }
}
