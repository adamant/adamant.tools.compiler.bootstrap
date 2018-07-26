using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticTreeBuilder
    {
        private readonly SyntaxAnnotation<DataType> typeAnnotations;

        public SemanticTreeBuilder(SyntaxAnnotation<DataType> typeAnnotations)
        {
            this.typeAnnotations = typeAnnotations;
        }

        public CompilationUnit Build(SyntaxTree<CompilationUnitSyntax> syntaxTree)
        {
            var declarations = syntaxTree.Root.Declarations.Select(d => Build(d, typeAnnotations));
            return new CompilationUnit(syntaxTree.Root, declarations);
        }

        private Declaration Build(DeclarationSyntax declaration, SyntaxAnnotation<DataType> typeAnnotations)
        {
            return MatchInto<Declaration>.On(declaration).With(m => m
                .Is<FunctionDeclarationSyntax>(f =>
                {
                    var access = BuildAccessModifier(f.AccessModifier, typeAnnotations);
                    var name = f.Name.Value;
                    var parameters = Build(f.Parameters, typeAnnotations);
                    var returnType = Build(f.ReturnType, typeAnnotations);
                    var body = Build(f.Body, typeAnnotations);
                    return new FunctionDeclaration(f, access, name, parameters, returnType, body);//TODO pass them into the constructor
                }));
        }

        private AccessLevel BuildAccessModifier(Token accessModifier, SyntaxAnnotation<DataType> typeAnnotations)
        {
            switch (accessModifier.Kind)
            {
                case TokenKind.PublicKeyword:
                    return AccessLevel.Public;
                default:
                    throw new NotSupportedException(accessModifier.ToString());
            }
        }

        private IEnumerable<Parameter> Build(ParameterListSyntax parameterList, SyntaxAnnotation<DataType> typeAnnotations)
        {
            return parameterList.Parameters
                .Select(p => new Parameter(p, p.VarKeyword != null, p.Identifier.Value, Build(p.Type, typeAnnotations)))
                .ToList();
        }

        private TypeName Build(TypeSyntax type, SyntaxAnnotation<DataType> typeAnnotations)
        {
            return new TypeName(type, typeAnnotations[type]);
        }

        private Block Build(BlockSyntax block, SyntaxAnnotation<DataType> typeAnnotations)
        {
            var statements = block.Statements.Select(s => Build(s, typeAnnotations));
            return new Block(block, statements);
        }

        private Statement Build(StatementSyntax statement, SyntaxAnnotation<DataType> typeAnnotations)
        {
            return MatchInto<Statement>.On(statement).With(m => m
                .Is<BlockSyntax>(b => Build(b, typeAnnotations))
                .Is<ExpressionStatementSyntax>(es => new ExpressionStatement(es, Build(es.Expression, typeAnnotations)))
            );
        }

        private Expression Build(ExpressionSyntax expression, SyntaxAnnotation<DataType> typeAnnotations)
        {
            return MatchInto<Expression>.On(expression).With(m => m
                .Is<BinaryOperatorExpressionSyntax>(op => Build(op, typeAnnotations))
                .Is<IdentifierNameSyntax>(i => new VariableExpression(i))
                .Is<ReturnExpressionSyntax>(r => new ReturnExpression(r, Build(r.Expression, typeAnnotations)))
            );
        }

        private BinaryOperatorExpression Build(BinaryOperatorExpressionSyntax expression, SyntaxAnnotation<DataType> typeAnnotations)
        {
            var leftOperand = Build(expression.LeftOperand, typeAnnotations);
            var rightOperand = Build(expression.RightOperand, typeAnnotations);
            switch (expression.Operator.Kind)
            {
                case TokenKind.Plus:
                    return new AddExpression(expression, leftOperand, rightOperand);
                default:
                    throw new InvalidEnumArgumentException(expression.Operator.Kind.ToString());
            }

        }
    }
}
