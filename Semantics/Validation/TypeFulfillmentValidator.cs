using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    /// <summary>
    /// Validates that all types are fulfilled. That is that everything as an
    /// assigned type, even if that type is Unknown.
    /// </summary>
    public class TypeFulfillmentValidator : SyntaxWalker
    {
        public void Walk(IEnumerable<IEntityDeclarationSyntax> entityDeclarations)
        {
            foreach (var declaration in entityDeclarations)
                WalkNonNull(declaration);
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IClassDeclarationSyntax _:
                    // Don't recur into body, we will see those as separate members
                    return;
                case ITypeSyntax type:
                    WalkChildren(type);
                    type.NamedType.Assigned();
                    return;
                case IForeachExpressionSyntax foreachExpression:
                    WalkChildren(foreachExpression);
                    foreachExpression.DataType.Assigned();
                    return;
                case IExpressionSyntax expression:
                    WalkChildren(expression);
                    expression.DataType.Assigned();
                    return;
            }

            WalkChildren(syntax);
        }
    }
}
