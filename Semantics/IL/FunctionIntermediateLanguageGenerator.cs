using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IL.Code;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.EndStatements;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.LValues;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using ILFunctionDeclaration = Adamant.Tools.Compiler.Bootstrap.IL.Declarations.FunctionDeclaration;
using Statement = Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Statements.Statement;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IL
{
    internal class FunctionIntermediateLanguageGenerator
    {
        private readonly FunctionDeclaration function;
        private readonly Dictionary<SemanticNode, BasicBlock> blocks;
        private readonly ILFunctionDeclaration il;
        private bool converted;
        private readonly BasicBlock currentBlock;

        public FunctionIntermediateLanguageGenerator(FunctionDeclaration function)
        {
            this.function = function;
            blocks = new Dictionary<SemanticNode, BasicBlock>();
            il = new ILFunctionDeclaration(function.Name, function.Parameters.Count);

            // Temp Variable for return
            il.Let(function.ReturnType.Type);
            foreach (var parameter in function.Parameters)
                il.AddVariable(parameter.MutableBinding, parameter.Type.Type, parameter.Name);

            var entryBlock = il.EntryBlock;
            blocks.Add(function.Body, entryBlock);
            currentBlock = entryBlock;
        }

        public ILFunctionDeclaration Convert()
        {
            if (!converted)
            {
                foreach (var statement in function.Body.Statements)
                    Convert(statement);

                if (!function.Body.Statements.Any())
                    currentBlock.End(new ReturnStatement());

                converted = true;
            }

            return il;
        }

        private VariableReference LookupVariable(string name)
        {
            return il.VariableDeclarations.Single(v => v.Name == name).Reference;
        }

        private void Convert(Statement statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatement variableDeclaration:
                    var variable = il.AddVariable(variableDeclaration.MutableBinding,
                        variableDeclaration.Type,
                        variableDeclaration.Name);
                    if (variableDeclaration.Initializer != null)
                        ConvertAssignment(variable.Reference, variableDeclaration.Initializer);
                    break;

                case ExpressionStatement expressionStatement:
                    Convert(expressionStatement.Expression);
                    break;

                case Block block:
                    foreach (var statementInBlock in block.Statements)
                        Convert(statementInBlock);

                    // Now we need to delete any owned variables
                    foreach (var variableDeclaration in block.Statements.OfType<VariableDeclarationStatement>().Where(IsOwned))
                        currentBlock.Add(new DeleteStatement(LookupVariable(variableDeclaration.Name).VariableNumber));

                    break;

                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        private static bool IsOwned(VariableDeclarationStatement declaration)
        {
            if (declaration.Type is LifetimeType type)
                return type.IsOwned;

            return false;
        }

        private void Convert(Expression expression)
        {
            switch (expression)
            {
                case AssignmentExpression assignment:
                    var lvalue = ConvertToLValue(assignment.LeftOperand);
                    ConvertAssignment(lvalue, assignment.RightOperand);
                    break;

                case ReturnExpression returnExpression:
                    ConvertAssignment(il.ReturnVariable.Reference, returnExpression.Expression);
                    currentBlock.End(new ReturnStatement());
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void ConvertAssignment(LValue lvalue, Expression value)
        {
            switch (value)
            {
                case NewObjectExpression newObjectExpression:
                    var args = newObjectExpression.Arguments.Select(ConvertToLValue);
                    currentBlock.Add(new NewObjectStatement(lvalue, newObjectExpression.Type, args));
                    break;

                case VariableExpression variableExpression:
                    currentBlock.Add(new AssignmentStatement(lvalue, LookupVariable(variableExpression.Name)));
                    break;

                case AddExpression addExpression:
                    currentBlock.Add(new AddStatement(lvalue, ConvertToLValue(addExpression.LeftOperand), ConvertToLValue(addExpression.RightOperand)));
                    break;

                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private LValue ConvertToLValue(Expression value)
        {
            switch (value)
            {
                case VariableExpression variableExpression:
                    return LookupVariable(variableExpression.Name);
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
