using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
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
            graph.Let(function.ReturnType.Resolved());
            foreach (var parameter in function.Parameters)
                graph.AddParameter(parameter.MutableBinding, parameter.Type.Resolved(), parameter.Name.UnqualifiedName);

            foreach (var statement in function.Body.NotNull().Statements)
                ConvertStatementSyntaxToStatement(statement);

            // Generate the implicit return statement
            if (graph.CurrentBlockNumber == 0)
                graph.AddBlockReturn();

            function.ControlFlow = graph.Build();
        }

        private void ConvertStatementSyntaxToStatement([NotNull] StatementSyntax statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    var variable = graph.AddVariable(variableDeclaration.MutableBinding,
                        variableDeclaration.Type.Fulfilled(),
                        variableDeclaration.Name.UnqualifiedName);
                    if (variableDeclaration.Initializer != null)
                        ConvertToAssignmentStatement(variable.Reference, variableDeclaration.Initializer);
                    break;

                case ExpressionSyntax expression:
                    ConvertExpressionSyntaxToStatement(expression);
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

        private void ConvertExpressionSyntaxToStatement([NotNull] ExpressionSyntax expression)
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
                            var temp = graph.Let(binaryOperatorExpression.Type.Resolved());
                            ConvertToAssignmentStatement(temp.Reference, expression);
                            break;
                    }
                    break;
                case ReturnExpressionSyntax returnExpression:
                    if (returnExpression.ReturnValue != null)
                        ConvertToAssignmentStatement(graph.ReturnVariable.Reference, returnExpression.ReturnValue);
                    graph.AddBlockReturn();
                    break;
                case BlockSyntax block:
                    foreach (var statementInBlock in block.Statements)
                        ConvertStatementSyntaxToStatement(statementInBlock);

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
                    ConvertExpressionSyntaxToStatement(unsafeExpression.Expression);
                    break;
                case InvocationSyntax invocation:
                    // TODO actually convert the expression
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void ConvertToAssignmentStatement(
            [NotNull] Place place,
            [NotNull] ExpressionSyntax value)
        {
            switch (value)
            {
                //case NewObjectExpressionAnalysis newObjectExpression:
                //    var args = newObjectExpression.Arguments.Select(a => ConvertToLValue(a.Value));
                //    statements.Add(new NewObjectStatement(place, newObjectExpression.Type.AssertResolved(), args));
                //    break;
                case IdentifierNameSyntax identifier:
                    graph.AddAssignment(place, new CopyPlace(graph.VariableFor(identifier.Name)));
                    break;
                case BinaryExpressionSyntax binaryOperator:
                    ConvertOperator(place, binaryOperator);
                    break;
                case IntegerLiteralExpressionSyntax _:
                    throw new InvalidOperationException("Integer literals should have an implicit conversion around them");
                case StringLiteralExpressionSyntax _:
                    throw new InvalidOperationException("String literals should have an implicit conversion around them");
                case ImplicitNumericConversionExpression implicitNumericConversion:
                    if (implicitNumericConversion.Expression.Type.Resolved() is IntegerConstantType constantType)
                    {
                        var constant = new IntegerConstant(constantType.Value, implicitNumericConversion.Type.Resolved());
                        graph.AddAssignment(place, constant);
                    }
                    else
                        throw new NotImplementedException();
                    break;
                case IfExpressionSyntax ifExpression:
                    ConvertExpressionSyntaxToStatement(ifExpression.Condition);
                    // TODO assign the result into the temp, branch and execute then or else, assign result
                    break;
                case UnsafeExpressionSyntax unsafeExpression:
                    ConvertExpressionSyntaxToStatement(unsafeExpression.Expression);
                    break;
                case ImplicitLiteralConversionExpression implicitLiteralConversion:
                {
                    var conversionFunction = implicitLiteralConversion.ConversionFunction.FullName;
                    var literal = (StringLiteralExpressionSyntax)implicitLiteralConversion.Expression;
                    var constantLength = Utf8BytesConstant.Encoding.GetByteCount(literal.Value);
                    var sizeArgument = new IntegerConstant(constantLength, DataType.Size);
                    var bytesArgument = new Utf8BytesConstant(literal.Value);
                    graph.AddCall(place, conversionFunction, sizeArgument, bytesArgument);
                }
                break;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private void ConvertOperator(
            [NotNull] Place lvalue,
            [NotNull] BinaryExpressionSyntax binary)
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
                    // TODO generate the correct statement
                    break;
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
