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
    internal class FunctionDeclarationSyntax : CallableDeclarationSyntax, IFunctionDeclarationSyntax
    {
        public NamespaceName ContainingNamespaceName { get; }
        public bool IsExternalFunction { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }
        public ITypeSyntax? ReturnType { [DebuggerStepThrough] get; }
        public new FixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
        DataType IFunctionMetadata.ReturnDataType => ReturnDataType.Fulfilled();
        public IBodySyntax Body { [DebuggerStepThrough] get; }

        public FunctionDeclarationSyntax(
            NamespaceName containingNamespaceName,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            MaybeQualifiedName fullName,
            TextSpan nameSpan,
            Name name,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnType,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            IBodySyntax body)
            : base(span, file, accessModifier, fullName, nameSpan, name, parameters,
                reachabilityAnnotations, GetChildMetadata(null, parameters, body))
        {
            ContainingNamespaceName = containingNamespaceName;
            Parameters = parameters;
            ReturnType = returnType;
            Body = body;
        }

        public override string ToString()
        {
            var returnType = ReturnType != null ? " -> " + ReturnType : "";
            return $"fn {FullName}({string.Join(", ", Parameters)}){returnType} {Body}";
        }
    }
}
