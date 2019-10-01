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

        public abstract void Enter(IReferenceLifetimeSyntax referenceLifetime);
        public void Exit(IReferenceLifetimeSyntax referenceLifetime) { }

        public abstract void Enter(ISelfTypeSyntax selfType);
        public void Exit(ISelfTypeSyntax selfType) { }

        public abstract void Enter(ITypeNameSyntax typeName);
        public void Exit(ITypeNameSyntax typeName) { }
    }
}
