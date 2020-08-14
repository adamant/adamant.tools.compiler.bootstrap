using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree;
using ExhaustiveMatching;
using Package = Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree.Package;

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
                                        .Select(BuildNonMemberDeclaration).ToFixedSet();

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

            return new ClassDeclaration(syn.Span, symbol);
        }

        private static IFunctionDeclaration BuildFunction(IFunctionDeclarationSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            var parameters = syn.Parameters.Select(BuildParameter).ToFixedList();
            return new FunctionDeclaration(syn.Span, symbol, parameters);
        }

        private static INamedParameter BuildParameter(INamedParameterSyntax syn)
        {
            var symbol = syn.Symbol.Result;
            // TODO build default value expression
            return new NamedParameter(syn.Span, symbol, syn.Unused, null);
        }
    }
}
