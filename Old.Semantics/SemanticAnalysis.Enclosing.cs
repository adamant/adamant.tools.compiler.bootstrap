using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics
{
    internal partial class SemanticAnalysis
    {
        private const string EnclosingFunctionAttribute = "EnclosingFunction";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FunctionDeclarationSyntax EnclosingFunction([NotNull] SyntaxNode s)
        {
            return attributes.GetOrAdd(s, EnclosingFunctionAttribute, ComputeEnclosingFunction);
        }

        [NotNull]
        private FunctionDeclarationSyntax ComputeEnclosingFunction([NotNull] SyntaxNode syntax)
        {
            switch (syntax)
            {
                case FunctionDeclarationSyntax function:
                    return function;
                case VariableDeclarationStatementSyntax _:
                case ExpressionStatementSyntax _:
                case BlockExpressionSyntax _:
                    // Inherit
                    return EnclosingFunction(Parent(syntax));
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }
    }
}
