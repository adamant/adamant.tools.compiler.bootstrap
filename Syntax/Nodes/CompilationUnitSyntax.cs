using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class CompilationUnitSyntax : SyntaxNode, IEquatable<CompilationUnitSyntax>
    {
        [CanBeNull]
        public CompilationUnitNamespaceSyntax Namespace { get; }

        [NotNull]
        public SyntaxList<UsingDirectiveSyntax> UsingDirectives { get; }

        [NotNull]
        public SyntaxList<DeclarationSyntax> Declarations { get; }

        [NotNull]
        public EndOfFileToken EndOfFile { get; }

        public IReadOnlyList<Diagnostic> Diagnostics => EndOfFile.Diagnostics;

        public CompilationUnitSyntax(
            [CanBeNull] CompilationUnitNamespaceSyntax @namespace,
            [NotNull] SyntaxList<UsingDirectiveSyntax> usingDirectives,
            [NotNull] SyntaxList<DeclarationSyntax> declarations,
            [NotNull] EndOfFileToken endOfFile)
        {
            Requires.NotNull(nameof(usingDirectives), usingDirectives);
            Requires.NotNull(nameof(declarations), declarations);
            Namespace = @namespace;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            EndOfFile = endOfFile;
        }

        public void AllDiagnostics([NotNull] List<Diagnostic> list)
        {
            list.AddRange(Diagnostics);
        }

        #region Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as CompilationUnitSyntax);
        }

        public bool Equals(CompilationUnitSyntax other)
        {
            return other != null &&
                   EqualityComparer<CompilationUnitNamespaceSyntax>.Default.Equals(Namespace, other.Namespace) &&
                   EqualityComparer<SyntaxList<UsingDirectiveSyntax>>.Default.Equals(UsingDirectives, other.UsingDirectives) &&
                   EqualityComparer<SyntaxList<DeclarationSyntax>>.Default.Equals(Declarations, other.Declarations) &&
                   EqualityComparer<EndOfFileToken>.Default.Equals(EndOfFile, other.EndOfFile) &&
                   EqualityComparer<IReadOnlyList<Diagnostic>>.Default.Equals(Diagnostics, other.Diagnostics);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Namespace, UsingDirectives, Declarations, EndOfFile, Diagnostics);
        }

        public static bool operator ==(CompilationUnitSyntax syntax1, CompilationUnitSyntax syntax2)
        {
            return EqualityComparer<CompilationUnitSyntax>.Default.Equals(syntax1, syntax2);
        }

        public static bool operator !=(CompilationUnitSyntax syntax1, CompilationUnitSyntax syntax2)
        {
            return !(syntax1 == syntax2);
        }
        #endregion
    }
}
