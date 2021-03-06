using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    // TODO No error is reported if IConcreteMethodDeclarationSyntax is missing
    internal class ConcreteMethodDeclarationSyntax : MethodDeclarationSyntax, IConcreteMethodDeclarationSyntax
    {
        public virtual IBodySyntax Body { get; }

        public ConcreteMethodDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            TextSpan nameSpan,
            Name name,
            ISelfParameterSyntax selfParameter,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnType,
            IReachabilityAnnotationsSyntax reachabilityAnnotations,
            IBodySyntax body)
            : base(declaringClass, span, file, accessModifier, nameSpan, name, selfParameter,
                parameters, returnType, reachabilityAnnotations)
        {
            Body = body;
        }

        public override string ToString()
        {
            var returnType = ReturnType != null ? " -> " + ReturnType : "";
            return $"fn {Name}({string.Join(", ", Parameters.Prepend<IParameterSyntax>(SelfParameter))}){returnType} {Body}";
        }
    }
}
