using System;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Grammars
{
    public static class SemanticNodeTypeGrammar
    {
        public static void Build(AttributeGrammar grammer)
        {
            grammer.For(Attribute.SemanticNodeType)
                .Bind<PackageSyntax, Package>()
                .Bind<CompilationUnitSyntax, CompilationUnit>()
                .Bind<ExpressionStatementSyntax, ExpressionStatement>()
                .Bind<FunctionDeclarationSyntax, FunctionDeclaration>()
                .Bind<ParameterSyntax, Parameter>()
                .Bind<ReturnExpressionSyntax, ReturnExpression>();
        }

        private static InheritedAttributeRuleBuilder<Type> Bind<TSyntax, TSemanticNode>(this InheritedAttributeRuleBuilder<Type> builder)
            where TSyntax : SyntaxBranchNode
            where TSemanticNode : SemanticNode
        {
            builder.Value<TSyntax>(typeof(TSemanticNode));
            return builder;
        }
    }
}
