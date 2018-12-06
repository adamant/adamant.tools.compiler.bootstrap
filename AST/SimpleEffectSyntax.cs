using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class SimpleEffectSyntax : EffectSyntax
    {
        public SimpleName Name { get; }

        public SimpleEffectSyntax(string name)
        {
            Name = new SimpleName(name);
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
