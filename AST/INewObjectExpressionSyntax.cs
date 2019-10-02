using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INewObjectExpressionSyntax : IExpressionSyntax
    {
        ITypeNameSyntax TypeSyntax { get; }
        INameExpressionSyntax? ConstructorName { get; }
        FixedList<Argument> Arguments { get; }
        [DisallowNull] ISymbol? ConstructorSymbol { get; set; }
        /// <summary>
        /// Because of ownership in the type, the constructed type is different
        /// than the type of the expression.
        /// </summary>
        [DisallowNull] DataType? ConstructorType { get; set; }
    }
}
