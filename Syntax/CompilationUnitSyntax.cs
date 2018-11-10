using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class CompilationUnitSyntax : NonTerminal
    {
        [NotNull] public CodeFile CodeFile { get; }
        [NotNull] public FileNamespaceDeclarationSyntax Namespace { get; }
        [NotNull] public FixedList<Diagnostic> Diagnostics { get; }

        public CompilationUnitSyntax(
            [NotNull] CodeFile codeFile,
            [NotNull] FileNamespaceDeclarationSyntax @namespace,
            [NotNull] FixedList<Diagnostic> diagnostics)
        {
            Requires.NotNull(nameof(codeFile), codeFile);
            Requires.NotNull(nameof(@namespace), @namespace);
            Requires.NotNull(nameof(diagnostics), diagnostics);
            Namespace = @namespace;
            Diagnostics = diagnostics;
            CodeFile = codeFile;
        }
    }
}
