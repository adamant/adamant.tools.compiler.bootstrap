namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class LifetimeType : DataType
    {
        public DataType Type { get; }
        public Lifetime Lifetime { get; }

        public LifetimeType(DataType type, Lifetime lifetime)
        {
            Type = type;
            Lifetime = lifetime;
        }

        public bool IsOwned => Lifetime.IsOwned;

        public override string ToString()
        {
            return $"{Type}${Lifetime}";
        }
    }
}
