using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Parts;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    /// Note: Naming this `Syntax` would conflict with the `Syntax` namespace
    public abstract class SyntaxNode : ISyntaxNodeOrToken
    {
        [NotNull]
        [ItemNotNull]
        public IEnumerable<SyntaxNode> DescendantsAndSelf()
        {
            var nodes = new Stack<SyntaxNode>();
            nodes.Push(this);
            while (nodes.TryPop(out var node))
            {
                switch (node)
                {
                    case null:
                        // If one of the descendants was null, not present,
                        // just skip it.
                        continue;
                    case PackageSyntax package:
                        nodes.PushRange(package.CompilationUnits.Reverse());
                        break;
                    case CompilationUnitSyntax compilationUnit:
                        nodes.PushRange(compilationUnit.Declarations.Reverse());
                        nodes.PushRange(compilationUnit.UsingDirectives.Reverse());
                        nodes.Push(compilationUnit.Namespace);
                        break;
                    case ClassDeclarationSyntax @class:
                        nodes.PushRange(@class.Members.Reverse());
                        nodes.Push(@class.AccessModifier);
                        break;
                    case ParameterSyntax parameter:
                        nodes.Push(parameter.TypeExpression);
                        break;
                    case FunctionDeclarationSyntax function:
                        nodes.Push(function.Body);
                        nodes.Push(function.ReturnTypeExpression);
                        nodes.PushRange(function.ParametersList.Nodes().Reverse());
                        nodes.Push(function.AccessModifier);
                        break;
                    case LifetimeTypeSyntax lifetimeType:
                        nodes.Push(lifetimeType.TypeName);
                        break;
                    case BlockExpressionSyntax blockStatement:
                        nodes.PushRange(blockStatement.Statements.Reverse());
                        break;
                    case ReturnExpressionSyntax returnExpression:
                        nodes.Push(returnExpression.ReturnValue);
                        break;
                    case ExpressionStatementSyntax expressionStatement:
                        nodes.Push(expressionStatement.Expression);
                        break;
                    case BinaryOperatorExpressionSyntax binaryOperator:
                        nodes.Push(binaryOperator.RightOperand);
                        nodes.Push(binaryOperator.LeftOperand);
                        break;
                    case VariableDeclarationStatementSyntax variableDeclaration:
                        nodes.Push(variableDeclaration.Initializer);
                        nodes.Push(variableDeclaration.TypeExpression);
                        break;
                    case UnaryOperatorExpressionSyntax unaryOperator:
                        nodes.Push(unaryOperator.Operand);
                        break;
                    case NewObjectExpressionSyntax newObject:
                        nodes.PushRange(newObject.ArgumentList.Nodes().Reverse());
                        nodes.Push(newObject.Constructor);
                        break;
                    case AccessModifierSyntax _:
                    case PrimitiveTypeSyntax _:
                    case IdentifierNameSyntax _:
                        // No child nodes
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(node);
                }
                yield return node;
            }
        }
    }
}
