using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class RefLifetime : Lifetime
    {
        #region Singleton
        [NotNull]
        public static readonly RefLifetime Instance = new RefLifetime();

        private RefLifetime() { }
        #endregion

        public override bool IsOwned => true;

        [NotNull]
        public override string ToString()
        {
            return "ref";
        }
    }
}
