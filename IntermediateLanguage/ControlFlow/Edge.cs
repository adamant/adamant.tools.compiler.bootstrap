using System;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public struct Edge : IEquatable<Edge>
    {
        public BasicBlock From { get; }
        public BasicBlock To { get; }

        public Edge(BasicBlock from, BasicBlock to)
        {
            From = from;
            To = to;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(From, To);
        }

        public bool Equals(Edge other)
        {
            return Equals(From, other.From) && Equals(To, other.To);
        }

        public override bool Equals(object? obj)
        {
            return obj is Edge other && Equals(other);
        }

        public static bool operator ==(Edge left, Edge right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Edge left, Edge right)
        {
            return !(left==right);
        }
    }
}
