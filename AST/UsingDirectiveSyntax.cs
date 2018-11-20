using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class UsingDirectiveSyntax : Syntax
    {
        // For now, we only support namespace names
        [NotNull] public Name Name { get; }

        public UsingDirectiveSyntax([NotNull] Name name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"using {Name};";
        }
    }
}
