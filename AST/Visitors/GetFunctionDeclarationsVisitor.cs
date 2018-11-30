using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class GetFunctionDeclarationsVisitor : DeclarationVisitor<Void>
    {
        [NotNull] public IReadOnlyList<FunctionDeclarationSyntax> FunctionDeclarations => functionDeclarations;
        [NotNull] private readonly List<FunctionDeclarationSyntax> functionDeclarations = new List<FunctionDeclarationSyntax>();


        public override void VisitFunctionDeclaration(FunctionDeclarationSyntax functionDeclaration, Void args)
        {
            // No need to visit the children, no functions there
            functionDeclarations.Add(functionDeclaration);
        }
    }
}
