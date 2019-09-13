namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes
{
    public sealed class ForeverLifetime : Lifetime
    {
        #region Singleton
        internal static readonly ForeverLifetime Instance = new ForeverLifetime();

        private ForeverLifetime() { }
        #endregion

        public override bool IsOwned => false;

        public override string ToString()
        {
            return "forever";
        }
    }
}
