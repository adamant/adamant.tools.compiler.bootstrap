namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public abstract class TypeWalker : ITypeWalker
    {
        public bool ShouldSkip(ITypeSyntax type)
        {
            return false;
        }

        public abstract void Enter(IMutableTypeSyntax mutableType);
        public virtual void Exit(IMutableTypeSyntax mutableType) { }

        public abstract void Enter(IReferenceLifetimeTypeSyntax referenceLifetimeType);
        public virtual void Exit(IReferenceLifetimeTypeSyntax referenceLifetimeType) { }

        public abstract void Enter(ITypeNameSyntax typeName);
        public virtual void Exit(ITypeNameSyntax typeName) { }
    }
}
