using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class DeclarationBuilder
    {
        [NotNull, ItemNotNull]
        public FixedList<Declaration> Build([NotNull, ItemNotNull] IEnumerable<DeclarationSyntax> declarationSyntaxes)
        {
            var declarations = new List<Declaration>();
            foreach (var namespacedDeclaration in declarationSyntaxes.Where(d => !d.Poisoned))
                switch (namespacedDeclaration)
                {
                    case NamedFunctionDeclarationSyntax namedFunction:
                        declarations.Add(new FunctionDeclaration(
                            namedFunction.FullName,
                            namedFunction.Type.Resolved(),
                            BuildParameters(namedFunction.Parameters),
                            namedFunction.ReturnType.Resolved(),
                            namedFunction.ControlFlow));
                        break;
                    case ClassDeclarationSyntax classDeclaration:
                        declarations.Add(new TypeDeclaration(
                            classDeclaration.FullName,
                            classDeclaration.Type.Resolved(),
                            BuildGenericParameters(classDeclaration.GenericParameters),
                            Build(classDeclaration.Members)));
                        break;
                    case ConstructorDeclarationSyntax constructorDeclaration:
                        declarations.Add(new ConstructorDeclaration(
                            constructorDeclaration.FullName,
                            constructorDeclaration.Type.Resolved(),
                            BuildParameters(constructorDeclaration.Parameters),
                            constructorDeclaration.ReturnType.Resolved(),
                            constructorDeclaration.ControlFlow));
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(namespacedDeclaration);
                }

            return declarations.ToFixedList();
        }

        [CanBeNull]
        private FixedList<GenericParameter> BuildGenericParameters([ItemNotNull] [CanBeNull] FixedList<GenericParameterSyntax> parameters)
        {
            if (parameters == null) return null;
            throw new System.NotImplementedException();
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
                case SelfParameterSyntax selfParameter:
                    return new Parameter(selfParameter.MutableBinding, selfParameter.Name, selfParameter.Type.Resolved());
                default:
                    throw NonExhaustiveMatchException.For(parameter);
            }
        }
    }
}
