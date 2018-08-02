using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Core.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticTreeBuilder
    {
        private readonly Annotations annotations;

        internal SemanticTreeBuilder(Annotations annotations)
        {
            this.annotations = annotations;
        }

        public CompilationUnit Build(CompilationUnitSyntax compilationUnit)
        {
            var declarations = compilationUnit.Declarations.Select(Build);
            return new CompilationUnit(compilationUnit, declarations, annotations.Diagnostics(compilationUnit));
        }

        private Declaration Build(DeclarationSyntax declaration)
        {
            return MatchInto<Declaration>.On(declaration).With(m => m
                .Is<FunctionDeclarationSyntax>(f =>
                {
                    var access = BuildAccessModifier(f.AccessModifier);
                    var name = f.Name.Value;
                    var parameters = Build(f.ParameterList);
                    var returnType = Build(f.ReturnType);
                    var body = Build(f.Body);
                    return new FunctionDeclaration(f, annotations.Diagnostics(f), access, name, parameters, returnType, body);//TODO pass them into the constructor
                }));
        }

        private static AccessLevel BuildAccessModifier(Token accessModifier)
        {
            switch (accessModifier.Kind)
            {
                case TokenKind.PublicKeyword:
                    return AccessLevel.Public;
                default:
                    throw new NotSupportedException(accessModifier.ToString());
            }
        }

        private IEnumerable<Parameter> Build(ParameterListSyntax parameterList)
        {
            return parameterList.Parameters
                .Select(p => new Parameter(p, annotations.Diagnostics(p), p.VarKeyword != null, p.Identifier.Value, Build(p.Type)))
                .ToList();
        }

        private TypeName Build(TypeSyntax type)
        {
            return new TypeName(type, annotations.Diagnostics(type), annotations.Type(type));
        }

        private Block Build(BlockSyntax block)
        {
            var statements = block.Statements.Select(Build);
            return new Block(block, annotations.Diagnostics(block), statements);
        }

        private Statement Build(StatementSyntax statement)
        {
            return MatchInto<Statement>.On(statement).With(m => m
                .Is<BlockSyntax>(Build)
                .Is<ExpressionStatementSyntax>(es => new ExpressionStatement(es, annotations.Diagnostics(statement), Build(es.Expression)))
            );
        }

        private Expression Build(ExpressionSyntax expression)
        {
            return MatchInto<Expression>.On(expression).With(m => m
                .Is<BinaryOperatorExpressionSyntax>(Build)
                .Is<IdentifierNameSyntax>(i => new VariableExpression(i, annotations.Diagnostics(expression), (VariableName)annotations.Name(i), annotations.Type(i)))
                .Is<ReturnExpressionSyntax>(r => new ReturnExpression(r, annotations.Diagnostics(expression), Build(r.Expression)))
            );
        }

        private BinaryOperatorExpression Build(BinaryOperatorExpressionSyntax expression)
        {
            var leftOperand = Build(expression.LeftOperand);
            var rightOperand = Build(expression.RightOperand);
            switch (expression.Operator.Kind)
            {
                case TokenKind.Plus:
                    return new AddExpression(expression, annotations.Diagnostics(expression), leftOperand, rightOperand, annotations.Type(expression));
                default:
                    throw new InvalidEnumArgumentException(expression.Operator.Kind.ToString());
            }
        }
    }
}
