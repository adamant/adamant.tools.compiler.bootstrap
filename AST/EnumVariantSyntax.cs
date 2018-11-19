using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class EnumVariantSyntax : Syntax
    {
        [NotNull] public SimpleName Name { get; }

        public EnumVariantSyntax([NotNull] string name)
        {
            Name = new SimpleName(name);
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
