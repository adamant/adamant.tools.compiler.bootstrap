using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class EnumVariantSyntax : Syntax
    {
        public SimpleName Name { get; }

        public EnumVariantSyntax(string name)
        {
            Name = new SimpleName(name);
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
