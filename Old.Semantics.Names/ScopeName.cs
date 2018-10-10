using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Names
{
    public abstract class ScopeName : Name
    {
        protected ScopeName([NotNull] string name)
            : base(name)
        {
        }
    }
}
