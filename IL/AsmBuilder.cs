using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IL
{
    internal class AsmBuilder : CodeBuilder
    {
        public new const string LineTerminator = "\n";

        public AsmBuilder()
            : base("    ", LineTerminator)
        {
        }

        public override void BeginBlock()
        // ensures CurrentIndentDepth == old(CurrentIndentDepth) + 1
        {
            base.AppendLine("{");
            base.BeginBlock();
        }


        public override void EndBlock()
        // requires CurrentIndentDepth > 0
        // ensures CurrentIndentDepth == old(CurrentIndentDepth) - 1
        {
            base.EndBlock();
            base.AppendLine("}");
        }
    }
}
