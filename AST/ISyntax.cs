using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IDeclarationSyntax),
        typeof(IStatementSyntax),
        typeof(IParameterSyntax),
        typeof(IUsingDirectiveSyntax),
        typeof(IModifierSyntax),
        typeof(ICompilationUnitSyntax),
        typeof(IArgumentSyntax),
        typeof(ITypeSyntax),
        typeof(IExpressionSyntax))]
    public interface ISyntax
    {
        TextSpan Span { get; }
        string ToString();
    }
}
