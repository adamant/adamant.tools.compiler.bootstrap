using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class GenericParameterSyntax : Syntax, ISymbol
    {
        // Generic parameters are not mutable bindings
        bool ISymbol.MutableBinding => false;
        public bool IsLifetime { get; }
        public bool IsParams { get; }
        public Name FullName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName Name => FullName.UnqualifiedName;

        public ExpressionSyntax TypeExpression { get; }
        public TypePromise Type { get; } = new TypePromise();
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        DataType ISymbol.Type => Type.Fulfilled();
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        SymbolSet ISymbol.ChildSymbols => SymbolSet.Empty;

        public GenericParameterSyntax(
            bool isLifetime,
            bool isParams,
            Name fullName,
            ExpressionSyntax typeExpression)
        {
            FullName = fullName;
            TypeExpression = typeExpression;
            IsLifetime = isLifetime;
            IsParams = isParams;
        }

        public override string ToString()
        {
            if (IsLifetime)
                return "$" + Name;
            var paramsKeyword = IsParams ? "params " : "";
            if (TypeExpression != null)
                return $"{paramsKeyword}{Name} : {TypeExpression}";
            return $"{paramsKeyword}{Name}";
        }
    }
}
