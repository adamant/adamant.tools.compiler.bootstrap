using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface ITypeNameSyntax : ITypeSyntax
    {
        [DisallowNull] ISymbol? ReferencedSymbol { get; set; }
        [DisallowNull] LexicalScope? ContainingScope { get; set; }
        FixedList<ISymbol> LookupInContainingScope();
    }
}
