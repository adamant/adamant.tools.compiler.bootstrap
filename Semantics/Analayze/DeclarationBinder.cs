using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analayze
{
    /// Binds declarations of functions and variables to their declared types
    public class DeclarationBinder
    {
        private readonly Annotations annotations;

        internal DeclarationBinder(Annotations annotations)
        {
            this.annotations = annotations;
        }

        internal void BindDeclarations(PackageSyntax package)
        {
            var packageSymbol = annotations.Symbol(package);
            foreach (var tree in packageSymbol.Declaration.SyntaxTrees)
                BindDeclarations(tree.Root);
        }

        private void BindDeclarations(SyntaxBranchNode syntax)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(annotations), annotations);

            Match.On(syntax).With(m => m
                .Is<CompilationUnitSyntax>(BindChildren)
                .Is<FunctionDeclarationSyntax>(f =>
                {
                    var parameterTypes = BindParameters(f.ParameterList);
                    var returnType = BindType(f.ReturnType);
                    var functionType = new FunctionType(parameterTypes, returnType);
                    annotations.Add(f, functionType);
                    BindChildren(f.Body);
                })
                // TODO varible declarations syntax needs checked
                // TODO for loop syntax needs checked
                // TODO if let syntax needs checked
                .Ignore<StatementSyntax>()
            );
        }

        private void BindChildren(
            SyntaxBranchNode syntax)
        {
            foreach (var child in syntax.Children)
                if (child is SyntaxBranchNode childNode)
                    BindDeclarations(childNode);
        }

        private IEnumerable<DataType> BindParameters(ParameterListSyntax parameterList)
        {
            return parameterList.Parameters
                .Select(BindParameter)
                .ToList();
        }

        private DataType BindParameter(ParameterSyntax parameter)
        {
            var type = BindType(parameter.Type);
            annotations.Add(parameter, type);
            return type;
        }

        private DataType BindType(TypeSyntax typeSyntax)
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
