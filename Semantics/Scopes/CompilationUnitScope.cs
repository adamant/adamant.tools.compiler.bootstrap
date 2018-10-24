using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class CompilationUnitScope : LexicalScope
    {
        [NotNull] public new CompilationUnitSyntax Syntax { get; }

        public CompilationUnitScope([NotNull] CompilationUnitSyntax compilationUnit)
            : base(compilationUnit)
        {
            Syntax = compilationUnit;
        }
    }
}
