using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.NotImplemented
{
    /// <summary>
    /// Reports errors for syntax that the parsing supports but the semantic
    /// analyzer doesn't.
    /// </summary>
    public class SemanticsNotImplementedChecker : SyntaxWalker
    {
        private readonly Diagnostics diagnostics;
        private CodeFile file;

        public SemanticsNotImplementedChecker(Diagnostics diagnostics)
        {
            this.diagnostics = diagnostics;
            file = null!;
        }

        public void Check(PackageSyntax packageSyntax)
        {
            foreach (var compilationUnit in packageSyntax.CompilationUnits)
            {
                file = compilationUnit.CodeFile;
                WalkNonNull(compilationUnit);
            }
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case INamedParameterSyntax syn:
                    if (!(syn.DefaultValue is null))
                        diagnostics.Add(SemanticError.NotImplemented(file, syntax.Span, "Default values"));
                    break;
                case IFieldParameterSyntax syn:
                    if (!(syn.DefaultValue is null))
                        diagnostics.Add(SemanticError.NotImplemented(file, syntax.Span, "Default values"));
                    break;
                case IConstructorDeclarationSyntax syn:
                    if (syn.Name != SpecialName.New)
                        diagnostics.Add(SemanticError.NotImplemented(file, syntax.Span, "Named constructors"));
                    break;

                case IEntityDeclarationSyntax syn:
                    if (!(syn is IClassDeclarationSyntax))
                    {
                        if (syn.Modifiers.OfType<IMutableKeywordToken>().Any())
                            diagnostics.Add(SemanticError.NotImplemented(file, syntax.Span, "`mut` modifiers"));

                        // TODO there should be more complex rules for classes
                        if (syn.Modifiers.Count > 1)
                            diagnostics.Add(SemanticError.NotImplemented(file, syntax.Span, "Multiple modifiers"));
                    }

                    if (syn.Modifiers.OfType<IMoveKeywordToken>().Any())
                        diagnostics.Add(SemanticError.NotImplemented(file, syntax.Span, "`move` modifiers"));

                    if (syn.Modifiers.OfType<ISafeKeywordToken>().Any())
                        diagnostics.Add(SemanticError.NotImplemented(file, syntax.Span, "`safe` modifiers"));
                    if (syn.Modifiers.OfType<IUnsafeKeywordToken>().Any())
                        diagnostics.Add(SemanticError.NotImplemented(file, syntax.Span, "`unsafe` modifiers"));
                    break;
            }
            WalkChildren(syntax);
        }
    }
}
