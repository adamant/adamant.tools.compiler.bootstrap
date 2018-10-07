using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public static class EnumerableExtensions
    {
        public static SyntaxList<T> ToSyntaxList<T>(this IEnumerable<T> nodes)
            where T : SyntaxNode
        {
            return nodes == null ? SyntaxList<T>.Empty : new SyntaxList<T>(nodes);
        }

        public static SyntaxList<T> ToSyntaxList<T>(this T node)
            where T : SyntaxNode
        {
            return node == null ? SyntaxList<T>.Empty : new SyntaxList<T>(node.Yield());
        }
    }
}
