using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IFieldDeclarationSyntax
    {
        [DisallowNull] ref IExpressionSyntax? Initializer { get; }
    }
}
