using System.Collections.Generic;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class MethodDeclarationSyntax : CallableDeclarationSyntax, IMethodDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringType { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;
        IEnumerable<IBindingSymbol> IFunctionSymbol.Parameters => Parameters;
        public IExpressionSyntax? LifetimeBounds { get; }
        public ITypeSyntax? ReturnTypeSyntax { get; }
        DataType IFunctionSymbol.ReturnType => ReturnType.Fulfilled();

        public MethodDeclarationSyntax(
            IClassDeclarationSyntax declaringType,
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            FixedList<IParameterSyntax> parameters, // For now we will not support pure meta functions
            IExpressionSyntax? lifetimeBounds,
            ITypeSyntax? returnTypeSyntax,
            FixedList<IStatementSyntax>? body)
            : base(span, file, modifiers, fullName, nameSpan, parameters, body)
        {
            DeclaringType = declaringType;
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
