using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public abstract class ObjectType : ReferenceType
    {
        protected ObjectType(Lifetime lifetime)
            : base(lifetime)
        {
        }
    }
}
