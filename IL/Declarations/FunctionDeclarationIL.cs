using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IL.Code;
using Adamant.Tools.Compiler.Bootstrap.IL.Refs;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Declarations
{
    public class FunctionDeclarationIL : DeclarationIL
    {
        public readonly string Name;
        public readonly int Arity;
        [NotNull] [ItemNotNull] public IReadOnlyList<LocalVariableDeclaration> VariableDeclarations { get; }
        [NotNull] [ItemNotNull] private readonly List<LocalVariableDeclaration> variableDeclarations = new List<LocalVariableDeclaration>();
        [NotNull] public LocalVariableDeclaration ReturnVariable => variableDeclarations[0];
        [NotNull] public DataType ReturnType => ReturnVariable.Type;
        [NotNull] [ItemNotNull] public IEnumerable<LocalVariableDeclaration> Parameters => variableDeclarations.Skip(1).Take(Arity);
        [NotNull] [ItemNotNull] public IReadOnlyList<BasicBlock> BasicBlocks { get; }
        [NotNull] [ItemNotNull] private readonly List<BasicBlock> basicBlocks = new List<BasicBlock>();
        public BasicBlock EntryBlock => basicBlocks.First();
        public BasicBlock ExitBlock => basicBlocks.Last(); // TODO this is likely not right

        public FunctionDeclarationIL([NotNull] string name, int arity)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
            Arity = arity;
            VariableDeclarations = variableDeclarations.AsReadOnly().AssertNotNull();
            BasicBlocks = basicBlocks.AsReadOnly().AssertNotNull();
            AddBlock(); // Create the entry block
        }

        public LocalVariableDeclaration AddVariable(bool mutableBinding, [NotNull] DataType type, [CanBeNull] string name = null)
        {
            Requires.NotNull(nameof(type), type);
            var variable = new LocalVariableDeclaration(mutableBinding, type, variableDeclarations.Count)
            {
                Name = name
            };
            variableDeclarations.Add(variable);
            return variable;
        }

        public LocalVariableDeclaration Let([NotNull] DataType type)
        {
            Requires.NotNull(nameof(type), type);
            return AddVariable(false, type);
        }

        public LocalVariableDeclaration Var([NotNull] DataType type)
        {
            Requires.NotNull(nameof(type), type);
            return AddVariable(true, type);
        }

        public BasicBlock AddBlock()
        {
            var block = new BasicBlock(basicBlocks.Count);
            basicBlocks.Add(block);
            return block;
        }

        public Edges Edges()
        {
            return new Edges(this);
        }

        internal override void ToString([NotNull] AsmBuilder builder)
        {
            builder.AppendLine($"public function {Name}() -> %0: &{ReturnType}");
            builder.BeginBlock();
            foreach (var variable in variableDeclarations.Skip(Arity + 1))
            {
                variable.ToString(builder);
            }

            foreach (var block in basicBlocks)
            {
                builder.BlankLine();
                block.ToString(builder);
            }
            builder.EndBlock();
        }
    }
}
