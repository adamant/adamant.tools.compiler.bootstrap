using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class CompilationUnitSyntax : Syntax
    {
        public CodeFile CodeFile { get; }
        public RootName ImplicitNamespaceName { get; }
        public FixedList<UsingDirectiveSyntax> UsingDirectives { get; }
        public FixedList<IDeclarationSyntax> Declarations { get; }
        public FixedList<IEntityDeclarationSyntax> EntityDeclarations { get; }
        public FixedList<Diagnostic> Diagnostics { get; }

        public CompilationUnitSyntax(
            RootName implicitNamespaceName,
            TextSpan span,
            CodeFile codeFile,
            FixedList<UsingDirectiveSyntax> usingDirectives,
            FixedList<IDeclarationSyntax> declarations,
            FixedList<Diagnostic> diagnostics)
            : base(span)
        {
            CodeFile = codeFile;
            ImplicitNamespaceName = implicitNamespaceName;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            EntityDeclarations = GetEntityDeclarations(declarations).ToFixedList();
            Diagnostics = diagnostics;
        }

        public override string ToString()
        {
            return CodeFile.Reference.ToString();
        }

        private IEnumerable<IEntityDeclarationSyntax> GetEntityDeclarations(IEnumerable<IDeclarationSyntax> declarations)
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
