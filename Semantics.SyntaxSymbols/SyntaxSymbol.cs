using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{

    internal abstract class SyntaxSymbol : ISyntaxSymbol
    {
        public string Name { get; }
        public int DeclarationNumber { get; }

        private readonly List<SyntaxSymbol> children = new List<SyntaxSymbol>();
        public IReadOnlyList<SyntaxSymbol> Children { get; }
        IReadOnlyList<ISyntaxSymbol> ISyntaxSymbol.Children => Children;

        IReadOnlyList<SyntaxBranchNode> ISyntaxSymbol.Declarations => GetDeclarations();

        protected SyntaxSymbol(string name)
            : this(name, 0)
        {
        }

        protected SyntaxSymbol(string name, int declarationNumber)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
            DeclarationNumber = declarationNumber;
            Children = children.AsReadOnly();
        }

        public void Add(SyntaxSymbol child)
        {
            children.Add(child);
        }

        protected abstract IReadOnlyList<SyntaxBranchNode> GetDeclarations();
    }

    internal abstract class SyntaxSymbol<T> : SyntaxSymbol
        where T : SyntaxBranchNode
    {
        private readonly List<T> declarations = new List<T>();
        public IReadOnlyList<T> Declarations { get; }

        protected SyntaxSymbol(
            string name,
            T declaration)
            : base(name)
        {
            Declarations = declarations.AsReadOnly();
            declarations.Add(declaration);
        }

        protected SyntaxSymbol(string name, int declarationNumber, T declaration)
            : base(name, declarationNumber)
        {
            Requires.NotNull(nameof(declaration), declaration);
            Declarations = declarations.AsReadOnly();
            declarations.Add(declaration);
        }

        public void AddDeclaration(T declaration)
        {
            declarations.Add(declaration);
        }

        protected sealed override IReadOnlyList<SyntaxBranchNode> GetDeclarations()
        {
            return Declarations;
        }
    }
}
