using System;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// A claim that a particular variable is the owner of an object
    /// </summary>
    public class Ownership : Claim, IEquatable<Ownership>
    {
        public Ownership(VariableNumber variable, int objectId)
           : base(variable, objectId)
        {
        }

        public override string ToString()
        {
            return $"%{Variable} owns #{ObjectId}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Ownership);
        }

        public override bool Equals(Claim other)
        {
            return Equals(other as Ownership);
        }

        public bool Equals(Ownership other)
        {
            return other != null &&
                   base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
