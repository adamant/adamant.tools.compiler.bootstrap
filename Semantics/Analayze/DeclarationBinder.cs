using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Parse.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Binders;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analayze
{
    /// Binds declarations of functions and variables to their declared types
    public class DeclarationBinder
    {
        internal void BindDeclarations(PackageSyntax package, Annotations annotations)
        {
            var packageSymbol = annotations.Symbol(package);
            var globalScope = new NameScope(packageSymbol.GlobalNamespace);
            foreach (var tree in packageSymbol.Declaration.SyntaxTrees)
                BindDeclarations(tree.Root, globalScope, annotations);
        }

        private void BindDeclarations(
            SyntaxBranchNode syntax,
            NameScope scope,
            Annotations annotations)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(scope), scope);
            Requires.NotNull(nameof(annotations), annotations);

            Match.On(syntax).With(m => m
                .Is<CompilationUnitSyntax>(cu => BindChildren(cu, scope, annotations))
                .Is<FunctionDeclarationSyntax>(f =>
                {
                    var parameterTypes = BindParameters(f.Parameters, scope, annotations);
                    var returnType = BindType(f.ReturnType, scope, annotations);
                    var functionType = new FunctionType(parameterTypes, returnType);
                    annotations.Add(f, functionType);
                    var symbol = annotations.Symbol(f);
                    var functionScope = new NameScope(scope, symbol);
                    BindChildren(f.Body, functionScope, annotations);
                })
                // TODO varible declarations syntax needs checked
                // TODO for loop syntax needs checked
                // TODO if let syntax needs checked
                .Ignore<StatementSyntax>()
            );
        }

        private void BindChildren(
            SyntaxBranchNode syntax,
            NameScope scope,
            Annotations annotations)
        {
            foreach (var child in syntax.Children)
                if (child is SyntaxBranchNode childNode)
                    BindDeclarations(childNode, scope, annotations);
        }

        private static IEnumerable<DataType> BindParameters(
            ParameterListSyntax parameterList,
            NameScope scope,
            Annotations annotations)
        {
            return parameterList.Parameters
                .Select(parameter => BindParameter(parameter, scope, annotations))
                .ToList();
        }

        private static DataType BindParameter(
            ParameterSyntax parameter,
            NameScope scope,
            Annotations annotations)
        {
            var type = BindType(parameter.Type, scope, annotations);
            annotations.Add(parameter, type);
            return type;
        }

        private static DataType BindType(TypeSyntax typeSyntax, NameScope scope, Annotations annotations)
        {
            return MatchInto<DataType>.On(typeSyntax).With(m => m
                .Is<PrimitiveTypeSyntax>(p =>
                {
                    var type = PrimitiveType.New(p.Keyword.Kind);
                    annotations.Add(typeSyntax, type);
                    return type;
                })
            );
        }
    }
}
