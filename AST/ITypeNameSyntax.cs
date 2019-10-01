using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface ITypeNameSyntax : ITypeSyntax
    {
        [DisallowNull] LexicalScope? ContainingScope { get; set; }
        [DisallowNull] ISymbol? ReferencedSymbol { get; set; }
        FixedList<ISymbol> LookupInContainingScope();
    }
}
