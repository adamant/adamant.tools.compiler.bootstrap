namespace Adamant.Tools.Compiler.Bootstrap.CodeGen.Config
{
    public class GrammarProperty
    {
        public string Name { get; }
        public GrammarType Type { get; }

        public GrammarProperty(string name, GrammarType type)
        {
            Name = name;
            Type = type;
        }
    }
}
