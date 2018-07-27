using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SyntaxAnnotation<TValue>
    {
        private readonly Dictionary<SyntaxBranchNode, TValue> annotations = new Dictionary<SyntaxBranchNode, TValue>();

        public void Add(SyntaxBranchNode syntax, TValue value)
        {
            annotations.Add(syntax, value);
        }

        public TValue this[SyntaxBranchNode syntax] => annotations[syntax];

        public bool TryGetValue(SyntaxBranchNode syntax, out TValue value)
        {
            return annotations.TryGetValue(syntax, out value);
        }
    }
}
