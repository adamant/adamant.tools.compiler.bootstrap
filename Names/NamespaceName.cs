using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    public sealed class NamespaceName : IEquatable<NamespaceName>
    {
        public static readonly NamespaceName Global = new NamespaceName(Enumerable.Empty<Name>());

        public FixedList<Name> Segments { get; }

        public NamespaceName(IEnumerable<Name> segments)
        {
            Segments = segments.ToFixedList();
        }

        public NamespaceName(Name segment)
        {
            Segments = segment.Yield().ToFixedList();
        }

        public NamespaceName Qualify(NamespaceName name)
        {
            return new NamespaceName(Segments.Concat(name.Segments));
        }

        public IEnumerable<NamespaceName> NamespaceNames()
        {
            yield return Global;
            for (int n = 1; n <= Segments.Count; n++)
                yield return new NamespaceName(Segments.Take(n));
        }

        public RootName ToRootName()
        {
            RootName name = GlobalNamespaceName.Instance;

            foreach (var segment in Segments)
                name = name.Qualify(segment.Text);

            return name;
        }

        public override string ToString()
        {
            return string.Join('.', Segments);
        }

        public bool Equals(NamespaceName? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Segments.Equals(other.Segments);
        }

        public override bool Equals(object? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is NamespaceName name && Equals(name);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Segments);
        }

        public static bool operator ==(NamespaceName? left, NamespaceName? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NamespaceName? left, NamespaceName? right)
        {
            return !Equals(left, right);
        }

        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates",
            Justification = "Constructor is alternative")]
        public static implicit operator NamespaceName(Name name)
        {
            return new NamespaceName(name);
        }

        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates",
            Justification = "This is just a chained implicit conversion")]
        public static implicit operator NamespaceName(string text)
        {
            return new NamespaceName(text);
        }

        public bool IsNestedIn(NamespaceName ns)
        {
            return ns.Segments.Count < Segments.Count
                   && ns.Segments.SequenceEqual(Segments.Take(ns.Segments.Count));
        }
    }
}
