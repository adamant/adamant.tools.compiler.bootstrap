using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class ControlFlowGraph
    {
        public FixedList<LocalVariableDeclaration> VariableDeclarations { get; }
        public LocalVariableDeclaration ReturnVariable => VariableDeclarations[0];
        public DataType ReturnType => ReturnVariable.Type;
        public FixedList<BasicBlock> BasicBlocks { get; }
        public BasicBlock EntryBlock => BasicBlocks.First();
        public IEnumerable<BasicBlock> ExitBlocks => BasicBlocks.Where(b => b.Terminator is ReturnStatement);
        public Edges Edges { get; }

        /// <summary>
        /// If requested, the semantic analyzer will store the live variables here
        /// </summary>
        public LiveVariables LiveVariables { get; set; }

        /// <summary>
        /// If requested, the semantic analyzer will store the borrow claims here
        /// </summary>
        public ControlFlowGraphClaims BorrowClaims { get; set; }

        public ControlFlowGraph(
            IEnumerable<LocalVariableDeclaration> variableDeclarations,
            IEnumerable<BasicBlock> basicBlocks)
        {
            VariableDeclarations = variableDeclarations.ToFixedList();
            BasicBlocks = basicBlocks.ToFixedList();
            Edges = Edges.InGraph(this);
        }

        public BasicBlock this[BasicBlockName block] => BasicBlocks[block.Number];
    }
}
