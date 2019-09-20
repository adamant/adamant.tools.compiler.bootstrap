using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    [Closed(typeof(UserObjectType))]
    public abstract class ObjectType : ReferenceType
    {
        public Name Name { get; }

        protected ObjectType(
            Name name,
            bool declaredMutable,
            Mutability mutability,
            Lifetime lifetime)
            : base(declaredMutable, mutability, lifetime)
        {
            Name = name;
        }
    }
}
