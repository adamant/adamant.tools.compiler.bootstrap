using System.Collections.Concurrent;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SyntaxAnnotation<TValue>
    {
        private readonly ConcurrentDictionary<SyntaxBranchNode, TValue> annotations = new ConcurrentDictionary<SyntaxBranchNode, TValue>();

        public bool Add(SyntaxBranchNode syntax, TValue value)
        {
            return annotations.TryAdd(syntax, value);
        }

        public TValue this[SyntaxBranchNode syntax] => annotations[syntax];

        public bool TryGetValue(SyntaxBranchNode syntax, out TValue value)
        {
            return annotations.TryGetValue(syntax, out value);
        }
    }
}
