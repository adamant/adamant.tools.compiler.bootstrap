using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class UsingDirectiveSyntax : Syntax
    {
        // For now, we only support namespace names
        public Name Name { get; }

        public UsingDirectiveSyntax(Name name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"using {Name};";
        }
    }
}
