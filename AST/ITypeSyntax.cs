using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(ICapabilityTypeSyntax),
        typeof(ITypeNameSyntax),
        typeof(IOptionalTypeSyntax))]
    public interface ITypeSyntax : ISyntax
    {
        [DisallowNull] DataType? NamedType { get; set; }
    }
}
