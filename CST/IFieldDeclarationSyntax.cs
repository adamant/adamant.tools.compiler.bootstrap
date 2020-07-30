using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IFieldDeclarationSyntax : IMemberDeclarationSyntax, IBindingSymbol
    {
        ITypeSyntax TypeSyntax { get; }
        new TypePromise Type { get; }
        [DisallowNull] ref IExpressionSyntax? Initializer { get; }
    }
}
