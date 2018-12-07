using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    public class ControlFlowAnalyzer
    {
        public static void BuildGraphs(IEnumerable<DeclarationSyntax> declarations)
        {
            var visitor = new GetFunctionDeclarationsVisitor();
            visitor.VisitDeclarations(declarations);
            foreach (var function in visitor.FunctionDeclarations.Where(ShouldBuildGraph))
            {
                var builder = new ControlFlowAnalyzer();
                builder.BuildGraph(function);
            }
        }

        private static bool ShouldBuildGraph(FunctionDeclarationSyntax function)
        {
            return function.Body != null // It is not abstract
                   && function.GenericParameters == null // It is not generic, generic functions need monomorphized
                   && !function.Poisoned; // There were errors, we may not be able to make a control flow graph, so don't try
        }

        private readonly ControlFlowGraphBuilder graph = new ControlFlowGraphBuilder();

        private void BuildGraph(FunctionDeclarationSyntax function)
        {
            // Temp Variable for return
            // TODO don't emit temp variables for unused parameters
            if (function is ConstructorDeclarationSyntax constructor)
                graph.AddParameter(true, constructor.SelfParameterType, SpecialName.Self);
            else
                graph.Let(function.ReturnType.Resolved());
            foreach (var parameter in function.Parameters.Where(p => !p.Unused))
                graph.AddParameter(parameter.MutableBinding, parameter.Type.Resolved(), parameter.Name.UnqualifiedName);

            foreach (var statement in function.Body.Statements)
                ConvertToStatement(statement);

            // Generate the implicit return statement
            if (graph.CurrentBlockNumber == 0)
                graph.AddBlockReturn();

            function.ControlFlow = graph.Build();
        }

        private void ConvertToStatement(StatementSyntax statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    Value value = null;
                    if (variableDeclaration.Initializer != null)
                        value = ConvertToValue(variableDeclaration.Initializer);

                    var variable = graph.AddVariable(variableDeclaration.MutableBinding,
                        variableDeclaration.Type,
                        variableDeclaration.Name.UnqualifiedName);
                    if (value != null) graph.AddAssignment(variable.Reference, value);
                    break;

                case ExpressionSyntax expression:
                    ConvertToStatement(expression);
                    break;

                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        private static bool IsOwned(VariableDeclarationStatementSyntax declaration)
        {
            if (declaration.Type is LifetimeType type)
                return type.IsOwned;

            return false;
        }

        private void ConvertToStatement(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case IdentifierNameSyntax _:
                    // Ignore, reading from variable does nothing.
                    break;
                case UnaryExpressionSyntax _:
                case BinaryExpressionSyntax _:
                case InvocationSyntax _:
                    ConvertToAssignmentStatement(expression);
                    break;
                case ReturnExpressionSyntax returnExpression:
                    if (returnExpression.ReturnValue != null)
                        graph.AddAssignment(graph.ReturnVariable.Reference, ConvertToValue(returnExpression.ReturnValue));

                    graph.AddBlockReturn();
                    break;
                case BlockSyntax block:
                    foreach (var statementInBlock in block.Statements)
                        ConvertToStatement(statementInBlock);

                    // Now we need to delete any owned variables
                    foreach (var variableDeclaration in block.Statements.OfType<VariableDeclarationStatementSyntax>().Where(IsOwned))
                        graph.AddDelete(graph.VariableFor(variableDeclaration.Name.UnqualifiedName), new TextSpan(block.Span.End, 0));
                    break;
                case ForeachExpressionSyntax @foreach:
                    // TODO actually convert the expression
                    break;
                case WhileExpressionSyntax @while:
                    // TODO actually convert the expression
                    break;
                case LoopExpressionSyntax loop:
                    // TODO actually convert the expression
                    break;
                case UnsafeExpressionSyntax unsafeExpression:
                    ConvertToStatement(unsafeExpression.Expression);
                    break;
                case AssignmentExpressionSyntax assignmentExpression:
                {
                    var place = ConvertToPlace(assignmentExpression.LeftOperand);
                    var value = ConvertToValue(assignmentExpression.RightOperand);
                    graph.AddAssignment(place, value);
                    break;
                }
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void ConvertToAssignmentStatement(ExpressionSyntax expression)
        {
            var value = ConvertToValue(expression);
            if (expression.Type is VoidType) graph.AddAction(value);
            else
            {
                var tempVariable = graph.Let(expression.Type.AssertResolved());
                graph.AddAssignment(tempVariable.Reference, value);
            }
        }

        private Value ConvertToValue(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case NewObjectExpressionSyntax newObjectExpression:
                    var args = newObjectExpression.Arguments.Select(a => ConvertToOperand(a.Value)).ToFixedList();
                    return new ConstructorCall((ObjectType)newObjectExpression.Type, args);
                case IdentifierNameSyntax identifier:
                {
                    var symbol = identifier.ReferencedSymbol;
                    switch (symbol)
                    {
                        case VariableDeclarationStatementSyntax _:
                        case ParameterSyntax _:
                            return new CopyPlace(
                                graph.VariableFor(symbol.FullName.UnqualifiedName));
                        default:
                            return new DeclaredValue(symbol.FullName);
                    }
                }
                case UnaryExpressionSyntax unaryExpression:
                    return ConvertUnaryExpressionToValue(unaryExpression);
                case BinaryExpressionSyntax binaryExpression:
                    return ConvertBinaryExpressionToValue(binaryExpression);
                case IntegerLiteralExpressionSyntax _:
                    throw new InvalidOperationException("Integer literals should have an implicit conversion around them");
                case StringLiteralExpressionSyntax _:
                    throw new InvalidOperationException("String literals should have an implicit conversion around them");
                case BoolLiteralExpressionSyntax boolLiteral:
                    return new BooleanConstant(boolLiteral.Value);
                case ImplicitNumericConversionExpression implicitNumericConversion:
                    if (implicitNumericConversion.Expression.Type.AssertResolved() is IntegerConstantType constantType)
                        return new IntegerConstant(constantType.Value, implicitNumericConversion.Type.AssertResolved());
                    else
                        throw new NotImplementedException();
                case IfExpressionSyntax ifExpression:
                    // TODO assign the result into the temp, branch and execute then or else, assign result
                    throw new NotImplementedException();
                case UnsafeExpressionSyntax unsafeExpression:
                    return ConvertToValue(unsafeExpression.Expression);
                case ImplicitLiteralConversionExpression implicitLiteralConversion:
                {
                    var conversionFunction = implicitLiteralConversion.ConversionFunction.FullName;
                    var literal = (StringLiteralExpressionSyntax)implicitLiteralConversion.Expression;
                    var constantLength = Utf8BytesConstant.Encoding.GetByteCount(literal.Value);
                    var sizeArgument = new IntegerConstant(constantLength, DataType.Size);
                    var bytesArgument = new Utf8BytesConstant(literal.Value);
                    return new FunctionCall(conversionFunction, sizeArgument, bytesArgument);
                }
                case InvocationSyntax invocation:
                    return ConvertInvocationToValue(invocation);
                case MemberAccessExpressionSyntax memberAccess:
                {
                    var value = ConvertToOperand(memberAccess.Expression);
                    var symbol = memberAccess.ReferencedSymbol;
                    if (symbol is IAccessorSymbol accessor)
                        return new VirtualFunctionCall(accessor.PropertyName.UnqualifiedName, value);

                    return new FieldAccessValue(value,
                        memberAccess.ReferencedSymbol.FullName);
                }
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private Operand ConvertToOperand(ExpressionSyntax expression)
        {
            var value = ConvertToValue(expression);
            if (value is Operand operand) return operand;
            var tempVariable = graph.Let(expression.Type.AssertResolved());
            graph.AddAssignment(tempVariable.Reference, value);
            return new CopyPlace(tempVariable.Reference);
        }

        private Value ConvertBinaryExpressionToValue(BinaryExpressionSyntax expression)
        {
            switch (expression.Operator)
            {
                case BinaryOperator.Plus:
                case BinaryOperator.Minus:
                case BinaryOperator.Asterisk:
                case BinaryOperator.Slash:
                case BinaryOperator.EqualsEquals:
                case BinaryOperator.NotEqual:
                case BinaryOperator.LessThan:
                case BinaryOperator.LessThanOrEqual:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.GreaterThanOrEqual:
                {
                    // TODO handle calls to overloaded operators
                    var leftOperand = ConvertToOperand(expression.LeftOperand);
                    var rightOperand = ConvertToOperand(expression.RightOperand);
                    // What matters is the type we are operating on, for comparisons, that is different than the result type which is bool
                    var operandType = (SimpleType)expression.LeftOperand.Type;
                    return new BinaryOperation(leftOperand, expression.Operator, rightOperand, operandType);
                }
                case BinaryOperator.And:
                case BinaryOperator.Or:
                {
                    // TODO handle calls to overloaded operators
                    // TODO handle short circuiting if needed
                    var leftOperand = ConvertToOperand(expression.LeftOperand);
                    var rightOperand = ConvertToOperand(expression.RightOperand);
                    return new BinaryOperation(leftOperand, expression.Operator, rightOperand, (SimpleType)expression.Type);
                }
                default:
                    throw NonExhaustiveMatchException.ForEnum(expression.Operator);
            }
        }

        private Value ConvertUnaryExpressionToValue(UnaryExpressionSyntax expression)
        {
            switch (expression.Operator)
            {
                case UnaryOperator.Not:
                    var operand = ConvertToOperand(expression.Operand);
                    return new UnaryOperation(expression.Operator, operand);
                case UnaryOperator.Plus:
                    // This is a no-op
                    return ConvertToValue(expression.Operand);
                default:
                    throw NonExhaustiveMatchException.ForEnum(expression.Operator);
            }
        }

        private Value ConvertInvocationToValue(InvocationSyntax invocation)
        {
            switch (invocation.Callee)
            {
                case IdentifierNameSyntax identifier:
                {
                    var symbol = identifier.ReferencedSymbol;
                    var arguments = invocation.Arguments
                        .Select(a => ConvertToOperand(a.Value)).ToList();
                    return new FunctionCall(symbol.FullName, arguments);
                }
                case MemberAccessExpressionSyntax memberAccess:
                {
                    var self = ConvertToOperand(memberAccess.Expression);
                    var arguments = invocation.Arguments
                        .Select(a => ConvertToOperand(a.Value)).ToList();
                    var symbol = memberAccess.ReferencedSymbol;
                    switch (symbol)
                    {
                        case IFunctionSymbol function:
                            switch (memberAccess.Expression.Type)
                            {
                                case SimpleType _:
                                    // case StructType _:
                                    // Full name because this isn't a member
                                    return new FunctionCall(function.FullName, self, arguments);
                                default:
                                    return new VirtualFunctionCall(function.FullName.UnqualifiedName, self, arguments);
                            }
                        default:
                            throw NonExhaustiveMatchException.For(symbol);
                    }
                }
                default:
                    throw NonExhaustiveMatchException.For(invocation.Callee);
            }
        }

        private Place ConvertToPlace(ExpressionSyntax value)
        {
            switch (value)
            {
                case IdentifierNameSyntax identifier:
                    // TODO what if this isn't just a variable?
                    return graph.VariableFor(identifier.ReferencedSymbol.FullName.UnqualifiedName);
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
