using System;
using System.Collections;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public class Generator<T> : IEnumerable<T>
    {
        private readonly Func<T> generator;

        public Generator(Func<T> generator)
        {
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
