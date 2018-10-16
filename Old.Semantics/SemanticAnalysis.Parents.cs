using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics
{
    internal partial class SemanticAnalysis
    {
        private const string ParentAttribute = "Parent";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackageSyntax Parent(CompilationUnitSyntax s) => Parent<PackageSyntax>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public CompilationUnitSyntax Parent([NotNull] FunctionDeclarationSyntax s) => Parent<CompilationUnitSyntax>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CompilationUnitSyntax Parent(ClassDeclarationSyntax s) => Parent<CompilationUnitSyntax>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CompilationUnitSyntax Parent(EnumStructDeclarationSyntax s) => Parent<CompilationUnitSyntax>(s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SyntaxNode Parent([NotNull] SyntaxNode s)
        {
            return attributes.Get<SyntaxNode>(s, ParentAttribute);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CanBeNull]
        private TSyntax Parent<TSyntax>([NotNull] SyntaxNode syntax)
            where TSyntax : SyntaxNode
        {
            return attributes.Get<TSyntax>(syntax, ParentAttribute);
        }

        private void AddParentAttribute([NotNull] PackageSyntax package)
        {
            AddParentAttribute(package, null);
        }

        private void AddParentAttribute([CanBeNull] SyntaxNode syntax, [CanBeNull] SyntaxNode parent)
        {
            if (syntax == null)
                return;

            attributes.GetOrAdd<SyntaxNode>(syntax, ParentAttribute, new Lazy<object>(parent));
            switch (syntax)
            {
                case PackageSyntax package:
                    AddAllParentAttributes(package.CompilationUnits, syntax);
                    break;
                case CompilationUnitSyntax compilationUnit:
                    AddParentAttribute(compilationUnit.Namespace, syntax);
                    AddAllParentAttributes(compilationUnit.UsingDirectives, syntax);
                    AddAllParentAttributes(compilationUnit.Declarations, syntax);
                    break;
                case ClassDeclarationSyntax classDeclaration:
                    AddParentAttribute(classDeclaration.AccessModifier, syntax);
                    AddAllParentAttributes(classDeclaration.Members, syntax);
                    break;
                case ParameterSyntax parameter:
                    AddParentAttribute(parameter.TypeExpression, syntax);
                    break;
                case FunctionDeclarationSyntax functionDeclaration:
                    AddParentAttribute(functionDeclaration.AccessModifier, syntax);
                    AddAllParentAttributes(functionDeclaration.Parameters.Nodes(), syntax);
                    AddParentAttribute(functionDeclaration.ReturnTypeExpression, syntax);
                    AddParentAttribute(functionDeclaration.Body, syntax);
                    break;
                case LifetimeTypeSyntax lifetimeType:
                    AddParentAttribute(lifetimeType.TypeName, syntax);
                    break;
                case BlockExpressionSyntax blockStatement:
                    AddAllParentAttributes(blockStatement.Statements, syntax);
                    break;
                case ReturnExpressionSyntax returnExpression:
                    AddParentAttribute(returnExpression.Expression, syntax);
                    break;
                case ExpressionStatementSyntax expressionStatement:
                    AddParentAttribute(expressionStatement.Expression, syntax);
                    break;
                case BinaryOperatorExpressionSyntax binaryOperator:
                    AddParentAttribute(binaryOperator.LeftOperand, syntax);
                    AddParentAttribute(binaryOperator.RightOperand, syntax);
                    break;
                case VariableDeclarationStatementSyntax variableDeclaration:
                    AddParentAttribute(variableDeclaration.TypeExpression, syntax);
                    AddParentAttribute(variableDeclaration.Initializer, syntax);
                    break;
                case UnaryOperatorExpressionSyntax unaryOperator:
                    AddParentAttribute(unaryOperator.Operand, syntax);
                    break;
                case NewObjectExpressionSyntax newObjectExpression:
                    AddParentAttribute(newObjectExpression.Type, syntax);
                    AddAllParentAttributes(newObjectExpression.Arguments.Nodes(), syntax);
                    break;
                case AccessModifierSyntax _:
                case PrimitiveTypeSyntax _:
                case IdentifierNameSyntax _:
                    // No child nodes
                    break;
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }

        private void AddAllParentAttributes<T>([NotNull][ItemCanBeNull] IEnumerable<T> list, [CanBeNull] SyntaxNode parent)
            where T : SyntaxNode
        {
            foreach (var node in list)
                AddParentAttribute(node, parent);
        }
    }
}
