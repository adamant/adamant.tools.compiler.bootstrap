using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    /// <summary>
    /// All non-namespace declarations since a namespace doesn't really create
    /// a thing, it just defines a group of names.
    /// </summary>
    [Closed(
        typeof(INonMemberEntityDeclarationSyntax),
        typeof(IMemberDeclarationSyntax),
        typeof(ICallableDeclarationSyntax))]
    public interface IEntityDeclarationSyntax : IDeclarationSyntax, ISymbol
    {
        FixedList<IModiferToken> Modifiers { get; }
    }
}
