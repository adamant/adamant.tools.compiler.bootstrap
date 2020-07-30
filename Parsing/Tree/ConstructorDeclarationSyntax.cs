using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ConstructorDeclarationSyntax : CallableDeclarationSyntax, IConstructorDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringClass { get; }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public ISelfParameterSyntax ImplicitSelfParameter { get; }
        public new FixedList<IConstructorParameterSyntax> Parameters { get; }

        public virtual IBodySyntax Body { get; }

        public ConstructorDeclarationSyntax(
            IClassDeclarationSyntax declaringType,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            Name fullName,
            TextSpan nameSpan,
            ISelfParameterSyntax implicitSelfParameter,
            FixedList<IConstructorParameterSyntax> parameters,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            IBodySyntax body)
            : base(span, file, accessModifier, fullName, nameSpan, parameters, reachabilityAnnotations,
                GetChildSymbols(implicitSelfParameter, parameters, body))
        {
            DeclaringClass = declaringType;
            ImplicitSelfParameter = implicitSelfParameter;
            Parameters = parameters;
            Body = body;
        }

        public override string ToString()
        {
            return $"{FullName}({string.Join(", ", Parameters)})";
        }
    }
}
