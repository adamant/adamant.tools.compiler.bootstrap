namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    /// <summary>
    /// A walker for <see cref="TypeSyntax"/> trees.
    /// </summary>
    public interface ITypeWalker
    {
        bool ShouldSkip(TypeSyntax type);

        void Enter(MutableTypeSyntax mutableType);
        void Exit(MutableTypeSyntax mutableType);

        void Enter(ReferenceLifetimeSyntax referenceLifetime);
        void Exit(ReferenceLifetimeSyntax referenceLifetime);

        void Enter(SelfTypeSyntax selfType);
        void Exit(SelfTypeSyntax selfType);

        void Enter(TypeNameSyntax typeName);
        void Exit(TypeNameSyntax typeName);
    }
}
