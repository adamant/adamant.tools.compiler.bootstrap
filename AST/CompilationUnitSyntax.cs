using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class CompilationUnitSyntax : NonTerminal
    {
        [NotNull] public CodeFile CodeFile { get; }
        [NotNull] public NamespaceDeclarationSyntax FileNamespace { get; }
        [NotNull] public FixedList<Diagnostic> Diagnostics { get; }

        public CompilationUnitSyntax(
            [NotNull] CodeFile codeFile,
            [NotNull] NamespaceDeclarationSyntax fileNamespace,
            [NotNull] FixedList<Diagnostic> diagnostics)
        {
            Requires.NotNull(nameof(codeFile), codeFile);
            Requires.NotNull(nameof(fileNamespace), fileNamespace);
            Requires.NotNull(nameof(diagnostics), diagnostics);
            FileNamespace = fileNamespace;
            Diagnostics = diagnostics;
            CodeFile = codeFile;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
