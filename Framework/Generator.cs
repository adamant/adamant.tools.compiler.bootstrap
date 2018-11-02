using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public class Generator<T> : IEnumerable<T>
    {
        [NotNull] private readonly Func<T> generator;

        public Generator([NotNull] Func<T> generator)
        {
            Requires.NotNull(nameof(generator), generator);
            this.generator = generator;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (; ; )
                yield return generator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
