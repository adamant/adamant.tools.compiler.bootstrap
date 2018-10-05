using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        private const string EnclosingFunctionAttribute = "EnclosingFunction";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FunctionDeclarationSyntax EnclosingFunction(SyntaxNode s)
        {
            return attributes.GetOrAdd(s, EnclosingFunctionAttribute, ComputeEnclosingFunction);
        }

        private FunctionDeclarationSyntax ComputeEnclosingFunction(SyntaxNode syntax)
        {
            switch (syntax)
            {
                case FunctionDeclarationSyntax function:
                    return function;
                case VariableDeclarationStatementSyntax _:
                case ExpressionStatementSyntax _:
                case BlockSyntax _:
                    // Inherit
                    return EnclosingFunction(Parent(syntax));
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }
    }
}
