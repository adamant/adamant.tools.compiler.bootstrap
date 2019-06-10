using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    public class ControlFlowGraphBuilder
    {
        private readonly List<LocalVariableDeclaration> variables = new List<LocalVariableDeclaration>();
        public LocalVariableDeclaration ReturnVariable => variables.First();
        private readonly List<BlockBuilder> blockBuilders = new List<BlockBuilder>();

        public ControlFlowGraphBuilder() { }

        public ControlFlowGraphBuilder(FixedList<LocalVariableDeclaration> variables)
        {
            this.variables = variables.ToList();
        }

        public LocalVariableDeclaration AddVariable(bool mutableBinding, DataType type, SimpleName name = null)
        {
            var variable = new LocalVariableDeclaration(false, mutableBinding, type, new Variable(variables.Count), name);
            variables.Add(variable);
            return variable;
        }

        public LocalVariableDeclaration AddParameter(bool mutableBinding, DataType type, SimpleName name)
        {
            var variable = new LocalVariableDeclaration(true, mutableBinding, type, new Variable(variables.Count), name);
            variables.Add(variable);
            return variable;
        }

        public LocalVariableDeclaration Let(DataType type)
        {
            return AddVariable(false, type);
        }

        public LocalVariableDeclaration Var(DataType type)
        {
            return AddVariable(true, type);
        }

        public LocalVariableDeclaration VariableFor(SimpleName name)
        {
            return variables.Single(v => v.Name == name);
        }

        public BlockBuilder NewBlock()
        {
            var block = new BlockBuilder(blockBuilders.Count);
            blockBuilders.Add(block);
            return block;
        }

        /// Used to create a new block for the entry into a loop if needed.
        /// Doesn't create a new block if the current block is empty.
        public BlockBuilder NewEntryBlock(BlockBuilder currentBlock)
        {
            if (!currentBlock.Statements.Any()) return currentBlock;
            var entryBlock = NewBlock();
            currentBlock.AddGoto(entryBlock);
            return entryBlock;
        }

        public ControlFlowGraph Build()
        {
            // We assume that the first block is the entry block
            var blocks = new List<BasicBlock>();
            foreach (var block in blockBuilders)
            {
                var statements = block.Statements
                    .Take(block.Statements.Count - 1)
                    .Cast<ExpressionStatement>();
                var terminator = block.Terminator;
                blocks.Add(new BasicBlock(block.BlockNumber, statements, terminator));
            }

            return new ControlFlowGraph(variables, blocks);
        }
    }
}
