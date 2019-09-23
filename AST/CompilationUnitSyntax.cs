using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class CompilationUnitSyntax : Syntax
    {
        public CodeFile CodeFile { get; }
        public RootName ImplicitNamespaceName { get; }
        public FixedList<UsingDirectiveSyntax> UsingDirectives { get; }
        public FixedList<DeclarationSyntax> Declarations { get; }
        public FixedList<MemberDeclarationSyntax> AllMemberDeclarations { get; }
        public FixedList<Diagnostic> Diagnostics { get; }

        public CompilationUnitSyntax(
            RootName implicitNamespaceName,
            TextSpan span,
            CodeFile codeFile,
            FixedList<UsingDirectiveSyntax> usingDirectives,
            FixedList<DeclarationSyntax> declarations,
            FixedList<Diagnostic> diagnostics)
            : base(span)
        {
            CodeFile = codeFile;
            ImplicitNamespaceName = implicitNamespaceName;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            var visitor = new GetMemberDeclarationsVisitor();
            visitor.VisitDeclarations(declarations);
            AllMemberDeclarations = visitor.MemberDeclarations.ToFixedList();
            Diagnostics = diagnostics;
        }

        public override string ToString()
        {
            return CodeFile.Reference.ToString();
        }
    }
}
