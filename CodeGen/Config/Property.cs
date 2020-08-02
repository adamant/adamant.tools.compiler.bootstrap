namespace Adamant.Tools.Compiler.Bootstrap.CodeGen.Config
{
    public class Property
    {
        public string Name { get; }
        public Symbol Type { get; }
        public bool IsOptional { get; }
        public bool IsList { get; }

        public Property(string name, Symbol type, bool isOptional, bool isList)
        {
            Name = name;
            Type = type;
            IsList = isList;
            IsOptional = isOptional;
        }
    }
}
