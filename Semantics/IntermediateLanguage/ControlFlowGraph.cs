using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class ControlFlowGraph
    {
        [NotNull] [ItemNotNull] public IReadOnlyList<LocalVariableDeclaration> VariableDeclarations { get; }
        [NotNull] [ItemNotNull] private readonly List<LocalVariableDeclaration> variableDeclarations = new List<LocalVariableDeclaration>();
        [NotNull] public LocalVariableDeclaration ReturnVariable => variableDeclarations[0];
        [NotNull] public DataType ReturnType => ReturnVariable.Type;
        //[NotNull] [ItemNotNull] public IEnumerable<LocalVariableDeclaration> Parameters => variableDeclarations.Skip(1).Take(Arity);
        [NotNull] [ItemNotNull] public IReadOnlyList<BasicBlock> BasicBlocks { get; }
        [NotNull] [ItemNotNull] private readonly List<BasicBlock> basicBlocks = new List<BasicBlock>();
        [NotNull] public BasicBlock EntryBlock => basicBlocks.First().NotNull();
        public BasicBlock ExitBlock => basicBlocks.Last(); // TODO this is likely not right

        public ControlFlowGraph()
        {
            VariableDeclarations = variableDeclarations.AsReadOnly().NotNull();
            BasicBlocks = basicBlocks.AsReadOnly().NotNull();
            AddBlock(); // Create the entry block
        }

        [NotNull]
        public LocalVariableDeclaration AddParameter(bool mutableBinding, [NotNull] DataType type, [CanBeNull] string name)
        {
            Requires.NotNull(nameof(type), type);
            var variable = new LocalVariableDeclaration(true, mutableBinding, type, variableDeclarations.Count)
            {
                Name = name
            };
            variableDeclarations.Add(variable);
            return variable;
        }

        [NotNull]
        public LocalVariableDeclaration AddVariable(bool mutableBinding, [NotNull] DataType type, [CanBeNull] string name = null)
        {
            Requires.NotNull(nameof(type), type);
            var variable = new LocalVariableDeclaration(false, mutableBinding, type, variableDeclarations.Count)
            {
                Name = name
            };
            variableDeclarations.Add(variable);
            return variable;
        }

        [NotNull]
        public LocalVariableDeclaration Let([NotNull] DataType type)
        {
            Requires.NotNull(nameof(type), type);
            return AddVariable(false, type);
        }

        [NotNull]
        public LocalVariableDeclaration Var([NotNull] DataType type)
        {
            Requires.NotNull(nameof(type), type);
            return AddVariable(true, type);
        }

        [NotNull]
        public BasicBlock AddBlock()
        {
            var block = new BasicBlock(basicBlocks.Count);
            basicBlocks.Add(block);
            return block;
        }

        [NotNull]
        public Edges Edges()
        {
            return IntermediateLanguage.Edges.InGraph(this);
        }
    }
}
