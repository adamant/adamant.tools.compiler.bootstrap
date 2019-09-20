using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    [Closed(
        typeof(ObjectType),
        typeof(AnyType))]
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
                ? ValueSemantics.Own
                : (Mutability == Mutability.Mutable ? ValueSemantics.Borrow : ValueSemantics.Alias);
        }

        protected internal override Self AsDeclaredReturnsSelf()
        {
            if (!Mutability.IsUpgradable)
                return this;
            return this.AsImmutable();
        }

        /// <summary>
        /// Use this type as an immutable type.
        /// </summary>
        protected internal abstract Self AsImmutableReturnsSelf();

        protected internal abstract Self WithLifetimeReturnsSelf(Lifetime lifetime);
    }

    public static class ReferenceTypeExtensions
    {
        public static T AsImmutable<T>(this T type)
            where T : ReferenceType
        {
            return type.AsImmutableReturnsSelf().Cast<T>();
        }

        public static T WithLifetime<T>(this T type, Lifetime lifetime)
            where T : ReferenceType
        {
            return type.WithLifetimeReturnsSelf(lifetime).Cast<T>();
        }
    }
}
