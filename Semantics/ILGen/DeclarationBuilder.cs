using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
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
        private readonly Dictionary<Symbol, DeclarationIL> declarationsIL = new Dictionary<Symbol, DeclarationIL>();

        public DeclarationBuilder(ILFactory ilFactory)
        {
            this.ilFactory = ilFactory;
        }

        public IEnumerable<DeclarationIL> AllDeclarations => declarationsIL.Values;

        public void Build(IEnumerable<IDeclaration> declarations, ISymbolTree symbolTree)
        {
            foreach (var declaration in declarations)
                Build(declaration, symbolTree);
        }

        private FixedList<DeclarationIL> BuildList(
            IEnumerable<IMemberDeclaration> memberDeclarations,
            ISymbolTree symbolTree)
        {
            return memberDeclarations.Select(e => Build(e, symbolTree)).ToFixedList();
        }

        private DeclarationIL Build(IDeclaration declaration, ISymbolTree symbolTree)
        {
            if (declarationsIL.TryGetValue(declaration.Symbol, out var declarationIL))
                return declarationIL;

            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case IFunctionDeclaration function:
                {
                    var il = ilFactory.CreateGraph(function);
                    declarationIL = new FunctionIL(false, false, function.Symbol, BuildParameters(function.Parameters), il);
                    break;
                }
                case IAssociatedFunctionDeclaration associatedFunction:
                {
                    var il = ilFactory.CreateGraph(associatedFunction);
                    declarationIL = new FunctionIL(false, true, associatedFunction.Symbol, BuildParameters(associatedFunction.Parameters), il);
                    break;
                }
                case IConcreteMethodDeclaration method:
                {
                    var il = ilFactory.CreateGraph(method);
                    declarationIL = new MethodDeclarationIL(method.Symbol, BuildParameter(method.SelfParameter), BuildParameters(method.Parameters), il);
                    break;
                }
                case IAbstractMethodDeclaration method:
                {
                    declarationIL = new MethodDeclarationIL(method.Symbol, BuildParameter(method.SelfParameter), BuildParameters(method.Parameters), null);
                    break;
                }
                case IConstructorDeclaration constructor:
                {
                    var il = ilFactory.CreateGraph(constructor);
                    var parameters = BuildConstructorParameters(constructor);
                    var fieldInitializations = BuildFieldInitializations(constructor);
                    declarationIL = new ConstructorIL(constructor.Symbol, parameters, fieldInitializations, il);
                    break;
                }
                case IFieldDeclaration fieldDeclaration:
                    declarationIL = new FieldIL(fieldDeclaration.Symbol);
                    break;
                case IClassDeclaration classDeclaration:
                    declarationIL = new ClassIL(classDeclaration.Symbol,
                        BuildClassMembers(classDeclaration, symbolTree));
                    break;
            }
            declarationsIL.Add(declaration.Symbol, declarationIL);
            return declarationIL;
        }

        private FixedList<DeclarationIL> BuildClassMembers(
            IClassDeclaration classDeclaration,
            ISymbolTree symbolTree)
        {
            var members = BuildList(classDeclaration.Members, symbolTree);
            var defaultConstructor = BuildDefaultConstructor(classDeclaration, symbolTree);
            if (!(defaultConstructor is null))
                members = members.Append(defaultConstructor).ToFixedList();
            return members.ToFixedList();
        }

        private DeclarationIL? BuildDefaultConstructor(
            IClassDeclaration classDeclaration,
            ISymbolTree symbolTree)
        {
            var constructorSymbol = classDeclaration.DefaultConstructorSymbol;
            if (constructorSymbol is null) return null;

            if (declarationsIL.TryGetValue(constructorSymbol, out var declaration))
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
            declarationsIL.Add(constructorSymbol, defaultConstructor);
            return defaultConstructor;
        }

        private static FixedList<ParameterIL> BuildParameters(IEnumerable<IParameter> parameters)
        {
            return parameters.Select(BuildParameter).ToFixedList();
        }

        private static FixedList<ParameterIL> BuildConstructorParameters(IConstructorDeclaration constructorDeclaration)
        {
            var selfParameterSymbol = constructorDeclaration.ImplicitSelfParameter.Symbol;
            var selfParameter = new SelfParameterIL(selfParameterSymbol);
            return selfParameter.Yield().Concat(constructorDeclaration.Parameters.Select(BuildParameter)).ToFixedList();
        }

        private static ParameterIL BuildParameter(IParameter parameter)
        {
            return parameter switch
            {
                INamedParameter namedParameter => new NamedParameterIL(namedParameter.Symbol),
                ISelfParameter selfParameter => new SelfParameterIL(selfParameter.Symbol),
                IFieldParameter fieldParameter => new FieldParameterIL(fieldParameter.ReferencedSymbol),
                _ => throw ExhaustiveMatch.Failed(parameter)
            };
        }

        private static FixedList<FieldInitializationIL> BuildFieldInitializations(
            IConstructorDeclaration constructorDeclaration)
        {
            return constructorDeclaration.Parameters.OfType<IFieldParameter>()
                                         .Select(BuildFieldInitialization)
                                         .ToFixedList();
        }

        private static FieldInitializationIL BuildFieldInitialization(IFieldParameter parameter)
        {
            return new FieldInitializationIL(parameter.ReferencedSymbol);
        }
    }
}
