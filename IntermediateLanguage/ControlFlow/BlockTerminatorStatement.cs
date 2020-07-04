//using System.Collections.Generic;
//using Adamant.Tools.Compiler.Bootstrap.Core;
//using ExhaustiveMatching;

//namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
//{
//    /// <summary>
//    /// A statement that can end a block. These are statements that exit the block
//    /// either to one or more other blocks or by returning from the function.
//    /// </summary>
//    [Closed(
//        typeof(GotoStatement),
//        typeof(IfStatement),
//        typeof(ReturnStatement))]
//    public abstract class BlockTerminatorStatement : Statement
//    {
//        protected BlockTerminatorStatement(TextSpan span, Scope scope)
//            : base(span, scope)
//        {
//        }

//        public abstract IEnumerable<BasicBlockName> OutBlocks();
//    }
//}
