using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class AnyPatternSyntax : PatternSyntax
    {
        public SimpleName Name { get; }

        public AnyPatternSyntax(string name)
        {
            Name = new SimpleName(name);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
