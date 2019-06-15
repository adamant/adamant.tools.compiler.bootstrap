using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
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
        public static void BuildGraphs(IEnumerable<MemberDeclarationSyntax> declarations)
        {
            foreach (var function in declarations.OfType<FunctionDeclarationSyntax>().Where(ShouldBuildGraph))
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

        /// <summary>
        /// Note this is only the variables in the current scope, not in nested scopes.
        /// </summary>
        private List<Variable> variablesInCurrentScope;

        /// <summary>
        /// The block we are currently adding statements to
        /// </summary>
        private BlockBuilder currentBlock;

        /// <summary>
        /// The block that a break statement should go to
        /// </summary>
        private BlockBuilder breakToBlock;


        private void BuildGraph(FunctionDeclarationSyntax function)
        {
            variablesInCurrentScope = new List<Variable>();

            // Temp Variable for return
            if (function is ConstructorDeclarationSyntax constructor)
                graph.AddParameter(true, constructor.SelfParameterType, SpecialName.Self);
            else
                graph.Let(function.ReturnType.Resolved());

            // TODO don't emit temp variables for unused parameters
            foreach (var parameter in function.Parameters.Where(p => !p.Unused))
                VariableInScope(graph.AddParameter(parameter.MutableBinding, parameter.Type.Resolved(), parameter.Name.UnqualifiedName));

            currentBlock = graph.NewBlock();
            breakToBlock = null;
            foreach (var statement in function.Body.Statements)
                currentBlock = ConvertToStatement(statement);

            // Generate the implicit return statement
            if (currentBlock != null && !currentBlock.IsTerminated)
                currentBlock.AddReturn();

            function.ControlFlow = graph.Build();
        }

        private void VariableInScope(LocalVariableDeclaration declaration)
        {
            // Only track owned references
            if (!IsOwned(declaration.Type)) return;

            variablesInCurrentScope.Add(declaration.Variable);
        }

        private BlockBuilder ConvertToStatement(StatementSyntax statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                {
                    var variable = graph.AddVariable(variableDeclaration.MutableBinding,
                        variableDeclaration.Type, variableDeclaration.Name.UnqualifiedName);
                    if (variableDeclaration.Initializer != null)
                    {
                        var value = ConvertToValue(variableDeclaration.Initializer);
                        currentBlock.AddAssignment(variable.AssignReference(variableDeclaration.Initializer.Span), value, variableDeclaration.Initializer.Span);
                    }
                    return currentBlock;
                }
                case ExpressionSyntax expression:
                {
                    if (expression.Type is VoidType || expression.Type is NeverType)
                        currentBlock = ConvertExpressionToStatement(expression);
                    else
                    {
                        var tempVariable = graph.Let(expression.Type.AssertResolved());
                        var value = ConvertToValue(expression);
                        currentBlock.AddAssignment(tempVariable.AssignReference(expression.Span), value, expression.Span);
                    }

                    return currentBlock;
                }
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        private static bool IsOwned(DataType type)
        {
            if (type is ReferenceType referenceType)
                return referenceType.IsOwned;

            return false;
        }

        /// <summary>
        /// Converts an expression of type `void` or `never` to a statement
        /// </summary>
        private BlockBuilder ConvertExpressionToStatement(ExpressionSyntax expression)
        {
            // expression m
            switch (expression)
            {
                case UnaryExpressionSyntax _:
                case BinaryExpressionSyntax _:
                    throw new NotImplementedException();
                case InvocationSyntax invocation:
                    //return ConvertToAssignmentStatement(currentBlock, expression);
                    currentBlock.AddAction(ConvertInvocationToValue(invocation), invocation.Span);
                    return currentBlock;
                case ReturnExpressionSyntax returnExpression:
                    if (returnExpression.ReturnValue != null)
                        currentBlock.AddAssignment(graph.ReturnVariable.AssignReference(returnExpression.ReturnValue.Span),
                            ConvertToValue(returnExpression.ReturnValue),
                            returnExpression.ReturnValue.Span);

                    currentBlock.AddReturn();

                    // There is no exit from a return block, hence null for exit block
                    return null;
                case ForeachExpressionSyntax _:
                case WhileExpressionSyntax _:
                    throw new NotImplementedException();
                case LoopExpressionSyntax loopExpression:
                {
                    var loopEntry = graph.NewEntryBlock(currentBlock);
                    currentBlock = loopEntry;
                    breakToBlock = graph.NewBlock();
                    var loopExit = ConvertExpressionToStatement(loopExpression.Block);
                    loopExit?.AddGoto(loopEntry);
                    return breakToBlock;
                }
                case BreakExpressionSyntax _:
                    currentBlock.AddGoto(breakToBlock ?? throw new InvalidOperationException());
                    return null;
                case IfExpressionSyntax ifExpression:
                    var containingBlock = currentBlock;
                    var condition = ConvertToOperand(ifExpression.Condition);
                    var thenEntry = graph.NewBlock();
                    currentBlock = thenEntry;
                    var thenExit = ConvertExpressionToStatement(ifExpression.ThenBlock);
                    BlockBuilder elseEntry;
                    BlockBuilder exit = null;
                    if (ifExpression.ElseClause == null)
                    {
                        elseEntry = exit = graph.NewBlock();
                        thenExit?.AddGoto(exit);
                    }
                    else
                    {
                        elseEntry = graph.NewBlock();
                        currentBlock = elseEntry;
                        var elseExit = ConvertExpressionToStatement(ifExpression.ElseClause);
                        if (thenExit != null || elseExit != null)
                        {
                            exit = graph.NewBlock();
                            thenExit?.AddGoto(exit);
                            elseExit?.AddGoto(exit);
                        }
                    }
                    containingBlock.AddIf(condition, thenEntry, elseEntry);
                    return exit;
                case BlockSyntax block:
                {
                    // It is ok that we lose knowledge of where this block ends because the liveness
                    // check will see that variables are dead at the end of the block. We will insert
                    // delete statements at the earliest possible point, so everything will be deleted
                    // at or before the end of the block.
                    foreach (var statementInBlock in block.Statements)
                        currentBlock = ConvertToStatement(statementInBlock);
                    return currentBlock;
                }
                case UnsafeExpressionSyntax unsafeExpression:
                    return ConvertToStatement(unsafeExpression.Expression);
                case AssignmentExpressionSyntax assignmentExpression:
                {
                    var place = ConvertToPlace(assignmentExpression.LeftOperand);
                    var value = ConvertToValue(assignmentExpression.RightOperand);
                    if (assignmentExpression.Operator != AssignmentOperator.Direct)
                    {
                        var type = (SimpleType)assignmentExpression.RightOperand.Type;
                        var rightOperand = ConvertToOperand(value, type);
                        BinaryOperator binaryOperator;
                        switch (assignmentExpression.Operator)
                        {
                            case AssignmentOperator.Plus:
                                binaryOperator = BinaryOperator.Plus;
                                break;
                            case AssignmentOperator.Minus:
                                binaryOperator = BinaryOperator.Minus;
                                break;
                            case AssignmentOperator.Asterisk:
                                binaryOperator = BinaryOperator.Asterisk;
                                break;
                            case AssignmentOperator.Slash:
                                binaryOperator = BinaryOperator.Slash;
                                break;
                            default:
                                throw NonExhaustiveMatchException.ForEnum(assignmentExpression.Operator);
                        }
                        value = new BinaryOperation(place, binaryOperator, rightOperand, type);
                    }
                    currentBlock.AddAssignment(place, value, assignmentExpression.Span);
                    return currentBlock;
                }
                case ResultExpressionSyntax resultExpression:
                    // Must be an expression of type `never`
                    return ConvertExpressionToStatement(resultExpression.Expression);
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private Value ConvertToValue(ExpressionSyntax expression)
        {
            switch (expression)
            {
                case NewObjectExpressionSyntax newObjectExpression:
                    var args = newObjectExpression.Arguments.Select(a => ConvertToOperand(a.Value)).ToFixedList();
                    return new ConstructorCall((ObjectType)newObjectExpression.Type, args, newObjectExpression.Span);
                case IdentifierNameSyntax identifier:
                {
                    var symbol = identifier.ReferencedSymbol;
                    switch (symbol)
                    {
                        case VariableDeclarationStatementSyntax _:
                        case ParameterSyntax _:
                            return graph.VariableFor(symbol.FullName.UnqualifiedName).Reference(identifier.Span);
                        default:
                            return new DeclaredValue(symbol.FullName, identifier.Span);
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
                    return new BooleanConstant(boolLiteral.Value, boolLiteral.Span);
                case ImplicitNumericConversionExpression implicitNumericConversion:
                    if (implicitNumericConversion.Expression.Type.AssertResolved() is IntegerConstantType constantType)
                        return new IntegerConstant(constantType.Value, implicitNumericConversion.Type.AssertResolved(), implicitNumericConversion.Span);
                    else
                        throw new NotImplementedException();
                case IfExpressionSyntax ifExpression:
                    // TODO deal with the value of the if expression
                    throw new NotImplementedException();
                case UnsafeExpressionSyntax unsafeExpression:
                    return ConvertToValue(unsafeExpression.Expression);
                case ImplicitLiteralConversionExpression implicitLiteralConversion:
                {
                    var conversionFunction = implicitLiteralConversion.ConversionFunction;
                    var literal = (StringLiteralExpressionSyntax)implicitLiteralConversion.Expression;
                    var constantLength = Utf8BytesConstant.Encoding.GetByteCount(literal.Value);
                    var sizeArgument = new IntegerConstant(constantLength, DataType.Size, literal.Span);
                    var bytesArgument = new Utf8BytesConstant(literal.Value, literal.Span);
                    return new FunctionCall(implicitLiteralConversion.Span,
                        conversionFunction.FullName,
                        (FunctionType)conversionFunction.Type,
                        sizeArgument,
                        bytesArgument);
                }
                case InvocationSyntax invocation:
                    return ConvertInvocationToValue(invocation);
                case MemberAccessExpressionSyntax memberAccess:
                {
                    var value = ConvertToOperand(memberAccess.Expression);
                    var symbol = memberAccess.ReferencedSymbol;
                    if (symbol is IAccessorSymbol accessor)
                        return new VirtualFunctionCall(memberAccess.Span, accessor.PropertyName.UnqualifiedName, value);

                    return new FieldAccessValue(value,
                        memberAccess.ReferencedSymbol.FullName, memberAccess.Span);
                }
                case MutableExpressionSyntax mutable:
                    // TODO shouldn't borrowing be explicit in the IR and don't we
                    // need to be able to check mutability on borrows?
                    return ConvertToValue(mutable.Expression);
                case MoveExpressionSyntax move:
                    // TODO should this be explicit in IR?
                    return ConvertToValue(move.Expression);
                case ImplicitImmutabilityConversionExpression implicitImmutabilityConversion:
                {
                    var operand = ConvertToOperand(implicitImmutabilityConversion.Expression);
                    switch (operand)
                    {
                        case BooleanConstant _:
                        case Utf8BytesConstant _:
                        case IntegerConstant _:
                            return operand;
                        case Dereference _:
                            throw new NotImplementedException();
                        case VariableReference varReference:
                            return varReference.AsShared();
                        default:
                            throw NonExhaustiveMatchException.For(expression);
                    }
                }
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private Operand ConvertToOperand(ExpressionSyntax expression)
        {
            var value = ConvertToValue(expression);
            return ConvertToOperand(value, expression.Type);
        }

        private Operand ConvertToOperand(Value value, DataType type)
        {
            if (value is Operand operand) return operand;
            var tempVariable = graph.Let(type.AssertResolved());
            currentBlock.AddAssignment(tempVariable.AssignReference(value.Span), value, value.Span);
            return tempVariable.Reference(value.Span);
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
                    return new UnaryOperation(expression.Operator, operand, expression.Span);
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
                    return new FunctionCall(symbol.FullName, (FunctionType)symbol.Type, arguments, invocation.Span);
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
                                    return new FunctionCall(function.FullName, (FunctionType)function.Type, self, arguments, invocation.Span);
                                default:
                                    return new VirtualFunctionCall(invocation.Span, function.FullName.UnqualifiedName, self, arguments);
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
                    return graph.VariableFor(identifier.ReferencedSymbol.FullName.UnqualifiedName).AssignReference(value.Span);
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
