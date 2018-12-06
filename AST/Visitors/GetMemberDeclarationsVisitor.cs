using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class GetMemberDeclarationsVisitor : DeclarationVisitor<Void>
    {
        [NotNull] public IReadOnlyList<MemberDeclarationSyntax> MemberDeclarations => memberDeclarations;
        [NotNull] private readonly List<MemberDeclarationSyntax> memberDeclarations = new List<MemberDeclarationSyntax>();

        public override void VisitMemberDeclaration(MemberDeclarationSyntax memberDeclaration, Void args)
        {
            base.VisitMemberDeclaration(memberDeclaration, args);
            memberDeclarations.Add(memberDeclaration);
        }

        public override void VisitFunctionDeclaration(FunctionDeclarationSyntax functionDeclaration, Void args)
        {
            // No need to visit the children, no members there
        }
    }
}
