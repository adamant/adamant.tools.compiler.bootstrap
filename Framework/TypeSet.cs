using System;
using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    /// <summary>
    /// A set of types
    /// </summary>
    /// <typeparam name="TBase">The type all types in the set must inherit from</typeparam>
    public class TypeSet<TBase>
    {
        private readonly ISet<Type> types = new HashSet<Type>();

        public bool Add<T>()
            where T : TBase
        {
            return types.Add(typeof(T));
        }

        public bool Contains<T>()
            where T : TBase
        {
            return types.Contains(typeof(T));
        }

        public bool Contains(Type type)
        {
            return types.Contains(type);
        }
    }
}
