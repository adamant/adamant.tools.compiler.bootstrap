using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class MethodDeclarationSyntax : CallableDeclarationSyntax, IMethodDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringClass { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;
        public ISelfParameterSyntax SelfParameter { get; }
        public new FixedList<INamedParameterSyntax> Parameters { get; }
        public ITypeSyntax? ReturnTypeSyntax { get; }
        DataType IFunctionSymbol.ReturnType => ReturnType.Fulfilled();

        protected MethodDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            ISelfParameterSyntax selfParameter,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnTypeSyntax,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            SymbolSet childSymbols)
            : base(span, file, modifiers, fullName, nameSpan,
                parameters.ImplicitCast<IParameterSyntax>().Prepend(selfParameter).ToFixedList(),
                reachabilityAnnotations, childSymbols)
        {
            DeclaringClass = declaringClass;
            SelfParameter = selfParameter;
            Parameters = parameters;
            ReturnTypeSyntax = returnTypeSyntax;
        }
    }
}
