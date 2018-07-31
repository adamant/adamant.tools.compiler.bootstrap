using System;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public abstract class InheritedAttribute
    {
        public readonly string Name;
        public abstract Type ValueType { get; }

        protected InheritedAttribute(string name)
        {
            Name = name;
        }
    }

    public class InheritedAttribute<T> : InheritedAttribute
    {
        public override Type ValueType => typeof(T);

        public InheritedAttribute(string name)
            : base(name)
        {
        }
    }
}
