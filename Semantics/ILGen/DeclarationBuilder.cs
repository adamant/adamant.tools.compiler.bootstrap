using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen
{
    public class DeclarationBuilder
    {
        private readonly ILFactory ilFactory;
        private readonly Dictionary<IMetadata, Declaration> declarations = new Dictionary<IMetadata, Declaration>();

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
                        function.ReturnDataType.Known(), il);
                    break;
                }
                case IAssociatedFunctionDeclarationSyntax associatedFunction:
                {
                    var il = ilFactory.CreateGraph(associatedFunction);
                    declaration = new FunctionDeclaration(false, true,
                        associatedFunction.FullName, BuildParameters(associatedFunction.Parameters),
                        associatedFunction.ReturnDataType.Known(), il);
                    break;
                }
                case IConcreteMethodDeclarationSyntax method:
                {
                    var il = ilFactory.CreateGraph(method);
                    declaration = new MethodDeclaration(method.FullName, BuildParameter(method.SelfParameter), BuildParameters(method.Parameters),
                        method.ReturnDataType.Known(), il);
                    break;
                }
                case IAbstractMethodDeclarationSyntax method:
                {
                    declaration = new MethodDeclaration(method.FullName, BuildParameter(method.SelfParameter), BuildParameters(method.Parameters),
                        method.ReturnDataType.Known(), null);
                    break;
                }
                case IConstructorDeclarationSyntax constructor:
                {
                    var il = ilFactory.CreateGraph(constructor);
                    var parameters = BuildConstructorParameters(constructor);
                    var fieldInitializations = BuildFieldInitializations(constructor);
                    declaration = new ConstructorDeclaration(constructor.FullName,
                       parameters, constructor.ImplicitSelfParameter.Symbol.Result.DataType.Known(), fieldInitializations, il);
                    break;
                }
                case IFieldDeclarationSyntax fieldDeclaration:
                    declaration = new FieldDeclaration(fieldDeclaration.IsMutableBinding, fieldDeclaration.FullName, fieldDeclaration.DataType.Known());
                    break;
                case IClassDeclarationSyntax classDeclaration:
                    declaration = new ClassDeclaration(classDeclaration.FullName,
                        classDeclaration.DeclaresDataType.Known(),
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
            var constructor = classDeclaration.ChildMetadata.SafeCast<IMetadata>()
                            .OfType<DefaultConstructor>().Single();
            if (declarations.TryGetValue(constructor, out var declaration))
                return declaration;

            var selfType = classDeclaration.Symbol.Result.DeclaresDataType;
            var selfParameter = new Parameter(false, SpecialNames.Self, selfType);
            var parameters = selfParameter.Yield().ToFixedList();

            var graph = new ControlFlowGraphBuilder(classDeclaration.File);
            graph.AddSelfParameter(constructor.ImplicitSelfParameterSymbol);
            var block = graph.NewBlock();
            block.End(new ReturnVoidInstruction(classDeclaration.NameSpan, Scope.Outer));

            //var il = new ControlFlowGraphBuilder(classDeclaration.File);
            //il.AddSelfParameter(selfType);
            //var block = il.NewBlock();
            //block.End(classDeclaration.NameSpan, Scope.Outer);

            var defaultConstructor = new ConstructorDeclaration(
                                            constructor.FullName,
                                            parameters,
                                            selfType,
                                            FixedList<FieldInitialization>.Empty,
                                            graph.Build());

            //defaultConstructor.ControlFlowOld.InsertedDeletes = new InsertedDeletes();
            declarations.Add(constructor, defaultConstructor);
            return defaultConstructor;
        }

        private static FixedList<Parameter> BuildParameters(IEnumerable<IParameterSyntax> parameters)
        {
            return parameters.Select(BuildParameter).ToFixedList();
        }

        private static FixedList<Parameter> BuildConstructorParameters(IConstructorDeclarationSyntax constructorDeclaration)
        {
            var selfType = constructorDeclaration.ImplicitSelfParameter.Symbol.Result.DataType.Known();
            var selfParameter = new Parameter(false, SpecialNames.Self, selfType);
            return selfParameter.Yield().Concat(constructorDeclaration.Parameters.Select(BuildParameter)).ToFixedList();
        }

        private static Parameter BuildParameter(IParameterSyntax parameter)
        {
            return parameter switch
            {
                INamedParameterSyntax namedParameter =>
                    new Parameter(namedParameter.IsMutableBinding, namedParameter.FullName.UnqualifiedName, namedParameter.DataType.Known()),
                ISelfParameterSyntax selfParameter =>
                    new Parameter(selfParameter.IsMutableBinding, selfParameter.FullName.UnqualifiedName, selfParameter.DataType.Known()),
                IFieldParameterSyntax fieldParameter =>
                    new Parameter(fieldParameter.IsMutableBinding, fieldParameter.FullName.UnqualifiedName, fieldParameter.DataType.Known()),
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
            return new FieldInitialization(parameter.FullName.UnqualifiedName, parameter.Name);
        }
    }
}
