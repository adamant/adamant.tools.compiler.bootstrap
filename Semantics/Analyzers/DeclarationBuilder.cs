using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Model;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class DeclarationBuilder
    {
        [NotNull, ItemNotNull]
        public FixedList<Declaration> Build([NotNull] FixedList<INonMemberDeclarationSyntax> nonMemberDeclarations)
        {
            var declarations = new List<Declaration>();
            foreach (var nonMemberDeclaration in nonMemberDeclarations)
                switch (nonMemberDeclaration)
                {
                    //case NamedFunctionDeclarationSyntax namedFunction:
                    //    declarations.Add(new FunctionDeclaration(namedFunction.Name,));
                    //    break;
                    default:
                        throw NonExhaustiveMatchException.For(nonMemberDeclaration);
                }

            return declarations.ToFixedList();
        }
    }
}
