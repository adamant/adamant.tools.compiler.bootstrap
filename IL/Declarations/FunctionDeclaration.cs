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
        public DataType ReturnType => variableDeclarations[0].Type;
        public IEnumerable<LocalVariableDeclaration> Parameters => variableDeclarations.Skip(1).Take(Arity);
        public IReadOnlyList<BasicBlock> BasicBlocks { get; }
        private readonly List<BasicBlock> basicBlocks = new List<BasicBlock>();

        public FunctionDeclaration(string name, int arity)
        {
            Name = name;
            Arity = arity;
            VariableDeclarations = variableDeclarations.AsReadOnly();
            BasicBlocks = basicBlocks.AsReadOnly();
        }
    }
}
