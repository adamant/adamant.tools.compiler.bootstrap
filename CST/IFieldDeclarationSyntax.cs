using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IFieldDeclarationSyntax : IMemberDeclarationSyntax, IBindingMetadata
    {
        ITypeSyntax TypeSyntax { get; }
        new DataTypePromise DataType { get; }
        [DisallowNull] ref IExpressionSyntax? Initializer { get; }
    }
}
