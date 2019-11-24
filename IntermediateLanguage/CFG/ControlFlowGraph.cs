using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG
{
    public class ControlFlowGraph
    {
        public CodeFile File { get; }
        public FixedList<VariableDeclaration> VariableDeclarations { get; }
        public IEnumerable<VariableDeclaration> Parameters => VariableDeclarations.Where(v => v.IsParameter);
        public IEnumerable<VariableDeclaration> LocalVariables => VariableDeclarations.Where(v => !v.IsParameter);
        public VariableDeclaration ReturnVariable => VariableDeclarations[0];
        public DataType ReturnType => ReturnVariable.Type;
        public FixedList<Block> Blocks { get; }
        public Block EntryBlock => Blocks[0];
        //public IEnumerable<Block> ExitBlocks => Blocks.Where(b => b.Terminator is ReturnStatement);
        //public Edges Edges { get; }

        /// <summary>
        /// If requested, the semantic analyzer will store the live variables here
        /// </summary>
        public LiveVariables? LiveVariables { get; set; }

        /// <summary>
        /// If requested, the semantic analyzer will store the borrow claims here
        /// </summary>
        public StatementClaims? BorrowClaims { get; set; }

        public InsertedDeletes? InsertedDeletes { get; set; }

        public ControlFlowGraph(
            CodeFile file,
            IEnumerable<VariableDeclaration> variableDeclarations,
            IEnumerable<Block> blocks)
        {
            File = file;
            VariableDeclarations = variableDeclarations.ToFixedList();
            Blocks = blocks.ToFixedList();
            //Edges = Edges.InGraph(this);
        }

        [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification =
            "BackBlockName is a value type")]
        public Block this[BasicBlockName block] => Blocks[block.Number];
    }
}