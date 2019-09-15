namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes
{
    /// <summary>
    /// Represents situations where we have no lifetime to associate to a type.
    /// For example, the metatype makes no reference to the lifetime of the type.
    /// </summary>
    public class NoLifetime : Lifetime
    {
        #region Singleton
        internal static readonly NoLifetime Instance = new NoLifetime();

        private NoLifetime() { }
        #endregion

        public override bool IsOwned => false;

        public override string ToString()
        {
            return "⧼none⧽";
        }
    }
}
