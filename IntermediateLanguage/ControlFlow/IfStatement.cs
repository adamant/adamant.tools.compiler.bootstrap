//using System.Collections.Generic;
//using Adamant.Tools.Compiler.Bootstrap.Core;

//namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
//{
//    public class IfStatement : BlockTerminatorStatement
//    {
//        public IOperand Condition { get; }
//        public BasicBlockName ThenBlock { get; }
//        public BasicBlockName ElseBlock { get; }

//        public IfStatement(IOperand condition, BasicBlockName thenBlock, BasicBlockName elseBlock, TextSpan span, Scope scope)
//            : base(span, scope)
//        {
//            Condition = condition;
//            ThenBlock = thenBlock;
//            ElseBlock = elseBlock;
//        }

//        public override IEnumerable<BasicBlockName> OutBlocks()
//        {
//            yield return ThenBlock;
//            yield return ElseBlock;
//        }

//        public override Statement Clone()
//        {
//            return new IfStatement(Condition, ThenBlock, ElseBlock, Span, Scope);
//        }

//        // Useful for debugging
//        public override string ToStatementString()
//        {
//            return $"if {Condition} -> {ThenBlock} else -> {ElseBlock};";
//        }
//    }
//}
