using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class StackExtensions
    {
        public static void PushRange<T>(this Stack<T> stack, IEnumerable<T> items)
        {
            foreach (var item in items)
                stack.Push(item);
        }
    }
}
