using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class GenericParameterSyntax : Syntax, ISymbol
    {
        public bool IsLifetime { get; }
        public bool IsParams { get; }
        [NotNull] public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public SimpleName Name => FullName.UnqualifiedName;

        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] DataType ISymbol.Type => Type.Fulfilled();
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] SymbolSet ISymbol.ChildSymbols => SymbolSet.Empty;

        public GenericParameterSyntax(
            bool isLifetime,
            bool isParams,
            [NotNull] Name fullName,
            [CanBeNull] ExpressionSyntax typeExpression)
        {
            FullName = fullName;
            TypeExpression = typeExpression;
            IsLifetime = isLifetime;
            IsParams = isParams;
        }

        public override string ToString()
        {
            if (IsLifetime) return "$" + Name;
            var paramsKeyword = IsParams ? "params " : "";
            if (TypeExpression != null) return $"{paramsKeyword}{Name} : {TypeExpression}";
            return $"{paramsKeyword}{Name}";
        }
    }
}
