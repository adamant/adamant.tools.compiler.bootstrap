using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        [NotNull] public PackageSyntax PackageSyntax { get; }
        [NotNull] private readonly SemanticAttributes attributes;

        internal SemanticAnalysis([NotNull] PackageSyntax packageSyntax)
        {
            Requires.NotNull(nameof(packageSyntax), packageSyntax);
            PackageSyntax = packageSyntax;
            attributes = new SemanticAttributes(packageSyntax);
            AddParentAttribute(packageSyntax);
        }
    }
}
