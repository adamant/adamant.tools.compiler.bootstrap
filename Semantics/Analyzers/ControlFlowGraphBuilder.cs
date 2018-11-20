//using System.Collections.Generic;
//using System.Linq;
//using Adamant.Tools.Compiler.Bootstrap.AST;
//using Adamant.Tools.Compiler.Bootstrap.Core;
//using Adamant.Tools.Compiler.Bootstrap.Framework;
//using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses;
//using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
//using Adamant.Tools.Compiler.Bootstrap.Types;
//using JetBrains.Annotations;

//namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
//{
//    public class ControlFlowGraphBuilder
//    {
//        public static void BuildGraphs([NotNull, ItemNotNull] IEnumerable<FunctionDeclarationAnalysis> functions)
//        {
//            Requires.NotNull(nameof(functions), functions);

//            foreach (var function in functions.Where(f => !f.Diagnostics.Any() // It has errors, can't generate intermediate code
//                                                          && f.Syntax.Body != null // It is abstract
//                                                          && !f.IsGeneric)) // It is not generic, generic functions need monomorphized
//            {
//                var builder = new ControlFlowGraphBuilder();
//                builder.BuildGraph(function);
//            }
//        }

//        [NotNull, ItemNotNull] private readonly List<LocalVariableDeclaration> variables = new List<LocalVariableDeclaration>();
//        [NotNull] private LocalVariableDeclaration ReturnVariable => variables.First().NotNull();
//        [NotNull, ItemNotNull] private readonly List<BasicBlock> blocks = new List<BasicBlock>();
//        private int CurrentBlockNumber => blocks.Count;

//        [NotNull]
//        public LocalVariableDeclaration AddParameter(bool mutableBinding, [NotNull] DataType type, [CanBeNull] string name)
//        {
//            var variable = new LocalVariableDeclaration(true, mutableBinding, type, variables.Count)
//            {
//                Name = name
//            };
//            variables.Add(variable);
//            return variable;
//        }

//        [NotNull]
//        public LocalVariableDeclaration AddVariable(bool mutableBinding, [NotNull] DataType type, [CanBeNull] string name = null)
//        {
//            var variable = new LocalVariableDeclaration(false, mutableBinding, type, variables.Count)
//            {
//                Name = name
//            };
//            variables.Add(variable);
//            return variable;
//        }

//        [NotNull]
//        public LocalVariableDeclaration Let([NotNull] DataType type)
//        {
//            return AddVariable(false, type);
//        }

//        [NotNull]
//        public LocalVariableDeclaration Var([NotNull] DataType type)
//        {
//            return AddVariable(true, type);
//        }

//        [NotNull]
//        public BasicBlock AddBlock(
//            [NotNull] IEnumerable<ExpressionStatement> statements,
//            [NotNull] BlockTerminatorStatement terminator)
//        {
//            var block = new BasicBlock(CurrentBlockNumber, statements, terminator);
//            blocks.Add(block);
//            return block;
//        }

//        private void BuildGraph([NotNull] FunctionDeclarationAnalysis function)
//        {
//            Requires.NotNull(nameof(function), function);

//            // Temp Variable for return
//            Let(function.ReturnType.AssertResolved());
//            foreach (var parameter in function.Parameters)
//                AddParameter(parameter.MutableBinding, parameter.Type.AssertResolved(), parameter.Name.UnqualifiedName.Text);

//            var entryBlockStatements = new List<ExpressionStatement>();
//            foreach (var statement in function.Statements)
//                ConvertStatementAnalysisToStatement(entryBlockStatements, statement);

//            // Generate the implicit return statement
//            if (!blocks.Any())
//                AddBlock(entryBlockStatements, new ReturnStatement(CurrentBlockNumber, entryBlockStatements.Count));

//            function.ControlFlow = new ControlFlowGraph(variables, blocks);
//        }

//        [NotNull]
//        private VariableReference LookupVariable([NotNull] string name)
//        {
//            return variables.Single(v => v.Name == name).NotNull().Reference;
//        }

//        private void ConvertStatementAnalysisToStatement(
//            [NotNull, ItemNotNull] List<ExpressionStatement> currentBlock,
//            [NotNull] StatementAnalysis statement)
//        {
//            switch (statement)
//            {
//                case VariableDeclarationStatementAnalysis variableDeclaration:
//                    var variable = AddVariable(variableDeclaration.MutableBinding,
//                        variableDeclaration.Type.AssertResolved(),
//                        variableDeclaration.Name.UnqualifiedName.Text);
//                    if (variableDeclaration.Initializer != null)
//                        ConvertToAssignmentStatement(variable.Reference, variableDeclaration.Initializer, currentBlock);
//                    break;

//                case ExpressionStatementAnalysis expressionStatement:
//                    ConvertExpressionAnalysisToStatement(expressionStatement.Expression, currentBlock);
//                    break;

//                default:
//                    throw NonExhaustiveMatchException.For(statement);
//            }
//        }

//        private static bool IsOwned([NotNull] VariableDeclarationStatementAnalysis declaration)
//        {
//            Requires.NotNull(nameof(declaration), declaration);
//            if (declaration.Type.AssertResolved() is LifetimeType type)
//                return type.IsOwned;

//            return false;
//        }

//        private void ConvertExpressionAnalysisToStatement(
//            [NotNull] ExpressionAnalysis expression,
//            [NotNull] [ItemNotNull] List<ExpressionStatement> statements)
//        {
//            switch (expression)
//            {
//                case IdentifierNameAnalysis _:
//                    // Ignore, reading from variable does nothing.
//                    break;
//                case BinaryExpressionAnalysis binaryOperatorExpression:
//                    switch (binaryOperatorExpression.Operator)
//                    {
//                        //case BinaryOperator. EqualsToken _:
//                        //    var lvalue = ConvertToLValue(binaryOperatorExpression.LeftOperand);
//                        //    ConvertToAssignmentStatement(lvalue, binaryOperatorExpression.RightOperand, statements);
//                        //    break;
//                        default:
//                            // Could be side effects possibly.
//                            var temp = Let(binaryOperatorExpression.Type.AssertResolved());
//                            ConvertToAssignmentStatement(temp.Reference, expression, statements);
//                            break;
//                    }
//                    break;
//                case ReturnExpressionAnalysis returnExpression:
//                    if (returnExpression.ReturnValue != null)
//                        ConvertToAssignmentStatement(ReturnVariable.Reference, returnExpression.ReturnValue, statements);
//                    AddBlock(statements, new ReturnStatement(CurrentBlockNumber, statements.Count));
//                    break;
//                case BlockAnalysis block:
//                    foreach (var statementInBlock in block.Statements)
//                        ConvertStatementAnalysisToStatement(statements, statementInBlock);

//                    // Now we need to delete any owned variables
//                    foreach (var variableDeclaration in block.Statements.OfType<VariableDeclarationStatementAnalysis>().Where(IsOwned))
//                        statements.Add(new DeleteStatement(CurrentBlockNumber, statements.Count, LookupVariable(variableDeclaration.Name.UnqualifiedName.Text).VariableNumber, new TextSpan(block.Syntax.Span.End, 0)));

//                    break;
//                case ForeachExpressionAnalysis @foreach:
//                    // TODO actually convert the expression
//                    break;
//                case WhileExpressionAnalysis @while:
//                    // TODO actually convert the expression
//                    break;
//                case LoopExpressionAnalysis loop:
//                    // TODO actually convert the expression
//                    break;
//                case UnsafeExpressionAnalysis unsafeExpression:
//                    ConvertExpressionAnalysisToStatement(unsafeExpression.Expression, statements);
//                    break;
//                case InvocationAnalysis invocation:
//                    // TODO actually convert the expression
//                    break;
//                default:
//                    throw NonExhaustiveMatchException.For(expression);
//            }
//        }

//        private void ConvertToAssignmentStatement(
//            [NotNull] Place place,
//            [NotNull] ExpressionAnalysis value,
//            [NotNull, ItemNotNull] List<ExpressionStatement> statements)
//        {
//            switch (value)
//            {
//                //case NewObjectExpressionAnalysis newObjectExpression:
//                //    var args = newObjectExpression.Arguments.Select(a => ConvertToLValue(a.Value));
//                //    statements.Add(new NewObjectStatement(place, newObjectExpression.Type.AssertResolved(), args));
//                //    break;
//                case IdentifierNameAnalysis identifier:
//                    statements.Add(new AssignmentStatement(CurrentBlockNumber, statements.Count, place, new CopyPlace(LookupVariable(identifier.Name.Text))));
//                    break;
//                case BinaryExpressionAnalysis binaryOperator:
//                    ConvertOperator(place, binaryOperator, statements);
//                    break;
//                case IntegerLiteralExpressionAnalysis v:
//                    var constant = new IntegerConstant(v.Value, v.Type);
//                    statements.Add(new AssignmentStatement(CurrentBlockNumber, statements.Count, place, constant));
//                    break;
//                case IfExpressionAnalysis ifExpression:
//                    ConvertExpressionAnalysisToStatement(ifExpression.Condition, statements);
//                    // TODO assign the result into the temp, branch and execute then or else, assign result
//                    break;
//                case UnsafeExpressionAnalysis unsafeExpression:
//                    ConvertExpressionAnalysisToStatement(unsafeExpression.Expression, statements);
//                    break;
//                default:
//                    throw NonExhaustiveMatchException.For(value);
//            }
//        }

//        private void ConvertOperator(
//            [NotNull] Place lvalue,
//            [NotNull] BinaryExpressionAnalysis binary,
//            [NotNull] List<ExpressionStatement> statements)
//        {
//            switch (binary.Operator)
//            {
//                //case PlusToken _:
//                //    currentBlock.Add(new AddStatement(lvalue,
//                //        ConvertToLValue(cfg, binaryOperator.LeftOperand),
//                //        ConvertToLValue(cfg, binaryOperator.RightOperand)));
//                //    break;
//                case BinaryOperator.LessThan:
//                case BinaryOperator.LessThanOrEqual:
//                case BinaryOperator.GreaterThan:
//                case BinaryOperator.GreaterThanOrEqual:
//                    // TODO generate the correct statement
//                    break;
//                //case IAsKeywordToken _:
//                //    ConvertExpressionAnalysisToStatement(binaryOperator.LeftOperand, statements);
//                //    break;
//                default:
//                    throw NonExhaustiveMatchException.For(binary.Operator);
//            }
//        }

//        [NotNull]
//        private Place ConvertToLValue([NotNull] ExpressionAnalysis value)
//        {
//            Requires.NotNull(nameof(value), value);
//            switch (value)
//            {
//                case IdentifierNameAnalysis identifier:
//                    return LookupVariable(identifier.Name.Text);
//                //case VariableExpression variableExpression:
//                //    return LookupVariable(variableExpression.Name);
//                default:
//                    throw NonExhaustiveMatchException.For(value);
//            }
//        }
//    }
//}
