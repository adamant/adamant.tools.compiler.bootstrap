using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    // NOTE: keeping this class around for now on the assumption there might be
    // other kinds of generic constraints that aren't just expressions
    public class GenericConstraintSyntax : Syntax
    {
        [NotNull] public ExpressionSyntax Expression { get; }

        public GenericConstraintSyntax(
            [NotNull] ExpressionSyntax expression)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
