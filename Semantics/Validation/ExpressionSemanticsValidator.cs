using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    /// <summary>
    /// Validates all expressions have been assigned expression semantics
    /// </summary>
    public class ExpressionSemanticsValidator : SyntaxWalker
    {
        public void Validate(IEnumerable<IEntityDeclarationSyntax> entityDeclarations)
        {
            foreach (var declaration in entityDeclarations)
                WalkNonNull(declaration);
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IReachabilityAnnotationSyntax _:
                    // Ignore for now
                    return;
                case IClassDeclarationSyntax _:
                    // Don't recur into body, we will see those as separate members
                    return;
                case IExpressionSyntax expression:
                    WalkChildren(expression);
                    expression.Semantics.Assigned();
                    return;
                case ITypeSyntax _:
                    return;
            }

            WalkChildren(syntax);
        }
    }
}
