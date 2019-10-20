using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IDeclarationSyntax),
        typeof(IStatementSyntax),
        typeof(IParameterSyntax),
        typeof(IUsingDirectiveSyntax),
        typeof(ICompilationUnitSyntax),
        typeof(ITypeSyntax),
        typeof(IExpressionSyntax),
        typeof(IBodyOrBlockSyntax),
        typeof(ICallableNameSyntax),
        typeof(ITransferSyntax),
        typeof(ILifetimeBoundSyntax))]
    public interface ISyntax
    {
        TextSpan Span { get; }
        string ToString();
    }
}
