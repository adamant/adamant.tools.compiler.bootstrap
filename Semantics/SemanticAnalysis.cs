using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        public PackageSyntax PackageSyntax { get; }
        private readonly SemanticAttributes attributes;

        internal SemanticAnalysis(PackageSyntax packageSyntax)
        {
            PackageSyntax = packageSyntax;
            attributes = new SemanticAttributes(packageSyntax);
            AddParentAttributes(packageSyntax);
        }
    }
}
