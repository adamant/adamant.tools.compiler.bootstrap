using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics
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
