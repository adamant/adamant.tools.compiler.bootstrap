namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public abstract class TypeWalker : ITypeWalker
    {
        public bool ShouldSkip(ITypeSyntax type)
        {
            return false;
        }

        public abstract void Enter(IMutableTypeSyntax mutableType);
        public void Exit(IMutableTypeSyntax mutableType) { }

        public abstract void Enter(IReferenceLifetimeTypeSyntax referenceLifetimeType);
        public void Exit(IReferenceLifetimeTypeSyntax referenceLifetimeType) { }

        public abstract void Enter(ITypeNameSyntax typeName);
        public void Exit(ITypeNameSyntax typeName) { }
    }
}
