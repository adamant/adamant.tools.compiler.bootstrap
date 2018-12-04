using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    public class ControlFlowGraphBuilder
    {
        private readonly List<LocalVariableDeclaration> variables = new List<LocalVariableDeclaration>();
        public LocalVariableDeclaration ReturnVariable => variables.First();
        private readonly List<BasicBlock> blocks = new List<BasicBlock>();
        private List<ExpressionStatement> statements = new List<ExpressionStatement>();
        public int CurrentBlockNumber => blocks.Count;
        public int NextStatementNumber => statements.Count;

        public LocalVariableDeclaration AddVariable(bool mutableBinding, DataType type, SimpleName name = null)
        {
            var variable = new LocalVariableDeclaration(false, mutableBinding, type, variables.Count, name);
            variables.Add(variable);
            return variable;
        }

        public LocalVariableDeclaration AddParameter(bool mutableBinding, DataType type, SimpleName name)
        {
            var variable = new LocalVariableDeclaration(true, mutableBinding, type, variables.Count, name);
            variables.Add(variable);
            return variable;
        }

        public LocalVariableDeclaration Let(DataType type)
        {
            return AddVariable(false, type);
        }

        public LocalVariableDeclaration Var(DataType type)
        {
            return AddVariable(true, type);
        }

        public VariableReference VariableFor(SimpleName name)
        {
            return variables.Single(v => v.Name == name).Reference;
        }

        public void AddAssignment(Place place, Value value)
        {
            statements.Add(new AssignmentStatement(CurrentBlockNumber, NextStatementNumber, place, value));
        }

        public void AddAction(Value value)
        {
            statements.Add(new ActionStatement(CurrentBlockNumber, NextStatementNumber, value));
        }

        public void AddDelete(VariableReference variable, TextSpan span)
        {
            statements.Add(new DeleteStatement(CurrentBlockNumber, NextStatementNumber, variable.VariableNumber, span));
        }

        private BasicBlock AddBlock(BlockTerminatorStatement terminator)
        {
            var block = new BasicBlock(CurrentBlockNumber, statements, terminator);
            blocks.Add(block);
            statements = new List<ExpressionStatement>();
            return block;
        }

        public BasicBlock AddBlockReturn()
        {
            return AddBlock(new ReturnStatement(CurrentBlockNumber, NextStatementNumber));
        }

        public ControlFlowGraph Build()
        {
            return new ControlFlowGraph(this.variables, this.blocks);
        }
    }
}
