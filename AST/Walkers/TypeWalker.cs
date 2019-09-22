namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public abstract class TypeWalker : ITypeWalker
    {
        public bool ShouldSkip(TypeSyntax type)
        {
            return false;
        }

        public abstract void Enter(MutableTypeSyntax mutableType);
        public void Exit(MutableTypeSyntax mutableType) { }

        public abstract void Enter(ReferenceLifetimeSyntax referenceLifetime);
        public void Exit(ReferenceLifetimeSyntax referenceLifetime) { }

        public abstract void Enter(SelfTypeSyntax selfType);
        public void Exit(SelfTypeSyntax selfType) { }

        public abstract void Enter(TypeNameSyntax typeName);
        public void Exit(TypeNameSyntax typeName) { }
    }
}
