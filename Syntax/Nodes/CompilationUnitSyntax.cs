using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitNamespaceSyntax Namespace { get; }
        public IReadOnlyList<UsingDirectiveSyntax> UsingDirectives { get; }
        public IReadOnlyList<DeclarationSyntax> Declarations { get; }
        public EndOfFileToken EndOfFile { get; }
        public IReadOnlyList<Diagnostic> Diagnostics => EndOfFile.Value;

        public CompilationUnitSyntax(
            CompilationUnitNamespaceSyntax @namespace,
            IReadOnlyList<UsingDirectiveSyntax> usingDirectives,
            IReadOnlyList<DeclarationSyntax> declarations,
            EndOfFileToken endOfFile)
        {
            Namespace = @namespace;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            EndOfFile = endOfFile;
        }

        public void AllDiagnostics(List<Diagnostic> list)
        {
            list.AddRange(Diagnostics);
        }
    }
}
