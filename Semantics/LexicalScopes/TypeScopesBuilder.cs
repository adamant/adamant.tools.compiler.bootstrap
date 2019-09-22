using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    /// <summary>
    /// Assigns containing scopes to <see cref="TypeSyntax"/> nodes.
    /// </summary>
    public class TypeScopesBuilder : TypeWalker
    {
        private readonly LexicalScope containingScope;

        public TypeScopesBuilder(LexicalScope containingScope)
        {
            this.containingScope = containingScope;
        }

        public override void Enter(MutableTypeSyntax mutableType)
        {
        }

        public override void Enter(ReferenceLifetimeSyntax referenceLifetime)
        {
        }

        public override void Enter(SelfTypeSyntax selfType)
        {
        }

        public override void Enter(TypeNameSyntax typeName)
        {
            typeName.ContainingScope = containingScope;
        }
    }
}
