using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public abstract class ReferenceType : DataType
    {
        public Lifetime Lifetime { get; }
        public bool IsOwned => Lifetime.IsOwned;

        protected ReferenceType(Lifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}
