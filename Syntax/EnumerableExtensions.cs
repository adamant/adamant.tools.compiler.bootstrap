using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public static class EnumerableExtensions
    {
        [NotNull]
        public static SyntaxList<T> ToSyntaxList<T>([CanBeNull][ItemNotNull] this IEnumerable<T> nodes)
            where T : SyntaxNode
        {
            return nodes == null ? SyntaxList<T>.Empty : new SyntaxList<T>(nodes);
        }

        [NotNull]
        public static SyntaxList<T> ToSyntaxList<T>([CanBeNull] this T node)
            where T : SyntaxNode
        {
            return node == null ? SyntaxList<T>.Empty : new SyntaxList<T>(node.Yield());
        }
    }
}
