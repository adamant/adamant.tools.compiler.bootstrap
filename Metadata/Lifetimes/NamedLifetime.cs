namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes
{
    public class NamedLifetime : Lifetime
    {
        public readonly string Name;

        public NamedLifetime(string name)
        {
            Name = name;
        }

        public override bool IsOwned => false;

        public override string ToString()
        {
            return Name;
        }
    }
}
