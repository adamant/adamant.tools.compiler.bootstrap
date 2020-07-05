using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG
{
    public class ControlFlowGraph
    {
        public CodeFile File { get; }
        public FixedList<VariableDeclaration> VariableDeclarations { get; }
        public IEnumerable<VariableDeclaration> Parameters => VariableDeclarations.Where(v => v.IsParameter);
        public IEnumerable<VariableDeclaration> LocalVariables => VariableDeclarations.Where(v => !v.IsParameter);
        //public VariableDeclaration ReturnVariable => VariableDeclarations[0];
        //public DataType ReturnType => ReturnVariable.Type;
        public FixedList<Block> Blocks { get; }

        /// <summary>
        /// If requested, the semantic analyzer will store the live variables here
        /// </summary>
        //public LiveVariables? LiveVariables { get; set; }

        public ControlFlowGraph(
            CodeFile file,
            IEnumerable<VariableDeclaration> variableDeclarations,
            IEnumerable<Block> blocks)
        {
            File = file;
            VariableDeclarations = variableDeclarations.ToFixedList();
            Blocks = blocks.ToFixedList();
        }
    }
}
