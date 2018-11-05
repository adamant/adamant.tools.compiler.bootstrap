using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Fakes
{
    public class FakeLexicalScope : LexicalScope
    {
        public FakeLexicalScope()
            : base(FakeSyntax.IncompleteDeclaration())
        {
        }

        [CanBeNull]
        public override ISymbol Lookup([NotNull] string name)
        {
            throw new System.NotImplementedException();
        }
    }
}
