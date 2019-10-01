using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INewObjectExpressionSyntax : IExpressionSyntax
    {
        ITypeNameSyntax Constructor { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
        [DisallowNull] ISymbol? ConstructorSymbol { get; set; }
        [DisallowNull] DataType? ConstructorType { get; set; }
    }
}
