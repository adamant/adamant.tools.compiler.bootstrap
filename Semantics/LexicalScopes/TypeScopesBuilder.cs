using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    /// <summary>
    /// Assigns containing scopes to <see cref="ITypeSyntax"/> nodes.
    /// </summary>
    public class TypeScopesBuilder : TypeWalker
    {
        private readonly LexicalScope containingScope;

        public TypeScopesBuilder(LexicalScope containingScope)
        {
            this.containingScope = containingScope;
        }

        public override void Enter(IMutableTypeSyntax mutableType)
        {
        }

        public override void Enter(IReferenceLifetimeTypeSyntax referenceLifetimeType)
        {
        }

        public override void Enter(ITypeNameSyntax typeName)
        {
            typeName.ContainingScope = containingScope;
        }
    }
}
