using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Names
{
    public abstract class ObjectTypeName : ScopeName
    {
        protected ObjectTypeName([NotNull] string name)
            : base(name)
        {
        }
    }
}
