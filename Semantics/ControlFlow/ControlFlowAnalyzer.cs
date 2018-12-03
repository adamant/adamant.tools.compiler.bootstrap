using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    public class ControlFlowAnalyzer
    {
        public static void BuildGraphs([NotNull, ItemNotNull] IEnumerable<DeclarationSyntax> declarations)
        {
            var visitor = new GetFunctionDeclarationsVisitor();
            visitor.VisitDeclarations(declarations);
            foreach (var function in visitor.FunctionDeclarations.Where(ShouldBuildGraph))
            {
                var builder = new ControlFlowAnalyzer();
                builder.BuildGraph(function);
            }
        }

        private static bool ShouldBuildGraph([NotNull] FunctionDeclarationSyntax function)
        {
            return function.Body != null // It is not abstract
                   && function.GenericParameters == null // It is not generic, generic functions need monomorphized
                   && !function.Poisoned; // There were errors, we may not be able to make a control flow graph, so don't try
        }

        [NotNull] private readonly ControlFlowGraphBuilder graph = new ControlFlowGraphBuilder();

        private void BuildGraph([NotNull] FunctionDeclarationSyntax function)
        {
            // Temp Variable for return
            if (function is ConstructorDeclarationSyntax constructor)
                graph.AddParameter(true, constructor.ReturnType.Resolved(), SpecialName.Self);
            else
                graph.Let(function.ReturnType.Resolved());
            foreach (var parameter in function.Parameters.Where(p => !p.Unused))
                graph.AddParameter(parameter.MutableBinding, parameter.Type.Resolved(), parameter.Name.UnqualifiedName);

            foreach (var statement in function.Body.NotNull().Statements)
                ConvertToStatement(statement);

            // Generate the implicit return statement
            if (graph.CurrentBlockNumber == 0)
                graph.AddBlockReturn();

            function.ControlFlow = graph.Build();
        }

        private void ConvertToStatement([NotNull] StatementSyntax statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    Value value = null;
                    if (variableDeclaration.Initializer != null)
                        value = ConvertToValue(variableDeclaration.Initializer);

                    var variable = graph.AddVariable(variableDeclaration.MutableBinding,
                        variableDeclaration.Type.Fulfilled(),
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

        private static bool IsOwned([NotNull] VariableDeclarationStatementSyntax declaration)
        {
            if (declaration.Type.Resolved() is LifetimeType type)
                return type.IsOwned;

            return false;
        }

        private void ConvertToStatement([NotNull] ExpressionSyntax expression)
        {
            switch (expression)
            {
                case IdentifierNameSyntax _:
                    // Ignore, reading from variable does nothing.
                    break;
                case BinaryExpressionSyntax binaryOperatorExpression:
                    switch (binaryOperatorExpression.Operator)
                    {
                        //case BinaryOperator. EqualsToken _:
                        //    var lvalue = ConvertToLValue(binaryOperatorExpression.LeftOperand);
                        //    ConvertToAssignmentStatement(lvalue, binaryOperatorExpression.RightOperand, statements);
                        //    break;
                        default:
                            // Could be side effects possibly.
                            ConvertToAssignmentStatement(expression);
                            break;
                    }
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
                case InvocationSyntax invocation:
                    ConvertToAssignmentStatement(invocation);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        [NotNull]
        private Place ConvertToAssignmentStatement([NotNull] ExpressionSyntax expression)
        {
            var value = ConvertToValue(expression);
            var place = graph.Let(expression.Type.Resolved());
            graph.AddAssignment(place.Reference, value);
            return place.Reference;
        }

        [NotNull]
        private Value ConvertToValue([NotNull] ExpressionSyntax expression)
        {
            switch (expression)
            {
                //case NewObjectExpressionAnalysis newObjectExpression:
                //    var args = newObjectExpression.Arguments.Select(a => ConvertToLValue(a.Value));
                //    statements.Add(new NewObjectStatement(place, newObjectExpression.Type.AssertResolved(), args));
                //    break;
                case IdentifierNameSyntax identifier:
                    var symbol = identifier.ReferencedSymbols.NotNull().Single().NotNull();
                    switch (symbol)
                    {
                        case VariableDeclarationStatementSyntax _:
                        case ParameterSyntax _:
                            return new CopyPlace(graph.VariableFor(symbol.FullName.UnqualifiedName));
                        default:
                            return new DeclaredValue(symbol.FullName);
                    }
                case BinaryExpressionSyntax binaryExpression:
                    return ConvertBinaryExpressionToValue(binaryExpression);
                case IntegerLiteralExpressionSyntax _:
                    throw new InvalidOperationException("Integer literals should have an implicit conversion around them");
                case StringLiteralExpressionSyntax _:
                    throw new InvalidOperationException("String literals should have an implicit conversion around them");
                case ImplicitNumericConversionExpression implicitNumericConversion:
                    if (implicitNumericConversion.Expression.Type.Resolved() is IntegerConstantType constantType)
                        return new IntegerConstant(constantType.Value, implicitNumericConversion.Type.Resolved());
                    else
                        throw new NotImplementedException();
                case IfExpressionSyntax ifExpression:
                    //ConvertToStatement(ifExpression.Condition);
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
                {
                    var temp = ConvertToOperand(invocation.Callee);
                    var arguments = invocation.Arguments.Select(a => ConvertToOperand(a.Value.NotNull())).ToList();
                    return new FunctionCall(SpecialName.Any, arguments);
                }
                case MemberAccessExpressionSyntax memberAccess:
                {
                    var value = ConvertToOperand(memberAccess.Expression.NotNull());
                    return new MemberAccessValue(value,
                        memberAccess.ReferencedSymbols.NotNull().Single().NotNull().FullName);
                }
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        [NotNull]
        private Operand ConvertToOperand([NotNull] ExpressionSyntax expression)
        {
            var value = ConvertToValue(expression);
            if (value is Operand operand) return operand;
            var temp = graph.Let(expression.Type.Resolved());
            graph.AddAssignment(temp.Reference, value);
            return new CopyPlace(temp.Reference);
        }

        [NotNull]
        private Value ConvertBinaryExpressionToValue([NotNull] BinaryExpressionSyntax binary)
        {
            switch (binary.Operator)
            {
                //case PlusToken _:
                //    currentBlock.Add(new AddStatement(lvalue,
                //        ConvertToLValue(cfg, binaryOperator.LeftOperand),
                //        ConvertToLValue(cfg, binaryOperator.RightOperand)));
                //    break;
                case BinaryOperator.LessThan:
                case BinaryOperator.LessThanOrEqual:
                case BinaryOperator.GreaterThan:
                case BinaryOperator.GreaterThanOrEqual:
                    // TODO generate the correct value
                    throw new NotImplementedException();
                //case IAsKeywordToken _:
                //    ConvertExpressionAnalysisToStatement(binaryOperator.LeftOperand, statements);
                //    break;
                default:
                    throw NonExhaustiveMatchException.For(binary.Operator);
            }
        }

        [NotNull]
        private Place ConvertToLValue([NotNull] ExpressionSyntax value)
        {
            Requires.NotNull(nameof(value), value);
            switch (value)
            {
                case IdentifierNameSyntax identifier:
                    return graph.VariableFor(identifier.Name);
                //case VariableExpression variableExpression:
                //    return LookupVariable(variableExpression.Name);
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
