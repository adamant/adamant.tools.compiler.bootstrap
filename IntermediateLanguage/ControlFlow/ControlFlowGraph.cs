using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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

        public ControlFlowGraph(
            IEnumerable<LocalVariableDeclaration> variableDeclarations,
            IEnumerable<BasicBlock> basicBlocks)
        {
            VariableDeclarations = variableDeclarations.ToFixedList();
            BasicBlocks = basicBlocks.ToFixedList();
            Edges = Edges.InGraph(this);
        }
    }
}
