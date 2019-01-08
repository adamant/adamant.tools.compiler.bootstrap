using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class IfStatement : BlockTerminatorStatement
    {
        public Operand Condition { get; }
        public int ThenBlockNumber { get; }
        public int ElseBlockNumber { get; }

        public IfStatement(Operand condition, int thenBlockNumber, int elseBlockNumber)
        {
            Condition = condition;
            ThenBlockNumber = thenBlockNumber;
            ElseBlockNumber = elseBlockNumber;
        }

        public override IEnumerable<int> OutBlocks()
        {
            yield return ThenBlockNumber;
            yield return ElseBlockNumber;
        }

        public override Statement Clone()
        {
            return new IfStatement(Condition, ThenBlockNumber, ElseBlockNumber);
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"if {Condition} -> bb{ThenBlockNumber} else -> bb{ElseBlockNumber}";
        }
    }
}
