using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Binders;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analayze
{
    /// Binds declarations of functions and variables to their declared types
    public class DeclarationBinder
    {
        public void BindDeclarations(PackageSyntaxSymbol packageSymbol, SyntaxAnnotation<DataType> typeAnnotations)
        {
            var globalScope = new NameScope(packageSymbol.GlobalNamespace);
            foreach (var tree in packageSymbol.Declaration.SyntaxTrees)
                BindDeclarations(packageSymbol.GlobalNamespace, tree.Root, globalScope, typeAnnotations);
        }

        private void BindDeclarations(
            ISyntaxSymbol parentSymbol,
            SyntaxBranchNode syntax,
            NameScope scope,
            SyntaxAnnotation<DataType> typeAnnotations)
        {
            Requires.NotNull(nameof(parentSymbol), parentSymbol);
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(scope), scope);
            Requires.NotNull(nameof(typeAnnotations), typeAnnotations);

            Match.On(syntax).With(m => m
                .Is<CompilationUnitSyntax>(cu => BindChildren(parentSymbol, cu, scope, typeAnnotations))
                .Is<FunctionDeclarationSyntax>(f =>
                {
                    var parameterSymbols = BindParameters(parentSymbol, f.Parameters, scope, typeAnnotations);
                    var returnType = BindType(f.ReturnType, scope, typeAnnotations);
                    var functionType = new FunctionType(parameterSymbols.Select(p => p.Type), returnType);
                    typeAnnotations.Add(f, functionType);
                    var name = f.Name.Text;
                    var symbol = new DeclarationSyntaxSymbol(name, functionType, f);
                    symbol.AddChildren(parameterSymbols);
                    parentSymbol.AddChild(symbol);
                    var functionScope = new NameScope(scope, symbol);
                    BindChildren(symbol, f.Body, functionScope, typeAnnotations);
                })
                // TODO varible declarations syntax needs checked
                // TODO for loop syntax needs checked
                // TODO if let syntax needs checked
                .Ignore<StatementSyntax>()
            );
        }

        private void BindChildren(
            ISyntaxSymbol parentSymbol,
            SyntaxBranchNode syntax,
            NameScope scope,
            SyntaxAnnotation<DataType> typeAnnotations)
        {
            foreach (var child in syntax.Children)
                if (child is SyntaxBranchNode childNode)
                    BindDeclarations(parentSymbol, childNode, scope, typeAnnotations);
        }

        private IEnumerable<DeclarationSyntaxSymbol> BindParameters(
            ISyntaxSymbol parentSymbol,
            ParameterListSyntax parameterList,
            NameScope scope,
            SyntaxAnnotation<DataType> typeAnnotations)
        {
            return parameterList.Parameters
                .Select(parameter => BindParameter(parentSymbol, parameter, scope, typeAnnotations))
                .ToList();
        }

        private DeclarationSyntaxSymbol BindParameter(
            ISyntaxSymbol parentSymbol,
            ParameterSyntax parameter,
            NameScope scope,
            SyntaxAnnotation<DataType> typeAnnotations)
        {
            var identifier = parameter.Identifier;
            var type = BindType(parameter.Type, scope, typeAnnotations);
            var symbol = new DeclarationSyntaxSymbol(identifier.Value, type, parameter);
            typeAnnotations.Add(parameter, type);
            return symbol;
        }

        private DataType BindType(TypeSyntax typeSyntax, NameScope scope, SyntaxAnnotation<DataType> typeAnnotations)
        {
            return MatchInto<DataType>.On(typeSyntax).With(m => m
                .Is<PrimitiveTypeSyntax>(p =>
                {
                    var type = PrimitiveType.New(p.Keyword.Kind);
                    typeAnnotations.Add(typeSyntax, type);
                    return type;
                })
            );
        }
    }
}
