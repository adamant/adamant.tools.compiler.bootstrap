using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen
{
    public class DeclarationBuilder
    {
        private readonly ILFactory ilFactory;
        private readonly Dictionary<Symbol, DeclarationIL> declarations = new Dictionary<Symbol, DeclarationIL>();

        public DeclarationBuilder(ILFactory ilFactory)
        {
            this.ilFactory = ilFactory;
        }

        public IEnumerable<DeclarationIL> AllDeclarations => declarations.Values;

        public void Build(IEnumerable<IEntityDeclarationSyntax> entityDeclarations, ISymbolTree symbolTree)
        {
            foreach (var memberDeclaration in entityDeclarations)
                Build(memberDeclaration, symbolTree);
        }

        private FixedList<DeclarationIL> BuildList(
            IEnumerable<IMemberDeclarationSyntax> memberDeclarations,
            ISymbolTree symbolTree)
        {
            return memberDeclarations.Select(e => Build(e, symbolTree)).ToFixedList();
        }

        private DeclarationIL Build(IEntityDeclarationSyntax entityDeclaration, ISymbolTree symbolTree)
        {
            if (declarations.TryGetValue(entityDeclaration.Symbol.Result, out var declaration))
                return declaration;

            switch (entityDeclaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(entityDeclaration);
                case IFunctionDeclarationSyntax function:
                {
                    var il = ilFactory.CreateGraph(function);
                    declaration = new FunctionIL(function.IsExternalFunction, false, function.Symbol.Result, BuildParameters(function.Parameters), il);
                    break;
                }
                case IAssociatedFunctionDeclarationSyntax associatedFunction:
                {
                    var il = ilFactory.CreateGraph(associatedFunction);
                    declaration = new FunctionIL(false, true, associatedFunction.Symbol.Result, BuildParameters(associatedFunction.Parameters), il);
                    break;
                }
                case IConcreteMethodDeclarationSyntax method:
                {
                    var il = ilFactory.CreateGraph(method);
                    declaration = new MethodDeclarationIL(method.Symbol.Result, BuildParameter(method.SelfParameter), BuildParameters(method.Parameters), il);
                    break;
                }
                case IAbstractMethodDeclarationSyntax method:
                {
                    declaration = new MethodDeclarationIL(method.Symbol.Result, BuildParameter(method.SelfParameter), BuildParameters(method.Parameters), null);
                    break;
                }
                case IConstructorDeclarationSyntax constructor:
                {
                    var il = ilFactory.CreateGraph(constructor);
                    var parameters = BuildConstructorParameters(constructor);
                    var fieldInitializations = BuildFieldInitializations(constructor);
                    declaration = new ConstructorIL(constructor.Symbol.Result, parameters, fieldInitializations, il);
                    break;
                }
                case IFieldDeclarationSyntax fieldDeclaration:
                    declaration = new FieldIL(fieldDeclaration.Symbol.Result);
                    break;
                case IClassDeclarationSyntax classDeclaration:
                    declaration = new ClassIL(classDeclaration.Symbol.Result,
                        BuildClassMembers(classDeclaration, symbolTree));
                    break;
            }
            declarations.Add(entityDeclaration.Symbol.Result, declaration);
            return declaration;
        }

        private FixedList<DeclarationIL> BuildClassMembers(
            IClassDeclarationSyntax classDeclaration,
            ISymbolTree symbolTree)
        {
            var members = BuildList(classDeclaration.Members, symbolTree);
            var defaultConstructor = BuildDefaultConstructor(classDeclaration, symbolTree);
            if (!(defaultConstructor is null))
                members = members.Append(defaultConstructor).ToFixedList();
            return members.ToFixedList();
        }

        private DeclarationIL? BuildDefaultConstructor(
            IClassDeclarationSyntax classDeclaration,
            ISymbolTree symbolTree)
        {
            var constructorSymbol = classDeclaration.DefaultConstructorSymbol;
            if (constructorSymbol is null) return null;

            if (declarations.TryGetValue(constructorSymbol, out var declaration))
                return declaration;

            var selfParameterSymbol = symbolTree.Children(constructorSymbol).OfType<SelfParameterSymbol>().Single();
            var selfParameter = new SelfParameterIL(selfParameterSymbol);
            var parameters = selfParameter.Yield().ToFixedList<ParameterIL>();

            var graph = new ControlFlowGraphBuilder(classDeclaration.File);
            graph.AddSelfParameter(selfParameterSymbol);
            var block = graph.NewBlock();
            block.End(new ReturnVoidInstruction(classDeclaration.NameSpan, Scope.Outer));

            //var il = new ControlFlowGraphBuilder(classDeclaration.File);
            //il.AddSelfParameter(selfType);
            //var block = il.NewBlock();
            //block.End(classDeclaration.NameSpan, Scope.Outer);

            var defaultConstructor = new ConstructorIL(// TODO how to get a name
                constructorSymbol,
                                            parameters, FixedList<FieldInitializationIL>.Empty, graph.Build());

            //defaultConstructor.ControlFlowOld.InsertedDeletes = new InsertedDeletes();
            declarations.Add(constructorSymbol, defaultConstructor);
            return defaultConstructor;
        }

        private static FixedList<ParameterIL> BuildParameters(IEnumerable<IParameterSyntax> parameters)
        {
            return parameters.Select(BuildParameter).ToFixedList();
        }

        private static FixedList<ParameterIL> BuildConstructorParameters(IConstructorDeclarationSyntax constructorDeclaration)
        {
            var selfParameterSymbol = constructorDeclaration.ImplicitSelfParameter.Symbol.Result;
            var selfParameter = new SelfParameterIL(selfParameterSymbol);
            return selfParameter.Yield().Concat(constructorDeclaration.Parameters.Select(BuildParameter)).ToFixedList();
        }

        private static ParameterIL BuildParameter(IParameterSyntax parameter)
        {
            return parameter switch
            {
                INamedParameterSyntax namedParameter => new NamedParameterIL(namedParameter.Symbol.Result),
                ISelfParameterSyntax selfParameter => new SelfParameterIL(selfParameter.Symbol.Result),
                IFieldParameterSyntax fieldParameter =>
                     new FieldParameterIL(fieldParameter.ReferencedSymbol.Result ?? throw new InvalidOperationException()),
                _ => throw ExhaustiveMatch.Failed(parameter)
            };
        }

        private static FixedList<FieldInitializationIL> BuildFieldInitializations(
            IConstructorDeclarationSyntax constructorDeclaration)
        {
            return constructorDeclaration.Parameters.OfType<IFieldParameterSyntax>()
                                         .Select(BuildFieldInitialization)
                                         .ToFixedList();
        }

        private static FieldInitializationIL BuildFieldInitialization(IFieldParameterSyntax parameter)
        {
            var field = parameter.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            return new FieldInitializationIL(field);
        }
    }
}
