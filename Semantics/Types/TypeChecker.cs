using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class TypeChecker
    {
        [NotNull] private readonly NameBinder nameBinder;

        public TypeChecker([NotNull] NameBinder nameBinder)
        {
            this.nameBinder = nameBinder;
        }

        public void CheckTypes([NotNull] Package package)
        {

        }
    }
}
