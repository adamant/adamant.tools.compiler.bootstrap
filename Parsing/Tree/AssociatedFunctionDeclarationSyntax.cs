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
    internal class AssociatedFunctionDeclarationSyntax : CallableDeclarationSyntax, IAssociatedFunctionDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringClass { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public new FixedList<INamedParameterSyntax> Parameters { get; }
        public ITypeSyntax? ReturnTypeSyntax { get; }
        DataType IFunctionSymbol.ReturnType => ReturnType.Fulfilled();
        public IBodySyntax Body { get; }

        public AssociatedFunctionDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
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
            DeclaringClass = declaringClass;
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
