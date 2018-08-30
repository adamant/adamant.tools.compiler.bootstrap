namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class LifetimeType : DataType
    {
        public readonly DataType Type;
        public readonly Lifetime Lifetime;

        public LifetimeType(DataType type, Lifetime lifetime)
        {
            Type = type;
            Lifetime = lifetime;
        }

        public override string ToString()
        {
            return $"{Type}${Lifetime}";
        }
    }
}
