using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes
{
    /// <summary>
    /// A lexical scope of variable places used to track when variables come into
    /// and go out of scope.
    /// </summary>
    internal class LexicalVariableScope : VariableScope
    {
        private readonly VariableScope containingScope;

        public LexicalVariableScope(PlaceIdentifierList identifiers, VariableScope containingScope)
            : base(identifiers)
        {
            this.containingScope = containingScope;
        }

        public override CallerVariableScope CallerScope => containingScope.CallerScope;
    }
}
