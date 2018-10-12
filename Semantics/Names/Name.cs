using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public abstract class Name
    {
        [NotNull]
        public abstract QualifiedName Qualify([NotNull] SimpleName name);

        [NotNull]
        public QualifiedName Qualify([NotNull] string name)
        {
            return Qualify(new SimpleName(name));
        }
    }
}
