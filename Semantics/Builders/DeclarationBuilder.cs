using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Builders
{
    public class DeclarationBuilder
    {
        private readonly Dictionary<ISymbol, Declaration> declarations = new Dictionary<ISymbol, Declaration>();

        public IEnumerable<Declaration> AllDeclarations => declarations.Values;

        public void Build(IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            foreach (var memberDeclaration in memberDeclarations)
                Build(memberDeclaration);
        }

        private FixedList<Declaration> BuildList(IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            return memberDeclarations.Select(Build).ToFixedList();
        }

        private Declaration Build(MemberDeclarationSyntax memberDeclarations)
        {
            if (declarations.TryGetValue(memberDeclarations, out var declaration))
                return declaration;

            switch (memberDeclarations)
            {
                case INamedFunctionDeclarationSyntax namedFunction:
                    declaration = new FunctionDeclaration(namedFunction.IsExternalFunction,
                        namedFunction.DeclaringType != null, namedFunction.FullName, BuildParameters(namedFunction.Parameters),
                        namedFunction.ReturnType.Known(), namedFunction.ControlFlow);
                    break;
                case IConstructorDeclarationSyntax constructorDeclaration:
                    //var constructorType = (FunctionType)constructorDeclaration.Type.Known();
                    var parameters = BuildConstructorParameters(constructorDeclaration);
                    //constructorType = new FunctionType(parameters.Select(p => p.Type),
                    //    constructorType.ReturnType);
                    declaration = new ConstructorDeclaration(constructorDeclaration.FullName,
                        /*constructorType,*/ parameters, constructorDeclaration.ReturnType.Known(),
                        constructorDeclaration.ControlFlow);
                    break;
                case FieldDeclarationSyntax fieldDeclaration:
                    declaration = new FieldDeclaration(fieldDeclaration.IsMutableBinding, fieldDeclaration.FullName, fieldDeclaration.Type.Known());
                    break;
                default:
                    throw NonExhaustiveMatchException.For(memberDeclarations);
            }
            declarations.Add(memberDeclarations, declaration);
            return declaration;
        }

        private FixedList<Declaration> BuildClassMembers(ClassDeclarationSyntax classDeclaration)
        {
            var members = BuildList(classDeclaration.Members);
            if (members.Any(m => m is ConstructorDeclaration))
                return members;

            var defaultConstructor = BuildDefaultConstructor(classDeclaration);
            return members.Append(defaultConstructor).ToFixedList();
        }

        private Declaration BuildDefaultConstructor(
            ClassDeclarationSyntax classDeclaration)
        {
            var symbol = classDeclaration.ChildSymbols.Values.SelectMany(l => l)
                            .OfType<DefaultConstructor>().Single();
            if (declarations.TryGetValue(symbol, out var declaration))
                return declaration;

            var className = classDeclaration.FullName;
            var selfType = classDeclaration.DeclaresType.Fulfilled();
            var selfName = className.Qualify(SpecialName.Self);
            var selfParameter = new Parameter(false, selfName, selfType);
            var parameters = selfParameter.Yield().ToFixedList();
            //var constructorType = new FunctionType(selfType.Yield(), selfType);

            var graph = new ControlFlowGraphBuilder();
            graph.AddSelfParameter(selfType);
            var block = graph.NewBlock();
            block.AddReturn(classDeclaration.NameSpan, Scope.Outer);

            var defaultConstructor = new ConstructorDeclaration(
                                            symbol.FullName,
                                            //constructorType,
                                            parameters,
                                            selfType,
                                            graph.Build());

            defaultConstructor.ControlFlow.InsertedDeletes = new InsertedDeletes();
            declarations.Add(symbol, defaultConstructor);
            return defaultConstructor;
        }

        //private static FixedList<GenericParameter> BuildGenericParameters(FixedList<GenericParameterSyntax> parameters)
        //{
        //    if (parameters == null) return null;
        //    throw new System.NotImplementedException();
        //}

        private static FixedList<Parameter> BuildParameters(FixedList<ParameterSyntax> parameters)
        {
            return parameters.Select(BuildParameter).ToFixedList();
        }

        private static FixedList<Parameter> BuildConstructorParameters(IConstructorDeclarationSyntax constructorDeclaration)
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
                    return new Parameter(namedParameter.IsMutableBinding, namedParameter.Name, namedParameter.Type.Known());
                case SelfParameterSyntax selfParameter:
                    return new Parameter(selfParameter.IsMutableBinding, selfParameter.Name, selfParameter.Type.Known());
                default:
                    throw NonExhaustiveMatchException.For(parameter);
            }
        }
    }
}
