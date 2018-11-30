using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class CompilationUnitSyntax : Syntax
    {
        [NotNull] public CodeFile CodeFile { get; }
        [NotNull] public RootName ImplicitNamespaceName { get; }
        [NotNull, ItemNotNull] public FixedList<UsingDirectiveSyntax> UsingDirectives { get; }
        [NotNull, ItemNotNull] public FixedList<DeclarationSyntax> Declarations { get; }
        [NotNull, ItemNotNull] public FixedList<INamespacedDeclarationSyntax> AllNamespacedDeclarations { get; }
        [NotNull, ItemNotNull] public FixedList<Diagnostic> Diagnostics { get; }

        public CompilationUnitSyntax(
            [NotNull] CodeFile codeFile,
            [NotNull] RootName implicitNamespaceName,
            [NotNull, ItemNotNull] FixedList<UsingDirectiveSyntax> usingDirectives,
            [NotNull, ItemNotNull] FixedList<DeclarationSyntax> declarations,
            [NotNull, ItemNotNull] FixedList<Diagnostic> diagnostics)
        {
            CodeFile = codeFile;
            ImplicitNamespaceName = implicitNamespaceName;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            AllNamespacedDeclarations = GetAllNamespacedDeclarations(declarations);
            Diagnostics = diagnostics;
        }

        [NotNull]
        private static FixedList<INamespacedDeclarationSyntax> GetAllNamespacedDeclarations(
            [NotNull, ItemNotNull] FixedList<DeclarationSyntax> declarations)
        {
            var memberDeclarations = new List<INamespacedDeclarationSyntax>();
            memberDeclarations.AddRange(declarations.OfType<INamespacedDeclarationSyntax>());
            var namespaces = new Queue<NamespaceDeclarationSyntax>();
            namespaces.EnqueueRange(declarations.OfType<NamespaceDeclarationSyntax>());
            while (namespaces.TryDequeue(out var ns))
            {
                memberDeclarations.AddRange(ns.Declarations.OfType<INamespacedDeclarationSyntax>());
                namespaces.EnqueueRange(ns.Declarations.OfType<NamespaceDeclarationSyntax>());
            }

            return memberDeclarations.ToFixedList();
        }

        public override string ToString()
        {
            return CodeFile.Reference.ToString().NotNull();
        }
    }
}
