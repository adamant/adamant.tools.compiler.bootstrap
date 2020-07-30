using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FunctionDeclarationSyntax : CallableDeclarationSyntax, IFunctionDeclarationSyntax
    {
        public bool IsExternalFunction { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
        public ITypeSyntax? ReturnTypeSyntax { [DebuggerStepThrough] get; }
        public new FixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
        DataType IFunctionSymbol.ReturnType => ReturnType.Fulfilled();
        public IBodySyntax Body { [DebuggerStepThrough] get; }

        public FunctionDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            Name fullName,
            TextSpan nameSpan,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnTypeSyntax,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            IBodySyntax body)
            : base(span, file, accessModifier, fullName, nameSpan, parameters,
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
