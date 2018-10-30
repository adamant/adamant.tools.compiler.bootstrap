using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types.Inheritance
{
    public class BaseClassSyntax : SyntaxNode
    {
        [NotNull] public ColonToken Colon { get; }
        [NotNull] public ExpressionSyntax TypeExpression { get; }

        public BaseClassSyntax([NotNull] ColonToken colon, [NotNull] ExpressionSyntax typeExpression)
        {
            Requires.NotNull(nameof(colon), colon);
            Requires.NotNull(nameof(typeExpression), typeExpression);
            Colon = colon;
            TypeExpression = typeExpression;
        }
    }
}
