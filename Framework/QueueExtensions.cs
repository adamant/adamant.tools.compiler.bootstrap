using System.Collections.Generic;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class QueueExtensions
    {
        public static void EnqueueRange<T>([NotNull] this Queue<T> queue, [NotNull] IEnumerable<T> items)
        {
            foreach (var item in items)
                queue.Enqueue(item);
        }
    }
}
