using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Names
{
    public abstract class NamespaceName : ScopeName
    {
        public abstract bool IsGlobalNamespace { get; }

        protected NamespaceName([NotNull] string name)
            : base(name)
        {
        }
    }
}
