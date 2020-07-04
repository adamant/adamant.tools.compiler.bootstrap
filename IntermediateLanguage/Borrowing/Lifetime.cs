//using System;
//using System.Diagnostics.CodeAnalysis;

//namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
//{
//    /// <summary>
//    /// The "name" of a lifetime of an object or function call.
//    /// </summary>
//    [SuppressMessage("Naming", "CA1724:Type names should not match namespaces", Justification = "Obscure system namespace")]
//    public struct Lifetime : IClaimHolder, IEquatable<Lifetime>
//    {
//        public int Number { get; }

//        public Lifetime(int number)
//        {
//            Number = number;
//        }

//        public override string ToString()
//        {
//            return "#" + Number;
//        }

//        public static bool operator ==(Lifetime a, Lifetime b)
//        {
//            return a.Number == b.Number;
//        }

//        public static bool operator !=(Lifetime a, Lifetime b)
//        {
//            return a.Number != b.Number;
//        }

//        public override int GetHashCode()
//        {
//            return Number.GetHashCode();
//        }

//        public bool Equals(Lifetime other)
//        {
//            return Number == other.Number;
//        }

//        public override bool Equals(object? obj)
//        {
//            return obj is Lifetime other && Number == other.Number;
//        }
//    }
//}
