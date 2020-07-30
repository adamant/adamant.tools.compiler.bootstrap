using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// A statement that can appear directly in a method body. (i.e. not a result statement)
    /// </summary>
    [Closed(
        typeof(IVariableDeclarationStatementSyntax),
        typeof(IExpressionStatementSyntax))]
    public interface IBodyStatementSyntax : IStatementSyntax
    {
    }
}
