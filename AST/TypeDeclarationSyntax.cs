using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class TypeDeclarationSyntax : DeclarationSyntax, INonMemberDeclarationSyntax, IMemberDeclarationSyntax, ISymbol
    {
        protected TypeDeclarationSyntax(TextSpan nameSpan)
            : base(nameSpan)
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
