using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class NameScopeBuilder
    {
        private readonly Annotations annotations;

        internal NameScopeBuilder(Annotations annotations)
        {
            this.annotations = annotations;
        }

        internal void Build(CompilationUnitSyntax compilationUnit)
        {
            var globalNamespaceSymbol = annotations.Symbol(compilationUnit);
            var globalScope = new NameScope(globalNamespaceSymbol);
            annotations.Add(compilationUnit, globalScope);
            foreach (var declaration in compilationUnit.Declarations)
                Build(globalScope, declaration);
        }

        private void Build(NameScope scope, DeclarationSyntax declaration)
        {
            Match.On(declaration).With(m => m
                .Is<FunctionDeclarationSyntax>(f =>
                {
                    var symbol = annotations.Symbol(f);
                    var functionScope = new NameScope(scope, symbol);
                    annotations.Add(f, functionScope);
                    foreach (var parameter in f.ParameterList.Parameters)
                        Apply(functionScope, parameter);

                    foreach (var statement in f.Body.Statements)
                        Apply(functionScope, statement);
                }));
        }

        private void Apply(NameScope scope, ParameterSyntax parameter)
        {
            annotations.Add(parameter, scope);
        }

        private void Apply(NameScope scope, StatementSyntax statement)
        {
            Match.On(statement).With(m => m
                .Is<ExpressionStatementSyntax>(es => Apply(scope, es.Expression)));
        }

        private void Apply(NameScope scope, ExpressionSyntax expression)
        {
            Match.On(expression).With(m => m
                .Is<IdentifierNameSyntax>(i => annotations.Add(i, scope))
                .Is<ReturnExpressionSyntax>(r => Apply(scope, r.Expression))
                .Is<BinaryOperatorExpressionSyntax>(b =>
                {
                    Apply(scope, b.LeftOperand);
                    Apply(scope, b.RightOperand);
                }));
        }
    }
}
