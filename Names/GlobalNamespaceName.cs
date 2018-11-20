using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// Represents the "name" of the global namespace i.e. nothing
    /// </summary>
    public class GlobalNamespaceName : RootName
    {
        #region Singleton
        [NotNull] public static readonly GlobalNamespaceName Instance = new GlobalNamespaceName();

        private GlobalNamespaceName() { }
        #endregion

        [NotNull]
        public override Name Qualify([NotNull] Name name)
        {
            return name;
        }
    }
}
