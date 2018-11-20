using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        [MustUseReturnValue]
        [NotNull]
        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var usingDirectives = ParseUsingDirectives();
            var declarations = ParseDeclarations();
            Tokens.Required<IEndOfFileToken>();

            return new CompilationUnitSyntax(
                Tokens.Context.File,
                ParseImplicitNamespaceName(),
                usingDirectives,
                declarations,
                Tokens.Context.Diagnostics.Build());
        }

        [NotNull]
        private RootName ParseImplicitNamespaceName()
        {
            RootName name = GlobalNamespaceName.Instance;
            foreach (var segment in Tokens.Context.File.Reference.Namespace)
                name = name.Qualify(new SimpleName(segment));

            return name;
        }
    }
}
