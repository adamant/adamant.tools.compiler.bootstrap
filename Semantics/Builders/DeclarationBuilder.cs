using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Builders
{
    public class DeclarationBuilder
    {
        private readonly ILFactory ilFactory;
        private readonly Dictionary<ISymbol, Declaration> declarations = new Dictionary<ISymbol, Declaration>();

        public DeclarationBuilder(ILFactory ilFactory)
        {
            this.ilFactory = ilFactory;
        }

        public IEnumerable<Declaration> AllDeclarations => declarations.Values;

        public void Build(IEnumerable<IEntityDeclarationSyntax> entityDeclarations)
        {
            foreach (var memberDeclaration in entityDeclarations)
                Build(memberDeclaration);
        }

        private FixedList<Declaration> BuildList(IEnumerable<IMemberDeclarationSyntax> memberDeclarations)
        {
            return memberDeclarations.Select(Build).ToFixedList();
        }

        private Declaration Build(IEntityDeclarationSyntax entityDeclaration)
        {
            if (declarations.TryGetValue(entityDeclaration, out var declaration))
                return declaration;

            switch (entityDeclaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(entityDeclaration);
                case IFunctionDeclarationSyntax function:
                {
                    var il = ilFactory.CreateGraph(function);
                    declaration = new FunctionDeclaration(function.IsExternalFunction, false,
                        function.FullName, BuildParameters(function.Parameters),
                        function.ReturnType.Known(), il);
                    break;
                }
                case IAssociatedFunctionDeclaration associatedFunction:
                {
                    var il = ilFactory.CreateGraph(associatedFunction);
                    declaration = new FunctionDeclaration(false, true,
                        associatedFunction.FullName, BuildParameters(associatedFunction.Parameters),
                        associatedFunction.ReturnType.Known(), il);
                    break;
                }
                case IConcreteMethodDeclarationSyntax method:
                {
                    var il = ilFactory.CreateGraph(method);
                    declaration = new FunctionDeclaration(false, true,
                        method.FullName, BuildParameters(method.Parameters),
                        method.ReturnType.Known(), il);
                    break;
                }
                case IAbstractMethodDeclarationSyntax method:
                {
                    declaration = new FunctionDeclaration(false, true,
                        method.FullName, BuildParameters(method.Parameters),
                        method.ReturnType.Known(), null);
                    break;
                }
                case IConstructorDeclarationSyntax constructor:
                {
                    var il = ilFactory.CreateGraph(constructor);
                    var parameters = BuildConstructorParameters(constructor);
                    declaration = new ConstructorDeclaration(constructor.FullName,
                       parameters, constructor.SelfParameterType.Known(), il);
                    break;
                }
                case IFieldDeclarationSyntax fieldDeclaration:
                    declaration = new FieldDeclaration(fieldDeclaration.IsMutableBinding, fieldDeclaration.FullName, fieldDeclaration.Type.Known());
                    break;
                case IClassDeclarationSyntax classDeclaration:
                    declaration = new ClassDeclaration(classDeclaration.FullName,
                        classDeclaration.DeclaresType.Known(),
                        BuildClassMembers(classDeclaration));
                    break;
            }
            declarations.Add(entityDeclaration, declaration);
            return declaration;
        }

        private FixedList<Declaration> BuildClassMembers(IClassDeclarationSyntax classDeclaration)
        {
            var members = BuildList(classDeclaration.Members);
            if (members.Any(m => m is ConstructorDeclaration))
                return members;

            var defaultConstructor = BuildDefaultConstructor(classDeclaration);
            return members.Append(defaultConstructor).ToFixedList();
        }

        private Declaration BuildDefaultConstructor(IClassDeclarationSyntax classDeclaration)
        {
            var symbol = classDeclaration.ChildSymbols.ImplicitCast<ISymbol>()
                            .OfType<DefaultConstructor>().Single();
            if (declarations.TryGetValue(symbol, out var declaration))
                return declaration;

            var className = classDeclaration.FullName;
            var selfType = classDeclaration.DeclaresType.Fulfilled();
            var selfName = className.Qualify(SpecialName.Self);
            var selfParameter = new Parameter(false, selfName, selfType);
            var parameters = selfParameter.Yield().ToFixedList();

            var graph = new ControlFlowGraphBuilder(classDeclaration.File);
            graph.AddSelfParameter(selfType);
            var block = graph.NewBlock();
            block.End(new ReturnVoidInstruction(classDeclaration.NameSpan, Scope.Outer));

            //var il = new ControlFlowGraphBuilder(classDeclaration.File);
            //il.AddSelfParameter(selfType);
            //var block = il.NewBlock();
            //block.End(classDeclaration.NameSpan, Scope.Outer);

            var defaultConstructor = new ConstructorDeclaration(
                                            symbol.FullName,
                                            parameters,
                                            selfType,
                                            graph.Build());

            //defaultConstructor.ControlFlowOld.InsertedDeletes = new InsertedDeletes();
            declarations.Add(symbol, defaultConstructor);
            return defaultConstructor;
        }

        private static FixedList<Parameter> BuildParameters(IEnumerable<IParameterSyntax> parameters)
        {
            return parameters.Select(BuildParameter).ToFixedList();
        }

        private static FixedList<Parameter> BuildConstructorParameters(IConstructorDeclarationSyntax constructorDeclaration)
        {
            var selfType = constructorDeclaration.SelfParameterType.Known();
            var selfName = ((QualifiedName)constructorDeclaration.FullName).Qualifier.Qualify(SpecialName.Self);
            var selfParameter = new Parameter(false, selfName, selfType);
            return selfParameter.Yield().Concat(constructorDeclaration.Parameters.Select(BuildParameter))
                .ToFixedList();
        }

        private static Parameter BuildParameter(IParameterSyntax parameter)
        {
            switch (parameter)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter);
                case INamedParameterSyntax namedParameter:
                    return new Parameter(namedParameter.IsMutableBinding, namedParameter.Name, namedParameter.Type.Known());
                case ISelfParameterSyntax selfParameter:
                    return new Parameter(selfParameter.IsMutableBinding, selfParameter.Name, selfParameter.Type.Known());
                case IFieldParameterSyntax _:
                    throw new NotImplementedException("Building field parameters not implemented");
            }
        }
    }
}
