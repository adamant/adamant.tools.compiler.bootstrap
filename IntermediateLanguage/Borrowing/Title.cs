using System;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    public class Title : Claim, IEquatable<Title>
    {
        public Title(VariableNumber variable, int objectId)
           : base(variable, objectId)
        {
        }

        public override string ToString()
        {
            return $"Title to #{ObjectId} for %{Variable}";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Title);
        }

        public override bool Equals(Claim other)
        {
            return Equals(other as Title);
        }

        public bool Equals(Title other)
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
