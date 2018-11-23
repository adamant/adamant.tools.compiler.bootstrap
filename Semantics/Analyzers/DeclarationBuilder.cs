using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
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
                    case NamedFunctionDeclarationSyntax namedFunction:
                        declarations.Add(new FunctionDeclaration(
                            namedFunction.Name,
                            namedFunction.Type.Resolved(),
                            BuildParameters(namedFunction.Parameters),
                            namedFunction.ReturnType.Resolved(),
                            namedFunction.ControlFlow));
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(namespacedDeclaration);
                }

            return declarations.ToFixedList();
        }

        [NotNull, ItemNotNull]
        private static FixedList<Parameter> BuildParameters([NotNull, ItemNotNull] FixedList<ParameterSyntax> parameters)
        {
            return parameters.Select(BuildParameter).ToFixedList();
        }

        [NotNull]
        private static Parameter BuildParameter([NotNull] ParameterSyntax parameter)
        {
            switch (parameter)
            {
                case NamedParameterSyntax namedParameter:
                    return new Parameter(namedParameter.MutableBinding, namedParameter.Name, namedParameter.Type.Resolved());
                default:
                    throw NonExhaustiveMatchException.For(parameter);
            }
        }
    }
}
