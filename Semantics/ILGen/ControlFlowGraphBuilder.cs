using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen
{
    /// <summary>
    /// Builder pattern for control flow graphs used during control flow graph fabrication
    /// </summary>
    internal class ControlFlowGraphBuilder
    {
        private readonly CodeFile file;
        private readonly List<VariableDeclaration> variables = new List<VariableDeclaration>();
        public VariableDeclaration SelfVariable => variables.SingleOrDefault(v => v.Variable == Variable.Self);
        private readonly List<BlockBuilder> blockBuilders = new List<BlockBuilder>();

        public ControlFlowGraphBuilder(CodeFile file)
        {
            this.file = file;
        }

        public VariableDeclaration AddVariable(bool mutableBinding, DataType type, Scope scope, SimpleName? name = null)
        {
            var variable =
                new VariableDeclaration(false, mutableBinding, type, new Variable(variables.Count), scope, name);
            variables.Add(variable);
            return variable;
        }

        public VariableDeclaration AddParameter(bool mutableBinding, DataType type, Scope scope, SimpleName name)
        {
            var variable =
                new VariableDeclaration(true, mutableBinding, type, new Variable(variables.Count), scope, name);
            variables.Add(variable);
            return variable;
        }

        public VariableDeclaration AddSelfParameter(DataType type)
        {
            Requires.That("variableNumber", variables.Count == 0, "Self parameter must have variable number 0");
            var variable = new VariableDeclaration(true, false, type, Variable.Self, null, SpecialName.Self);
            variables.Add(variable);
            return variable;
        }

        public VariableDeclaration Let(DataType type, Scope scope)
        {
            return AddVariable(false, type, scope);
        }

        public VariableDeclaration Var(DataType type, Scope scope)
        {
            return AddVariable(true, type, scope);
        }

        public VariableDeclaration VariableFor(SimpleName name)
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
        public BlockBuilder NewEntryBlock(BlockBuilder currentBlock, TextSpan span, Scope scope)
        {
            if (!currentBlock.IsTerminated && !currentBlock.Instructions.Any()) return currentBlock;
            var entryBlock = NewBlock();
            currentBlock.End(new GotoInstruction(entryBlock.Number, span, scope));
            return entryBlock;
        }

        public ControlFlowGraph Build()
        {
            // We assume that the first block is the entry block
            var blocks = new List<Block>();
            foreach (var block in blockBuilders)
            {
                var terminator = block.Terminator ?? throw new InvalidOperationException();
                blocks.Add(new Block(block.Number, block.Instructions.ToFixedList(), terminator));
            }

            return new ControlFlowGraph(file, variables, blocks);
        }

        [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification =
            "Variable is a value type, essentially a strongly type integer")]
        public VariableDeclaration this[Variable variable]
        {
            get
            {
                var declaration = variables[variable.Number];
                Debug.Assert(declaration.Variable == variable);
                return declaration;
            }
        }
    }
}
