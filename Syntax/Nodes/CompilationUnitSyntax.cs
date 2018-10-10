using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class CompilationUnitSyntax : SyntaxNode
    {
        [NotNull] public CodeFile CodeFile { get; }

        [CanBeNull] public CompilationUnitNamespaceSyntax Namespace { get; }
        [NotNull] public SyntaxList<UsingDirectiveSyntax> UsingDirectives { get; }
        [NotNull] public SyntaxList<DeclarationSyntax> Declarations { get; }
        [NotNull] public EndOfFileToken EndOfFile { get; }

        [NotNull] public Diagnostics Diagnostics { get; }

        public CompilationUnitSyntax(
            [NotNull] CodeFile codeFile,
            [CanBeNull] CompilationUnitNamespaceSyntax @namespace,
            [NotNull] SyntaxList<UsingDirectiveSyntax> usingDirectives,
            [NotNull] SyntaxList<DeclarationSyntax> declarations,
            [NotNull] EndOfFileToken endOfFile,
            [NotNull] Diagnostics diagnostics)
        {
            Requires.NotNull(nameof(codeFile), codeFile);
            Requires.NotNull(nameof(usingDirectives), usingDirectives);
            Requires.NotNull(nameof(declarations), declarations);
            Requires.NotNull(nameof(endOfFile), endOfFile);
            Requires.NotNull(nameof(diagnostics), diagnostics);
            Namespace = @namespace;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            EndOfFile = endOfFile;
            Diagnostics = diagnostics;
            CodeFile = codeFile;
        }
    }
}
