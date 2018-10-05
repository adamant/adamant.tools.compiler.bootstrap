using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class SeparatedListSyntax<T> : SyntaxNode, IEnumerable<T>
        where T : SyntaxNode
    {
        public readonly IReadOnlyList<T> Items;
        public readonly IReadOnlyList<SimpleToken> Separators;
        public SeparatedListSyntax(IEnumerable<SyntaxNode> children)
        {
            // TODO validate that it alternates between them
            Items = children.OfType<T>().ToList().AsReadOnly();
            Separators = children.OfType<SimpleToken>().ToList().AsReadOnly();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
