using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public abstract class ReferenceType : DataType
    {
        public Lifetime Lifetime { get; }
        public bool IsOwned => Lifetime.IsOwned;

        /// <summary>
        /// Whether this type was declared `mut class` or just `class`
        /// </summary>
        public bool DeclaredMutable { get; }

        public Mutability Mutability { get; }

        public override ValueSemantics ValueSemantics { get; }

        protected ReferenceType(bool declaredMutable, Mutability mutability, Lifetime lifetime)
        {
            Lifetime = lifetime;
            DeclaredMutable = declaredMutable;
            Mutability = mutability;
            ValueSemantics = IsOwned
                ? ValueSemantics.Move
                : (Mutability == Mutability.Mutable ? ValueSemantics.Alias : ValueSemantics.Borrow);
        }

        public abstract ReferenceType WithLifetime(Lifetime lifetime);
    }
}
