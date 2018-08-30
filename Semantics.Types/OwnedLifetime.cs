namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class OwnedLifetime : Lifetime
    {
        #region Singleton
        public static readonly OwnedLifetime Instance = new OwnedLifetime();

        private OwnedLifetime() { }

        #endregion

        public override bool IsOwned => true;

        public override string ToString()
        {
            return "owned";
        }
    }
}
