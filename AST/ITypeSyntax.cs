using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IReferenceLifetimeSyntax),
        typeof(ISelfTypeSyntax),
        typeof(ITypeNameSyntax),
        typeof(IMutableTypeSyntax))]
    public interface ITypeSyntax : ISyntax
    {
        [DisallowNull] DataType? NamedType { get; set; }
    }
}
