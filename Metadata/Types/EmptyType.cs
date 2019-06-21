using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The two empty types are `never` and `void`. They are the only types with
    /// no values.
    /// </summary>
    public abstract class EmptyType : DataType
    {
        public Name Name { get; }

        public override bool IsEmpty => true;

        public override bool IsKnown => true;

        protected EmptyType(Name name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}