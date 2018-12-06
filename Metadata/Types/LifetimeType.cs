namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class LifetimeType : DataType
    {
        public ObjectType Referent { get; }
        public Lifetime Lifetime { get; }
        public override bool IsResolved { get; }
        public bool IsOwned => Lifetime.IsOwned;

        public LifetimeType(ObjectType referent, Lifetime lifetime)
        {
            Referent = referent;
            Lifetime = lifetime;
            IsResolved = Referent.IsResolved;
        }

        public override string ToString()
        {
            return $"{Referent}${Lifetime}";
        }
    }
}
