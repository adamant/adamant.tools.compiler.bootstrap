using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class GotoStatement : BlockTerminatorStatement
    {
        public readonly BasicBlockName GotoBlock;

        public GotoStatement(BasicBlockName gotoBlock, TextSpan span, Scope scope)
            : base(span, scope)
        {
            GotoBlock = gotoBlock;
        }

        public override IEnumerable<BasicBlockName> OutBlocks()
        {
            yield return GotoBlock;
        }

        public override Statement Clone()
        {
            return new GotoStatement(GotoBlock, Span, Scope);
        }

        public override string ToStatementString()
        {
            return $"goto {GotoBlock};";
        }
    }
}
