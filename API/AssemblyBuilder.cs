using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.API
{
    public class AssemblyBuilder : CodeBuilder
    {
        public new const string LineTerminator = "\n";

        /// Whether we need a blank separator line before the next declaration
        public bool NeedsDeclarationSeparatorLine { get; private set; }

        public AssemblyBuilder(string indentCharacters = "    ")
            : base(indentCharacters, LineTerminator)
        {
        }

        public override void EndLine(string value)
        {
            base.EndLine(value);
            NeedsDeclarationSeparatorLine = true;
        }

        public override void EndLine()
        {
            base.EndLine();
            NeedsDeclarationSeparatorLine = true;
        }

        public override void AppendLine(string value)
        {
            base.AppendLine(value);
            NeedsDeclarationSeparatorLine = true;
        }

        public override void BlankLine()
        {
            base.BlankLine();
            NeedsDeclarationSeparatorLine = false;
        }

        public virtual void DeclarationSeparatorLine()
        {
            if (!NeedsDeclarationSeparatorLine) return;
            base.BlankLine();
            NeedsDeclarationSeparatorLine = false;
        }

        public override void BeginBlock()
        // ensures CurrentIndentDepth == old(CurrentIndentDepth) + 1
        {
            base.AppendLine("{");
            base.BeginBlock();
            NeedsDeclarationSeparatorLine = false;
        }

        public override void EndBlock()
        // ensures CurrentIndentDepth == old(CurrentIndentDepth) - 1
        {
            Requires.That(nameof(CurrentIndent), CurrentIndentDepth > 0, "indent depth must not be zero");
            base.EndBlock();
            base.AppendLine("}");
            NeedsDeclarationSeparatorLine = true;
        }
    }
}
