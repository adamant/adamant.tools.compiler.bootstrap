using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FunctionDeclarationSyntax : CallableDeclarationSyntax, IFunctionDeclarationSyntax
    {
        public bool IsExternalFunction { get; set; }
        public ITypeSyntax? ReturnTypeSyntax { get; }
        public new FixedList<INamedParameterSyntax> Parameters { get; }
        DataType IFunctionSymbol.ReturnType => ReturnType.Fulfilled();
        public IBodySyntax Body { get; }

        public FunctionDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnTypeSyntax,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            IBodySyntax body)
            : base(span, file, modifiers, fullName, nameSpan, parameters,
                reachabilityAnnotations, GetChildSymbols(null, parameters, body))
        {
            Parameters = parameters;
            ReturnTypeSyntax = returnTypeSyntax;
            Body = body;
        }

        public override string ToString()
        {
            var returnType = ReturnTypeSyntax != null ? " -> " + ReturnTypeSyntax : "";
            return $"fn {FullName}({string.Join(", ", Parameters)}){returnType} {Body}";
        }

    }
}
