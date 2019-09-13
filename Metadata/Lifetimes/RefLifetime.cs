namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes
{
    public sealed class RefLifetime : Lifetime
    {
        #region Singleton
        public static readonly RefLifetime Instance = new RefLifetime();

        private RefLifetime() { }
        #endregion

        public override bool IsOwned => true;

        public override string ToString()
        {
            return "ref";
        }
    }
}
