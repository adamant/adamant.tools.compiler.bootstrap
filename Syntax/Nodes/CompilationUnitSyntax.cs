using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class CompilationUnitSyntax : SyntaxNode
    {
        [NotNull] public CodeFile CodeFile { get; }
        [NotNull] public FileNamespaceDeclarationSyntax Namespace { get; }
        [NotNull] public EndOfFileToken EndOfFile { get; }
        [NotNull] public Diagnostics Diagnostics { get; }

        public CompilationUnitSyntax(
            [NotNull] CodeFile codeFile,
            [NotNull] FileNamespaceDeclarationSyntax @namespace,
            [NotNull] EndOfFileToken endOfFile,
            [NotNull] Diagnostics diagnostics)
        {
            Requires.NotNull(nameof(codeFile), codeFile);
            Requires.NotNull(nameof(@namespace), @namespace);
            Requires.NotNull(nameof(endOfFile), endOfFile);
            Requires.NotNull(nameof(diagnostics), diagnostics);
            Namespace = @namespace;
            EndOfFile = endOfFile;
            Diagnostics = diagnostics;
            CodeFile = codeFile;
        }
    }
}
