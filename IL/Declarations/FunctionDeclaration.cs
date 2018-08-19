using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Declarations
{
    public class FunctionDeclaration : Declaration
    {
        public readonly string Name;
        public readonly int Arity;
        public IReadOnlyList<LocalVariableDeclaration> VariableDeclarations { get; }
        private readonly List<LocalVariableDeclaration> variableDeclarations = new List<LocalVariableDeclaration>();
        public LocalVariableDeclaration ReturnVariable => variableDeclarations[0];
        public DataType ReturnType => ReturnVariable.Type;
        public IEnumerable<LocalVariableDeclaration> Parameters => variableDeclarations.Skip(1).Take(Arity);
        public IReadOnlyList<BasicBlock> BasicBlocks { get; }
        private readonly List<BasicBlock> basicBlocks = new List<BasicBlock>();
        public BasicBlock EntryBlock => basicBlocks.First();

        public FunctionDeclaration(string name, int arity)
        {
            Name = name;
            Arity = arity;
            VariableDeclarations = variableDeclarations.AsReadOnly();
            BasicBlocks = basicBlocks.AsReadOnly();
            AddBlock(); // Create the entry block
        }

        public LocalVariableDeclaration AddVariable(bool mutableBinding, DataType type, string name = null)
        {
            var variable = new LocalVariableDeclaration(mutableBinding, type, variableDeclarations.Count);
            variable.Name = name;
            variableDeclarations.Add(variable);
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

        public BasicBlock AddBlock()
        {
            var block = new BasicBlock(basicBlocks.Count);
            basicBlocks.Add(block);
            return block;
        }
    }
}