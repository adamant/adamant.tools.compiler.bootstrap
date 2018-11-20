using Adamant.Tools.Compiler.Bootstrap.AST;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
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
