//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using Adamant.Tools.Compiler.Bootstrap.Framework;
//using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

//namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
//{
//    [DebuggerDisplay("Blocks = {values.Count}, Variables = {VariableCount}")]
//    public class LiveVariables
//    {
//        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
//        public int VariableCount { get; }

//        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
//        private readonly FixedList<List<BitArray>> values;

//        public LiveVariables(ControlFlowGraphOld graphOld)
//        {
//            VariableCount = graphOld.VariableDeclarations.Count;
//            values = graphOld.BasicBlocks.Select(block =>
//                            block.Statements.Select(s => new BitArray(VariableCount)).ToList())
//                        .ToFixedList();
//        }

//        /// <summary>
//        /// Note, this will allow you to ask for the live variables before the
//        /// block terminator as if it were a statement.
//        /// </summary>
//        public BitArray Before(Statement statement)
//        {
//            return values[statement.Block.Number][statement.Number];
//        }

//        public BitArray After(Statement statement)
//        {
//            return values[statement.Block.Number][statement.Number + 1];
//        }

//        /// <summary>
//        /// Returns the new liveness array for between the two statements
//        /// </summary>
//        public BitArray AddStatementAfter(Statement statement)
//        {
//            var liveVariables = new BitArray(VariableCount);
//            // The live variables after the statement should be based on what they were before
//            liveVariables.Or(After(statement));
//            values[statement.Block.Number].Insert(statement.Number + 1, liveVariables);
//            return liveVariables;
//        }
//    }
//}
