using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class EnumValuePatternSyntax : PatternSyntax
    {
        public SimpleName Name { get; }

        public EnumValuePatternSyntax(string name)
        {
            Name = new SimpleName(name);
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
