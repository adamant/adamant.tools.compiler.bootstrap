using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    public class ControlFlowGraphBuilder
    {
        [NotNull, ItemNotNull] private readonly List<LocalVariableDeclaration> variables = new List<LocalVariableDeclaration>();
        [NotNull] public LocalVariableDeclaration ReturnVariable => variables.First();
        [NotNull, ItemNotNull] private readonly List<BasicBlock> blocks = new List<BasicBlock>();
        [NotNull, ItemNotNull] private List<ExpressionStatement> statements = new List<ExpressionStatement>();
        public int CurrentBlockNumber => blocks.Count;
        public int NextStatementNumber => statements.Count;

        [NotNull]
        public LocalVariableDeclaration AddVariable(bool mutableBinding, [NotNull] DataType type, [CanBeNull] SimpleName name = null)
        {
            var variable = new LocalVariableDeclaration(false, mutableBinding, type, variables.Count, name);
            variables.Add(variable);
            return variable;
        }

        [NotNull]
        public LocalVariableDeclaration AddParameter(bool mutableBinding, [NotNull] DataType type, [CanBeNull] SimpleName name)
        {
            var variable = new LocalVariableDeclaration(true, mutableBinding, type, variables.Count, name);
            variables.Add(variable);
            return variable;
        }

        [NotNull]
        public LocalVariableDeclaration Let([NotNull] DataType type)
        {
            return AddVariable(false, type);
        }

        [NotNull]
        public LocalVariableDeclaration Var([NotNull] DataType type)
        {
            return AddVariable(true, type);
        }

        [NotNull]
        public VariableReference VariableFor([NotNull] SimpleName name)
        {
            return variables.Single(v => v.Name == name).Reference;
        }

        public void AddAssignment([NotNull] Place place, [NotNull] Value value)
        {
            statements.Add(new AssignmentStatement(CurrentBlockNumber, NextStatementNumber, place, value));
        }

        public void AddDelete([NotNull] VariableReference variable, TextSpan span)
        {
            statements.Add(new DeleteStatement(CurrentBlockNumber, NextStatementNumber, variable.VariableNumber, span));
        }

        [NotNull]
        private BasicBlock AddBlock([NotNull] BlockTerminatorStatement terminator)
        {
            var block = new BasicBlock(CurrentBlockNumber, statements, terminator);
            blocks.Add(block);
            statements = new List<ExpressionStatement>();
            return block;
        }

        [NotNull]
        public BasicBlock AddBlockReturn()
        {
            return AddBlock(new ReturnStatement(CurrentBlockNumber, NextStatementNumber));
        }

        [NotNull]
        public ControlFlowGraph Build()
        {
            return new ControlFlowGraph(this.variables, this.blocks);
        }
    }
}
