namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    /// <summary>
    /// A walker for <see cref="Adamant.Tools.Compiler.Bootstrap.Parsing.Tree.TypeSyntax"/> trees.
    /// </summary>
    public interface ITypeWalker
    {
        bool ShouldSkip(ITypeSyntax type);

        void Enter(IMutableTypeSyntax mutableType);
        void Exit(IMutableTypeSyntax mutableType);

        void Enter(IReferenceLifetimeSyntax referenceLifetime);
        void Exit(IReferenceLifetimeSyntax referenceLifetime);

        void Enter(ISelfTypeSyntax selfType);
        void Exit(ISelfTypeSyntax selfType);

        void Enter(ITypeNameSyntax typeName);
        void Exit(ITypeNameSyntax typeName);
    }
}
