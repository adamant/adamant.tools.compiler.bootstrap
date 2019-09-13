namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes
{
    public sealed class AnonymousLifetime : Lifetime
    {
        #region Singleton
        public static readonly AnonymousLifetime Instance = new AnonymousLifetime();

        private AnonymousLifetime() { }
        #endregion

        public override bool IsOwned => false;

        public override string ToString()
        {
            return "_";
        }
    }
}
