//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using Adamant.Tools.Compiler.Bootstrap.Framework;
//using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;
//using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
//using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements;
//using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.LValues;
//using JetBrains.Annotations;

//namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.IL
//{
//    internal class FunctionIntermediateLanguageGenerator
//    {
//        [NotNull] private readonly FunctionDeclaration function;
//        [NotNull] [ItemNotNull] private readonly Dictionary<SemanticNode, BasicBlock> blocks = new Dictionary<SemanticNode, BasicBlock>();
//        [NotNull] private readonly FunctionDeclarationIL il;
//        private bool converted;
//        [NotNull] private readonly BasicBlock currentBlock;

//        public FunctionIntermediateLanguageGenerator([NotNull] FunctionDeclaration function)
//        {
//            Requires.NotNull(nameof(function), function);
//            this.function = function;
//            il = new FunctionDeclarationIL(function.Name, function.Parameters.Count);

//            // Temp Variable for return
//            il.Let(function.ReturnType.Type);
//            foreach (var parameter in function.Parameters)
//                il.AddVariable(parameter.MutableBinding, parameter.Type.Type, parameter.Name);

//            var entryBlock = il.EntryBlock;
//            blocks.Add(function.Body, entryBlock);
//            currentBlock = entryBlock;
//        }

//        [NotNull]
//        public FunctionDeclarationIL Convert()
//        {
//            if (!converted)
//            {
//                foreach (var statement in function.Body.Statements)
//                    Convert(statement);

//                if (!function.Body.Statements.Any())
//                    currentBlock.End(new ReturnStatement());

//                converted = true;
//            }

//            return il;
//        }

//        [NotNull]
//        private VariableReference LookupVariable([NotNull] string name)
//        {
//            Requires.NotNull(nameof(name), name);
//            return il.VariableDeclarations.Single(v => v.Name == name).Reference;
//        }

//        private void Convert(Statement statement)
//        {
//            switch (statement)
//            {
//                case VariableDeclarationStatement variableDeclaration:
//                    var variable = il.AddVariable(variableDeclaration.MutableBinding,
//                        variableDeclaration.Type,
//                        variableDeclaration.Name);
//                    if (variableDeclaration.Initializer != null)
//                        ConvertAssignment(variable.Reference, variableDeclaration.Initializer);
//                    break;

//                case ExpressionStatement expressionStatement:
//                    Convert(expressionStatement.Expression);
//                    break;

//                case Block block:
//                    foreach (var statementInBlock in block.Statements)
//                        Convert(statementInBlock);

//                    // Now we need to delete any owned variables
//                    foreach (var variableDeclaration in block.Statements.OfType<VariableDeclarationStatement>().Where(IsOwned))
//                        currentBlock.Add(new DeleteStatement(LookupVariable(variableDeclaration.Name).VariableNumber));

//                    break;

//                default:
//                    throw NonExhaustiveMatchException.For(statement);
//            }
//        }

//        private static bool IsOwned([NotNull] VariableDeclarationStatement declaration)
//        {
//            Requires.NotNull(nameof(declaration), declaration);
//            if (declaration.Type is LifetimeType type)
//                return type.IsOwned;

//            return false;
//        }

//        private void Convert(Expression expression)
//        {
//            switch (expression)
//            {
//                case AssignExpression assignment:
//                    var lvalue = ConvertToLValue(assignment.LeftOperand);
//                    ConvertAssignment(lvalue, assignment.RightOperand);
//                    break;

//                case ReturnExpression returnExpression:
//                    if (returnExpression.Expression != null)
//                        ConvertAssignment(il.ReturnVariable.Reference, returnExpression.Expression);
//                    currentBlock.End(new ReturnStatement());
//                    break;
//                default:
//                    throw NonExhaustiveMatchException.For(expression);
//            }
//        }

//        private void ConvertAssignment([NotNull] LValue lvalue, [NotNull] Expression value)
//        {
//            Requires.NotNull(nameof(lvalue), lvalue);
//            Requires.NotNull(nameof(value), value);
//            switch (value)
//            {
//                case NewObjectExpression newObjectExpression:
//                    var args = newObjectExpression.Arguments.Select(ConvertToLValue);
//                    currentBlock.Add(new NewObjectStatement(lvalue, newObjectExpression.Type, args));
//                    break;

//                case VariableExpression variableExpression:
//                    currentBlock.Add(new AssignmentStatement(lvalue, LookupVariable(variableExpression.Name)));
//                    break;

//                case AddExpression plusExpression:
//                    currentBlock.Add(new AddStatement(lvalue, ConvertToLValue(plusExpression.LeftOperand), ConvertToLValue(plusExpression.RightOperand)));
//                    break;

//                default:
//                    throw NonExhaustiveMatchException.For(value);
//            }
//        }

//        [NotNull]
//        private LValue ConvertToLValue([NotNull] Expression value)
//        {
//            Requires.NotNull(nameof(value), value);
//            switch (value)
//            {
//                case VariableExpression variableExpression:
//                    return LookupVariable(variableExpression.Name);
//                default:
//                    throw NonExhaustiveMatchException.For(value);
//            }
//        }
//    }
//}
