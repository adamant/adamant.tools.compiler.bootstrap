using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class TypeDeclarationSyntax : DeclarationSyntax, INamespacedDeclarationSyntax, IMemberDeclarationSyntax, ISymbol
    {
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        protected TypeDeclarationSyntax([NotNull] CodeFile file, TextSpan nameSpan)
            : base(file, nameSpan)
        {
        }

        public Name Name => throw new System.NotImplementedException();

        public IEnumerable<DataType> Types => throw new System.NotImplementedException();

        public ISymbol ComposeWith(ISymbol symbol)
        {
            throw new System.NotImplementedException();
        }

        public ISymbol Lookup(SimpleName name)
        {
            throw new System.NotImplementedException();
        }
    }
}
