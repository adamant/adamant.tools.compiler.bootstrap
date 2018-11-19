using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public class OwnedLifetime : Lifetime
    {
        #region Singleton
        [NotNull]
        public static readonly OwnedLifetime Instance = new OwnedLifetime();

        private OwnedLifetime() { }
        #endregion

        public override bool IsOwned => true;

        [NotNull]
        public override string ToString()
        {
            return "owned";
        }
    }
}
