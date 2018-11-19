using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class AnyPatternSyntax : PatternSyntax
    {
        [NotNull] public SimpleName Name { get; }

        public AnyPatternSyntax([NotNull] string name)
        {
            Name = new SimpleName(name);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
