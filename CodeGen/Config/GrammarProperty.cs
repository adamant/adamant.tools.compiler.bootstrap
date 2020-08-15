namespace Adamant.Tools.Compiler.Bootstrap.CodeGen.Config
{
    public class GrammarProperty
    {
        public string Name { get; }
        public GrammarSymbol Type { get; }
        public bool IsOptional { get; }
        public bool IsList { get; }

        public GrammarProperty(string name, GrammarSymbol type, bool isOptional, bool isList)
        {
            Name = name;
            Type = type;
            IsList = isList;
            IsOptional = isOptional;
        }
    }
}
