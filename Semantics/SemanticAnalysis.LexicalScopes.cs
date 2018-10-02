using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        public const string LexicalScopeAttribute = "LexicalScope";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LexicalScope LexicalScope(SyntaxBranchNode syntax)
        {
            return attributes.GetOrAdd(syntax, LexicalScopeAttribute, ComputeLexicalScope);
        }

        private LexicalScope ComputeLexicalScope(SyntaxBranchNode syntax)
        {
            switch (syntax)
            {
                case CompilationUnitSyntax compilationUnit:
                    return new LexicalScope(PackageSyntaxSymbol.GlobalNamespace);

                case FunctionDeclarationSyntax function:
                    {
                        var enclosingScope = LexicalScope(Parent(function));
                        var symbol = SyntaxSymbol(function);
                        return new LexicalScope(enclosingScope, symbol);
                    }

                case StatementSyntax _:
                case ExpressionSyntax _:
                case ParameterSyntax _:
                case ParameterListSyntax _:
                case ArgumentListSyntax _:
                    {
                        // Inherit
                        var enclosingScope = LexicalScope(Parent(syntax));
                        return enclosingScope;
                    }

                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }
    }
}
