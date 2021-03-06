using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class UsingDirectiveSyntax : Syntax, IUsingDirectiveSyntax
    {
        // For now, we only support namespace names
        public NamespaceName Name { get; }

        public UsingDirectiveSyntax(TextSpan span, NamespaceName name)
            : base(span)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"using {Name};";
        }
    }
}
