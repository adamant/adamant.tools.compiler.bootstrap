using System.Collections.Generic;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class StackExtensions
    {
        public static void PushRange<T>([NotNull] this Stack<T> stack, [NotNull] IEnumerable<T> items)
        {
            foreach (var item in items)
                stack.Push(item);
        }
    }
}
