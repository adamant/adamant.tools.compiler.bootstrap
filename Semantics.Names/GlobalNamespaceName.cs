using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class GlobalNamespaceName : RootName
    {
        #region Singleton
        [NotNull] public static readonly GlobalNamespaceName Instance = new GlobalNamespaceName();

        private GlobalNamespaceName() { }
        #endregion

        [NotNull]
        public override Name Qualify([NotNull] SimpleName name)
        {
            Requires.NotNull(nameof(name), name);
            return name;
        }
    }
}