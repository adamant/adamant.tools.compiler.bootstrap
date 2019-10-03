using System;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    /// <summary>
    /// This struct is used as a marker type for the returns `Self` type idiom.
    /// In this idiom, a `protected internal` method is declared on a class with
    /// return type `Self` and name ending in `ReturnsSelf`. For convenience,
    /// the base type may want to implement an implicit conversion to `Self`.
    /// A generic extension method is then used to re-expose the `ReturnsSelf`
    /// method as a method that actually has the same return type as the object
    /// it is called on. Note, it is up to implementors of the `ReturnsSelf`
    /// methods to actually return an object of the same type as the current
    /// class.
    /// </summary>
    public struct Self : IEquatable<Self>
    {
        private readonly object self;

        public Self(object self)
        {
            this.self = self;
        }

        public T Cast<T>()
        {
            return (T)self;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Self other)
                return ReferenceEquals(other.self, self);
            return false;
        }

        public bool Equals(Self other)
        {
            return ReferenceEquals(other.self, self);
        }

        public override int GetHashCode()
        {
            return self?.GetHashCode() ?? 0;
        }

        public static bool operator ==(Self left, Self right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Self left, Self right)
        {
            return !(left==right);
        }
    }
}
