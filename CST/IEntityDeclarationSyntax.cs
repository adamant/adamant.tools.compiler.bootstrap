using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// All non-namespace declarations since a namespace doesn't really create
    /// a thing, it just defines a group of names.
    /// </summary>
    [Closed(
        typeof(INonMemberEntityDeclarationSyntax),
        typeof(IMemberDeclarationSyntax),
        typeof(ICallableDeclarationSyntax))]
    public interface IEntityDeclarationSyntax : IDeclarationSyntax, IMetadata
    {
        IAccessModifierToken? AccessModifier { get; }
    }
}
