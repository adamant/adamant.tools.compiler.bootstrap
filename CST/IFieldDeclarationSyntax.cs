using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IFieldDeclarationSyntax : IBindingMetadata
    {
        [DisallowNull] ref IExpressionSyntax? Initializer { get; }
    }
}
