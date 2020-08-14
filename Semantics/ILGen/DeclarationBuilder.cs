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
        private readonly Dictionary<Symbol, Declaration> declarations = new Dictionary<Symbol, Declaration>();

        public DeclarationBuilder(ILFactory ilFactory)
        {
            this.ilFactory = ilFactory;
        }

        public IEnumerable<Declaration> AllDeclarations => declarations.Values;

        public void Build(IEnumerable<IEntityDeclarationSyntax> entityDeclarations, ISymbolTree symbolTree)
        {
            foreach (var memberDeclaration in entityDeclarations)
                Build(memberDeclaration, symbolTree);
        }

        private FixedList<Declaration> BuildList(
            IEnumerable<IMemberDeclarationSyntax> memberDeclarations,
            ISymbolTree symbolTree)
        {
            return memberDeclarations.Select(e => Build(e, symbolTree)).ToFixedList();
        }

        private Declaration Build(IEntityDeclarationSyntax entityDeclaration, ISymbolTree symbolTree)
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
                    declaration = new FunctionDeclaration(function.IsExternalFunction, false, function.Symbol.Result, BuildParameters(function.Parameters), il);
                    break;
                }
                case IAssociatedFunctionDeclarationSyntax associatedFunction:
                {
                    var il = ilFactory.CreateGraph(associatedFunction);
                    declaration = new FunctionDeclaration(false, true, associatedFunction.Symbol.Result, BuildParameters(associatedFunction.Parameters), il);
                    break;
                }
                case IConcreteMethodDeclarationSyntax method:
                {
                    var il = ilFactory.CreateGraph(method);
                    declaration = new MethodDeclaration(method.Symbol.Result, BuildParameter(method.SelfParameter), BuildParameters(method.Parameters), il);
                    break;
                }
                case IAbstractMethodDeclarationSyntax method:
                {
                    declaration = new MethodDeclaration(method.Symbol.Result, BuildParameter(method.SelfParameter), BuildParameters(method.Parameters), null);
                    break;
                }
                case IConstructorDeclarationSyntax constructor:
                {
                    var il = ilFactory.CreateGraph(constructor);
                    var parameters = BuildConstructorParameters(constructor);
                    var fieldInitializations = BuildFieldInitializations(constructor);
                    declaration = new ConstructorDeclaration(constructor.Symbol.Result, parameters, fieldInitializations, il);
                    break;
                }
                case IFieldDeclarationSyntax fieldDeclaration:
                    declaration = new FieldDeclaration(fieldDeclaration.Symbol.Result);
                    break;
                case IClassDeclarationSyntax classDeclaration:
                    declaration = new ClassDeclaration(classDeclaration.Symbol.Result,
                        BuildClassMembers(classDeclaration, symbolTree));
                    break;
            }
            declarations.Add(entityDeclaration.Symbol.Result, declaration);
            return declaration;
        }

        private FixedList<Declaration> BuildClassMembers(
            IClassDeclarationSyntax classDeclaration,
            ISymbolTree symbolTree)
        {
            var members = BuildList(classDeclaration.Members, symbolTree);
            var defaultConstructor = BuildDefaultConstructor(classDeclaration, symbolTree);
            if (!(defaultConstructor is null))
                members = members.Append(defaultConstructor).ToFixedList();
            return members.ToFixedList();
        }

        private Declaration? BuildDefaultConstructor(
            IClassDeclarationSyntax classDeclaration,
            ISymbolTree symbolTree)
        {
            var constructorSymbol = classDeclaration.DefaultConstructorSymbol;
            if (constructorSymbol is null) return null;

            if (declarations.TryGetValue(constructorSymbol, out var declaration))
                return declaration;

            var selfParameterSymbol = symbolTree.Children(constructorSymbol).OfType<SelfParameterSymbol>().Single();
            var selfParameter = new SelfParameter(selfParameterSymbol);
            var parameters = selfParameter.Yield().ToFixedList<Parameter>();

            var graph = new ControlFlowGraphBuilder(classDeclaration.File);
            graph.AddSelfParameter(selfParameterSymbol);
            var block = graph.NewBlock();
            block.End(new ReturnVoidInstruction(classDeclaration.NameSpan, Scope.Outer));

            //var il = new ControlFlowGraphBuilder(classDeclaration.File);
            //il.AddSelfParameter(selfType);
            //var block = il.NewBlock();
            //block.End(classDeclaration.NameSpan, Scope.Outer);

            var defaultConstructor = new ConstructorDeclaration(// TODO how to get a name
                constructorSymbol,
                                            parameters, FixedList<FieldInitialization>.Empty, graph.Build());

            //defaultConstructor.ControlFlowOld.InsertedDeletes = new InsertedDeletes();
            declarations.Add(constructorSymbol, defaultConstructor);
            return defaultConstructor;
        }

        private static FixedList<Parameter> BuildParameters(IEnumerable<IParameterSyntax> parameters)
        {
            return parameters.Select(BuildParameter).ToFixedList();
        }

        private static FixedList<Parameter> BuildConstructorParameters(IConstructorDeclarationSyntax constructorDeclaration)
        {
            var selfParameterSymbol = constructorDeclaration.ImplicitSelfParameter.Symbol.Result;
            var selfParameter = new SelfParameter(selfParameterSymbol);
            return selfParameter.Yield().Concat(constructorDeclaration.Parameters.Select(BuildParameter)).ToFixedList();
        }

        private static Parameter BuildParameter(IParameterSyntax parameter)
        {
            return parameter switch
            {
                INamedParameterSyntax namedParameter => new NamedParameter(namedParameter.Symbol.Result),
                ISelfParameterSyntax selfParameter => new SelfParameter(selfParameter.Symbol.Result),
                IFieldParameterSyntax fieldParameter =>
                     new FieldParameter(fieldParameter.ReferencedSymbol.Result ?? throw new InvalidOperationException()),
                _ => throw ExhaustiveMatch.Failed(parameter)
            };
        }

        private static FixedList<FieldInitialization> BuildFieldInitializations(
            IConstructorDeclarationSyntax constructorDeclaration)
        {
            return constructorDeclaration.Parameters.OfType<IFieldParameterSyntax>()
                                         .Select(BuildFieldInitialization)
                                         .ToFixedList();
        }

        private static FieldInitialization BuildFieldInitialization(IFieldParameterSyntax parameter)
        {
            var field = parameter.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            return new FieldInitialization(field);
        }
    }
}
