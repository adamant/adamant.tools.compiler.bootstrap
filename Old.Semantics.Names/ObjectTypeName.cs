using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public abstract class ObjectTypeName : ScopeName
    {
        protected ObjectTypeName([NotNull] string name)
            : base(name)
        {
        }
    }
}
