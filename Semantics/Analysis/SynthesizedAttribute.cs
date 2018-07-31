using System;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public abstract class SynthesizedAttribute
    {
        public readonly string Name;
        public abstract Type ValueType { get; }

        protected SynthesizedAttribute(string name)
        {
            Name = name;
        }
    }

    public class SynthesizedAttribute<T> : SynthesizedAttribute
    {
        public override Type ValueType => typeof(T);

        public SynthesizedAttribute(string name)
            : base(name)
        {
        }
    }
}
