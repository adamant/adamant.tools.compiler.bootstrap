using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class GetFunctionDeclarationsVisitor : DeclarationVisitor<Void>
    {
        public IReadOnlyList<FunctionDeclarationSyntax> FunctionDeclarations => functionDeclarations;
        private readonly List<FunctionDeclarationSyntax> functionDeclarations = new List<FunctionDeclarationSyntax>();

        public override void VisitFunctionDeclaration(FunctionDeclarationSyntax functionDeclaration, Void args)
        {
            // No need to visit the children, no functions there
            functionDeclarations.Add(functionDeclaration);
        }
    }
}
