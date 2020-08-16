using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST
{
    // ReSharper disable once UnusedMember.Global
    [SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes",
        Justification = "In Progress")]
    internal class ASTBuilder
    {
        // ReSharper disable once UnusedMember.Global
        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
        public Package BuildPackage(FixedList<IEntityDeclarationSyntax> entities)
        {
            var nonMemberDeclarations = entities
                                        .OfType<INonMemberEntityDeclarationSyntax>()
                                        .Select(BuildNonMemberDeclaration).ToFixedList();

            return new Package(nonMemberDeclarations);
        }

        private static INonMemberDeclaration BuildNonMemberDeclaration(INonMemberEntityDeclarationSyntax entity)
        {
            return entity switch
            {
                IClassDeclarationSyntax syn => BuildClass(syn),
                IFunctionDeclarationSyntax syn => BuildFunction(syn),
                _ => throw ExhaustiveMatch.Failed(entity)
            };
        }

        private static IClassDeclaration BuildClass(IClassDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var members = syn.Members.Select(BuildMember).ToFixedList();
            return new ClassDeclaration(syn.Span, symbol, members);
        }

        private static IMemberDeclaration BuildMember(IMemberDeclarationSyntax member)
        {
            return member switch
            {
                IAssociatedFunctionDeclarationSyntax syn => BuildAssociatedFunction(syn),
                IAbstractMethodDeclarationSyntax syn => BuildAbstractMethod(syn),
                IConcreteMethodDeclarationSyntax syn => BuildConcreteMethod(syn),
                IConstructorDeclarationSyntax syn => BuildConstructor(syn),
                IFieldDeclarationSyntax syn => BuildField(syn),
                _ => throw ExhaustiveMatch.Failed(member)
            };
        }

        private static IAssociatedFunctionDeclaration BuildAssociatedFunction(IAssociatedFunctionDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            // TODO build function body
            IBody body = null!;
            return new AssociatedFunctionDeclaration(syn.Span, symbol, parameters, body);
        }

        private static IAbstractMethodDeclaration BuildAbstractMethod(IAbstractMethodDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var selfParameter = BuildParameter(syn.SelfParameter);
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            return new AbstractMethodDeclaration(syn.Span, symbol, selfParameter, parameters);
        }

        private static IConcreteMethodDeclaration BuildConcreteMethod(IConcreteMethodDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var selfParameter = BuildParameter(syn.SelfParameter);
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            // TODO build function body
            IBody body = null!;
            return new ConcreteMethodDeclaration(syn.Span, symbol, selfParameter, parameters, body);
        }

        private static IConstructorDeclaration BuildConstructor(IConstructorDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var selfParameter = BuildParameter(syn.ImplicitSelfParameter);
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            // TODO build function body
            IBody body = null!;
            return new ConstructorDeclaration(syn.Span, symbol, selfParameter, parameters, body);
        }

        private static IFieldDeclaration BuildField(IFieldDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            return new FieldDeclaration(syn.Span, symbol);
        }

        private static IFunctionDeclaration BuildFunction(IFunctionDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            // TODO build function body
            IBody body = null!;
            return new FunctionDeclaration(syn.Span, symbol, parameters, body);
        }

        private static IConstructorParameter BuildParameter(IConstructorParameterSyntax parameter)
        {
            return parameter switch
            {
                INamedParameterSyntax syn => BuildParameter(syn),
                IFieldParameterSyntax syn => BuildParameter(syn),
                _ => throw ExhaustiveMatch.Failed(parameter),
            };
        }

        private static INamedParameter BuildParameter(INamedParameterSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            // TODO build default value expression
            IExpression defaultValue = null!;
            return new NamedParameter(syn.Span, symbol, syn.Unused, defaultValue);
        }

        private static ISelfParameter BuildParameter(ISelfParameterSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var unused = syn.Unused;
            return new SelfParameter(syn.Span, symbol, unused);
        }

        private static IFieldParameter BuildParameter(IFieldParameterSyntax syn)
        {
            var referencedSymbol = syn.ReferencedSymbol.Result ?? throw new InvalidOperationException();
            // TODO build default value expression
            IExpression defaultValue = null!;
            return new FieldParameter(syn.Span, referencedSymbol, defaultValue);
        }
    }
}
