using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class AssociatedFunctionDeclarationSyntax : CallableDeclarationSyntax, IAssociatedFunctionDeclarationSyntax
    {
        public IClassDeclarationSyntax DeclaringClass { get; }
        public new Name Name { get; }
        public Promise<FunctionSymbol> Symbol { get; } = new Promise<FunctionSymbol>();
        protected override IPromise<Symbol> SymbolPromise => Symbol;

        public new FixedList<INamedParameterSyntax> Parameters { get; }
        public ITypeSyntax? ReturnType { get; }
        DataType IFunctionMetadata.ReturnDataType => ReturnDataType.Result;
        public IBodySyntax Body { get; }

        public AssociatedFunctionDeclarationSyntax(
            IClassDeclarationSyntax declaringClass,
            TextSpan span,
            CodeFile file,
            IAccessModifierToken? accessModifier,
            MaybeQualifiedName fullName,
            TextSpan nameSpan,
            Name name,
            FixedList<INamedParameterSyntax> parameters,
            ITypeSyntax? returnTypeSyntax,
            FixedList<IReachabilityAnnotationSyntax> reachabilityAnnotations,
            IBodySyntax body)
            : base(span, file, accessModifier, fullName, nameSpan, name, parameters,
                reachabilityAnnotations, GetChildMetadata(null, parameters, body))
        {
            DeclaringClass = declaringClass;
            Name = name;
            Parameters = parameters;
            ReturnType = returnTypeSyntax;
            Body = body;
        }

        public override string ToString()
        {
            var returnType = ReturnType != null ? " -> " + ReturnType : "";
            return $"fn {FullName}({string.Join(", ", Parameters)}){returnType} {Body}";
        }
    }
}
