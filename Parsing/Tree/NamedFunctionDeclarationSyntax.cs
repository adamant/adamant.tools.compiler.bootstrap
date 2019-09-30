using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class NamedFunctionDeclarationSyntax : FunctionLikeSyntax, IFunctionDeclarationSyntax
    {
        public bool IsExternalFunction { get; set; }
        public ExpressionSyntax? LifetimeBounds { get; }
        public TypeSyntax? ReturnTypeSyntax { get; }

        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;

        DataType IFunctionSymbol.ReturnType => ReturnType.Fulfilled();

        public NamedFunctionDeclarationSyntax(
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<IParameterSyntax> parameters, // For now we will not support pure meta functions
            ExpressionSyntax? lifetimeBounds,
            TypeSyntax? returnTypeSyntax,
            FixedList<StatementSyntax>? body)
            : base(span, file, modifiers, fullName, nameSpan, parameters, body)
        {
            LifetimeBounds = lifetimeBounds;
            ReturnTypeSyntax = returnTypeSyntax;
        }

        public override string ToString()
        {
            var returnType = ReturnTypeSyntax != null ? " -> " + ReturnTypeSyntax : "";
            var body = Body != null ? " {{ … }}" : ";";
            return $"fn {FullName}({string.Join(", ", Parameters)}){returnType}{body}";
        }
    }
}