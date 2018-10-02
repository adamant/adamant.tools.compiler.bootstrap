using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class SeparatedListSyntax<T> : SyntaxBranchNode, IEnumerable<T>
        where T : SyntaxBranchNode
    {
        public readonly IReadOnlyList<T> Items;
        public readonly IReadOnlyList<Token> Separators;
        public SeparatedListSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            // TODO validate that it alternates between them
            Items = Children.OfType<T>().ToList().AsReadOnly();
            Separators = Children.OfType<Token>().ToList().AsReadOnly();
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
