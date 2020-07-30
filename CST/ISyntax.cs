using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
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
        typeof(IArgumentSyntax),
        typeof(IReachabilityAnnotationSyntax))]
    public interface ISyntax
    {
        TextSpan Span { get; }
        string ToString();
    }
}
