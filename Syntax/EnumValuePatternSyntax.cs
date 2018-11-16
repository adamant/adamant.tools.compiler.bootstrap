using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EnumValuePatternSyntax : PatternSyntax
    {
        [NotNull] public SimpleName Name { get; }

        public EnumValuePatternSyntax([NotNull] string name)
        {
            Name = new SimpleName(name);
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
