using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class CompilationUnitSyntax : NonTerminal
    {
        [NotNull] public CodeFile CodeFile { get; }
        [NotNull] public FileNamespaceDeclarationSyntax Namespace { get; }
        [NotNull] public IEndOfFileToken EndOfFile { get; }
        [NotNull] public FixedList<Diagnostic> Diagnostics { get; }

        public CompilationUnitSyntax(
            [NotNull] CodeFile codeFile,
            [NotNull] FileNamespaceDeclarationSyntax @namespace,
            [NotNull] IEndOfFileToken endOfFile,
            [NotNull] FixedList<Diagnostic> diagnostics)
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
