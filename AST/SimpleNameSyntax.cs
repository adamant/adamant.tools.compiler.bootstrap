using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class SimpleNameSyntax : NameSyntax
    {
        [NotNull] public SimpleName Name { get; }
        [CanBeNull] public ISymbol ReferencedSymbol { get; set; }

        protected SimpleNameSyntax([NotNull] SimpleName name, TextSpan span)
            : base(span)
        {
            Name = name;
        }
    }
}
