using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INameExpressionSyntax : IExpressionSyntax
    {
        SimpleName Name { get; }
        [DisallowNull] LexicalScope? ContainingScope { get; set; }
        [DisallowNull] ISymbol? ReferencedSymbol { get; set; }
        FixedList<ISymbol> LookupInContainingScope();
    }
}
