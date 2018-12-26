namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes
{
    public class OwnedLifetime : Lifetime
    {
        #region Singleton
        internal static readonly OwnedLifetime Instance = new OwnedLifetime();

        private OwnedLifetime() { }
        #endregion

        public override bool IsOwned => true;

        public override string ToString()
        {
            return "owned";
        }
    }
}
