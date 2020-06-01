using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INewObjectExpressionSyntax : IExpressionSyntax
    {
        ITypeNameSyntax TypeSyntax { get; }
        ICallableNameSyntax? ConstructorName { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
        [DisallowNull] IFunctionSymbol? ConstructorSymbol { get; set; }
    }
}
