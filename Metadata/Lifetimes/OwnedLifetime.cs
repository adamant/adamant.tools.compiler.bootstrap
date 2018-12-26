namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes
{
    public class OwnedLifetime : Lifetime
    {
        // TODO expose this as a property on lifetime instead `Lifetime.Owned`
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
