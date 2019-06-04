using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// A claim that a variable has borrowed an object reference. For implementation
    /// purposes, a borrow is a mutable borrow while an immutable borrow is termed
    /// a share. When there are multiple borrow claims, the most recent one takes
    /// precedence.
    /// </summary>
    public class Borrows : Claim, IEquatable<Borrows>
    {
        public Borrows(VariableNumber variable, int objectId)
            : base(variable, objectId)
        {
        }

        public override string ToString()
        {
            return $"%{Variable} borrows #{ObjectId}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Borrows);
        }

        public override bool Equals(Claim other)
        {
            return Equals(other as Borrows);
        }

        public bool Equals(Borrows other)
        {
            return other != null && base.Equals(other);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(base.GetHashCode());
            return hash.ToHashCode();
        }

        public static bool operator ==(Borrows loan1, Borrows loan2)
        {
            return EqualityComparer<Borrows>.Default.Equals(loan1, loan2);
        }

        public static bool operator !=(Borrows loan1, Borrows loan2)
        {
            return !(loan1 == loan2);
        }
    }
}
