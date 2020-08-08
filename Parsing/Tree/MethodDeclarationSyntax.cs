using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class MethodDeclarationSyntax : CallableDeclarationSyntax, IMethodDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringClass { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public new SimpleName Name => FullName.UnqualifiedName;
        public ISelfParameterSyntax SelfParameter { get; }
        public IBindingMetadata SelfParameterMetadata => SelfParameter;
        public new FixedList<INamedParameterSyntax> Parameters { get; }
        public ITypeSyntax? ReturnType { get; }
        DataType IFunctionMetadata.ReturnDataType => ReturnDataType.Fulfilled();

        protected MethodDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            MaybeQualifiedName fullName,
            TextSpan nameSpan,
            Name name,
            ISelfParameterSyntax selfParameter,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnType,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            MetadataSet childMetadata)
            : base(span, file, accessModifier, fullName, nameSpan, name,
                parameters, reachabilityAnnotations, childMetadata)
        {
            DeclaringClass = declaringClass;
            SelfParameter = selfParameter;
            Parameters = parameters;
            ReturnType = returnType;
        }
    }
}
