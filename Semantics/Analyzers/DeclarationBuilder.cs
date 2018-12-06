using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class DeclarationBuilder
    {
        public FixedList<Declaration> Build(IEnumerable<DeclarationSyntax> declarationSyntaxes)
        {
            var declarations = new List<Declaration>();
            foreach (var namespacedDeclaration in declarationSyntaxes.Where(d => !d.Poisoned))
                switch (namespacedDeclaration)
                {
                    case NamedFunctionDeclarationSyntax namedFunction:
                        declarations.Add(new FunctionDeclaration(
                            namedFunction.IsExternalFunction,
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
                    {
                        var constructorType = (FunctionType)constructorDeclaration.Type.Resolved();
                        var parameters = BuildConstructorParameters(constructorDeclaration);
                        constructorType = new FunctionType(parameters.Select(p => p.Type), constructorType.ReturnType);
                        declarations.Add(new ConstructorDeclaration(constructorDeclaration.FullName,
                            constructorType,
                            parameters,
                            constructorDeclaration.ReturnType.Resolved(),
                            constructorDeclaration.ControlFlow));
                    }
                    break;
                    default:
                        throw NonExhaustiveMatchException.For(namespacedDeclaration);
                }

            return declarations.ToFixedList();
        }

        private FixedList<GenericParameter> BuildGenericParameters(FixedList<GenericParameterSyntax> parameters)
        {
            if (parameters == null) return null;
            throw new System.NotImplementedException();
        }

        private static FixedList<Parameter> BuildParameters(FixedList<ParameterSyntax> parameters)
        {
            return parameters.Select(BuildParameter).ToFixedList();
        }

        private static FixedList<Parameter> BuildConstructorParameters(ConstructorDeclarationSyntax constructorDeclaration)
        {
            var selfType = constructorDeclaration.SelfParameterType;
            var selfName = ((QualifiedName)constructorDeclaration.FullName).Qualifier.Qualify(SpecialName.Self);
            var selfParameter = new Parameter(true, selfName, selfType);
            return selfParameter.Yield().Concat(constructorDeclaration.Parameters.Select(BuildParameter))
                .ToFixedList();
        }

        private static Parameter BuildParameter(ParameterSyntax parameter)
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
