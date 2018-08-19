using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.IL.ControlFlow
{
    public class Block
    {
        public IReadOnlyCollection<Block> Predecessors => predecessors;
        private readonly HashSet<Block> predecessors = new HashSet<Block>();

        public readonly IReadOnlyList<Operation> Operations;

        public IReadOnlyCollection<Block> Successors => successors;
        private readonly HashSet<Block> successors = new HashSet<Block>();

        public Block(IEnumerable<Operation> operations)
        {
            Operations = operations.ToList().AsReadOnly();
        }

        public static void AddEdge(Block from, Block to)
        {
            from.successors.Add(to);
            to.predecessors.Add(from);
        }
    }
}
