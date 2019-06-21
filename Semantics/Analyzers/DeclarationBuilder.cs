using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class DeclarationBuilder
    {
        public FixedList<Declaration> Build(IEnumerable<DeclarationSyntax> declarationSyntaxes)
        {
            var declarations = new List<Declaration>();
            foreach (var namespacedDeclaration in declarationSyntaxes)
                switch (namespacedDeclaration)
                {
                    case NamedFunctionDeclarationSyntax namedFunction:
                        declarations.Add(new FunctionDeclaration(
                            namedFunction.IsExternalFunction,
                            namedFunction.DeclaringType != null,
                            namedFunction.FullName,
                            namedFunction.Type.Known(),
                            BuildParameters(namedFunction.Parameters),
                            namedFunction.ReturnType.Known(),
                            namedFunction.ControlFlow));
                        break;
                    case ClassDeclarationSyntax classDeclaration:
                        declarations.Add(new TypeDeclaration(
                            classDeclaration.FullName,
                            classDeclaration.Type.Known(),
                            BuildGenericParameters(classDeclaration.GenericParameters),
                            BuildClassMembers(classDeclaration, declarations)));
                        break;
                    case ConstructorDeclarationSyntax constructorDeclaration:
                    {
                        var constructorType = (FunctionType)constructorDeclaration.Type.Known();
                        var parameters = BuildConstructorParameters(constructorDeclaration);
                        constructorType = new FunctionType(parameters.Select(p => p.Type), constructorType.ReturnType);
                        declarations.Add(new ConstructorDeclaration(constructorDeclaration.FullName,
                            constructorType,
                            parameters,
                            constructorDeclaration.ReturnType.Known(),
                            constructorDeclaration.ControlFlow));
                    }
                    break;
                    default:
                        throw NonExhaustiveMatchException.For(namespacedDeclaration);
                }

            return declarations.ToFixedList();
        }

        private FixedList<Declaration> BuildClassMembers(
            ClassDeclarationSyntax classDeclaration,
            List<Declaration> declarations)
        {
            var members = Build(classDeclaration.Members);
            if (members.Any(m => m is ConstructorDeclaration)) return members;

            var defaultConstructor = BuildDefaultConstructor(classDeclaration);
            members = members.Append(defaultConstructor).ToFixedList();
            // We have to add it to the list of all declarations too, or it won't be found by the emitter
            declarations.Add(defaultConstructor);
            return members;
        }

        private static ConstructorDeclaration BuildDefaultConstructor(
            ClassDeclarationSyntax classDeclaration)
        {
            var className = classDeclaration.FullName;
            var constructorName = className.Qualify(SpecialName.New);
            var selfType = ((Metatype)classDeclaration.Type.Known()).Instance;
            var selfName = className.Qualify(SpecialName.Self);
            var selfParameter = new Parameter(false, selfName, selfType);
            var parameters = selfParameter.Yield().ToFixedList();
            var constructorType = new FunctionType(selfType.Yield(), selfType);

            var graph = new ControlFlowGraphBuilder();
            graph.AddSelfParameter(selfType);
            var block = graph.NewBlock();
            block.AddReturn(classDeclaration.NameSpan, Scope.Outer);

            var defaultConstructor = new ConstructorDeclaration(constructorName, constructorType,
                parameters, selfType, graph.Build());
            return defaultConstructor;
        }

        private static FixedList<GenericParameter> BuildGenericParameters(FixedList<GenericParameterSyntax> parameters)
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
            var selfParameter = new Parameter(false, selfName, selfType);
            return selfParameter.Yield().Concat(constructorDeclaration.Parameters.Select(BuildParameter))
                .ToFixedList();
        }

        private static Parameter BuildParameter(ParameterSyntax parameter)
        {
            switch (parameter)
            {
                case NamedParameterSyntax namedParameter:
                    return new Parameter(namedParameter.MutableBinding, namedParameter.Name, namedParameter.Type.Known());
                case SelfParameterSyntax selfParameter:
                    return new Parameter(selfParameter.MutableBinding, selfParameter.Name, selfParameter.Type.Known());
                default:
                    throw NonExhaustiveMatchException.For(parameter);
            }
        }
    }
}
