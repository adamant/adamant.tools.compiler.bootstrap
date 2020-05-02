using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class AbstractMethodDeclarationSyntax : MethodDeclarationSyntax
    {
        public AbstractMethodDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            FixedList<IModiferToken> modifiers,
            Name fullName,
            TextSpan nameSpan,
            ISelfParameterSyntax selfParameter,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnTypeSyntax,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations)
            : base(declaringClass, span, file, modifiers, fullName, nameSpan,
                selfParameter, parameters, returnTypeSyntax, reachabilityAnnotations,
                new SymbolSet(parameters.ImplicitCast<IParameterSyntax>().Prepend(selfParameter)))
        {
        }

        public override string ToString()
        {
            var returnType = ReturnTypeSyntax != null ? " -> " + ReturnTypeSyntax : "";
            return $"fn {FullName}({string.Join(", ", Parameters)}){returnType};";
        }
    }
}
