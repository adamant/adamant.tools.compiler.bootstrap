using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Functions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class FunctionScope : NestedScope
    {
        [NotNull] public new NamedFunctionDeclarationSyntax Syntax { get; }

        public FunctionScope(
            [NotNull] LexicalScope containingScope,
            [NotNull] NamedFunctionDeclarationSyntax syntax)
            : base(containingScope, syntax)
        {
            Syntax = syntax;
        }
    }
}
