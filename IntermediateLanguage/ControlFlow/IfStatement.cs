using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class IfStatement : BlockTerminatorStatement
    {
        public Operand Condition { get; }
        public BasicBlockName ThenBlock { get; }
        public BasicBlockName ElseBlock { get; }

        public IfStatement(Operand condition, BasicBlockName thenBlock, BasicBlockName elseBlock, TextSpan span, Scope scope)
            : base(span, scope)
        {
            Condition = condition;
            ThenBlock = thenBlock;
            ElseBlock = elseBlock;
        }

        public override IEnumerable<BasicBlockName> OutBlocks()
        {
            yield return ThenBlock;
            yield return ElseBlock;
        }

        public override Statement Clone()
        {
            return new IfStatement(Condition, ThenBlock, ElseBlock, Span, Scope);
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"if {Condition} -> {ThenBlock} else -> {ElseBlock} // at {Span} in {Scope}";
        }
    }
}
