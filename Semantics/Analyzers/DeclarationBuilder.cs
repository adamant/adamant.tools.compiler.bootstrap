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
        public FixedList<Declaration> Build([NotNull] FixedList<INamespacedDeclarationSyntax> namespacedDeclarations)
        {
            var declarations = new List<Declaration>();
            foreach (var namespacedDeclaration in namespacedDeclarations)
                switch (namespacedDeclaration)
                {
                    //case NamedFunctionDeclarationSyntax namedFunction:
                    //    declarations.Add(new FunctionDeclaration(namedFunction.Name,));
                    //    break;
                    default:
                        throw NonExhaustiveMatchException.For(namespacedDeclaration);
                }

            return declarations.ToFixedList();
        }
    }
}
