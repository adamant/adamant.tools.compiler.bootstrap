namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    ///// <summary>
    ///// Builder pattern for control flow graphs used during control flow graph fabrication
    ///// </summary>
    //public class ControlFlowGraphBuilder
    //{
    //    private readonly CodeFile file;
    //    private readonly List<VariableDeclaration> variables = new List<VariableDeclaration>();
    //    public VariableDeclaration ReturnVariable => variables.First();
    //    private readonly List<BlockBuilder> blockBuilders = new List<BlockBuilder>();

    //    public ControlFlowGraphBuilder(CodeFile file)
    //    {
    //        this.file = file;
    //    }

    //    public VariableDeclaration AddVariable(bool mutableBinding, DataType type, Scope scope, SimpleName? name = null)
    //    {
    //        var variable = new VariableDeclaration(false, mutableBinding, type, new Variable(variables.Count), scope, name);
    //        variables.Add(variable);
    //        return variable;
    //    }

    //    public VariableDeclaration AddParameter(bool mutableBinding, DataType type, Scope scope, SimpleName name)
    //    {
    //        var variable = new VariableDeclaration(true, mutableBinding, type, new Variable(variables.Count), scope, name);
    //        variables.Add(variable);
    //        return variable;
    //    }

    //    public VariableDeclaration AddSelfParameter(DataType type)
    //    {
    //        Requires.That("variableNumber", variables.Count == 0, "Self parameter must have variable number 0");
    //        var variable = new VariableDeclaration(true, false, type, Variable.Result, null, SpecialName.Self);
    //        variables.Add(variable);
    //        return variable;
    //    }

    //    public VariableDeclaration AddReturnVariable(DataType type)
    //    {
    //        Requires.That("variableNumber", variables.Count == 0, "Return variable must have variable number 0");
    //        var variable = new VariableDeclaration(false, false, type, Variable.Result, null);
    //        variables.Add(variable);
    //        return variable;
    //    }

    //    public VariableDeclaration Let(DataType type, Scope scope)
    //    {
    //        return AddVariable(false, type, scope);
    //    }

    //    public VariableDeclaration Var(DataType type, Scope scope)
    //    {
    //        return AddVariable(true, type, scope);
    //    }

    //    public VariableDeclaration VariableFor(SimpleName name)
    //    {
    //        return variables.Single(v => v.Name == name);
    //    }

    //    public BlockBuilder NewBlock()
    //    {
    //        var block = new BlockBuilder(new BasicBlockName(blockBuilders.Count));
    //        blockBuilders.Add(block);
    //        return block;
    //    }

    //    /// Used to create a new block for the entry into a loop if needed.
    //    /// Doesn't create a new block if the current block is empty.
    //    public BlockBuilder NewEntryBlock(BlockBuilder currentBlock, TextSpan span, Scope scope)
    //    {
    //        if (!currentBlock.Statements.Any())
    //            return currentBlock;
    //        var entryBlock = NewBlock();
    //        currentBlock.AddGoto(entryBlock, span, scope);
    //        return entryBlock;
    //    }

    //    public ControlFlowGraphOld Build()
    //    {
    //        // We assume that the first block is the entry block
    //        var blocks = new List<BasicBlock>();
    //        foreach (var block in blockBuilders)
    //        {
    //            var statements = block.Statements
    //                .Take(block.Statements.Count - 1)
    //                .Cast<ExpressionStatement>();
    //            var terminator = block.Terminator;
    //            blocks.Add(new BasicBlock(block.BlockName, statements, terminator));
    //        }

    //        return new ControlFlowGraphOld(file, variables, blocks);
    //    }

    //    [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "Variable is a value type, essentially a strongly type integer")]
    //    public VariableDeclaration this[Variable variable]
    //    {
    //        get
    //        {
    //            var declaration = variables[variable.Number];
    //            Debug.Assert(declaration.Variable == variable);
    //            return declaration;
    //        }
    //    }
    //}
}
