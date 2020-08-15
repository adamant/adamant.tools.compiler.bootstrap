using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class CompilationUnitSyntax : Syntax, ICompilationUnitSyntax
    {
        public CodeFile File { get; }
        public NamespaceName ImplicitNamespaceName { get; }
        public FixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
        public FixedList<INonMemberDeclarationSyntax> Declarations { get; }
        public FixedList<IEntityDeclarationSyntax> AllEntityDeclarations { get; }
        public FixedList<Diagnostic> Diagnostics { get; private set; }

        public CompilationUnitSyntax(
            NamespaceName implicitNamespaceName,
            TextSpan span,
            CodeFile file,
            FixedList<IUsingDirectiveSyntax> usingDirectives,
            FixedList<INonMemberDeclarationSyntax> declarations)
            : base(span)
        {
            File = file;
            ImplicitNamespaceName = implicitNamespaceName;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            AllEntityDeclarations = GetEntityDeclarations(declarations).ToFixedList();
            Diagnostics = FixedList<Diagnostic>.Empty;
        }

        public void Attach(FixedList<Diagnostic> diagnostics)
        {
            Diagnostics = diagnostics;
        }

        public override string ToString()
        {
            return File.Reference.ToString();
        }

        private static IEnumerable<IEntityDeclarationSyntax> GetEntityDeclarations(IEnumerable<IDeclarationSyntax> declarations)
        {
            var queue = new Queue<IDeclarationSyntax>();
            queue.EnqueueRange(declarations);

            while (queue.Any())
            {
                var declaration = queue.Dequeue();
                switch (declaration)
                {
                    default:
                        throw ExhaustiveMatch.Failed(declaration);
                    case IMemberDeclarationSyntax member:
                        yield return member;
                        break;
                    case IFunctionDeclarationSyntax function:
                        yield return function;
                        break;
                    case INamespaceDeclarationSyntax ns:
                        queue.EnqueueRange(ns.Declarations);
                        break;
                    case IClassDeclarationSyntax classDeclaration:
                        yield return classDeclaration;
                        queue.EnqueueRange(classDeclaration.Members);
                        break;
                }
            }
        }
    }
}
