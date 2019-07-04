using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ForeachExpressionSyntax : ExpressionSyntax, ISymbol
    {
        public bool MutableBinding { get; }
        Name ISymbol.FullName => FullVariableName;
        private Name FullVariableName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName VariableName => FullVariableName.UnqualifiedName;
        public ExpressionSyntax TypeExpression { get; }
        public ExpressionSyntax InExpression { get; }
        public BlockSyntax Block { get; }

        DataType ISymbol.Type => VariableType;
        private DataType variableType;
        public DataType VariableType
        {
            get => variableType;
            set
            {
                if (variableType != null) throw new InvalidOperationException("Can't set type repeatedly");
                variableType = value ?? throw new ArgumentNullException(nameof(Type), "Can't set type to null");
            }
        }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        SymbolSet ISymbol.ChildSymbols => SymbolSet.Empty;

        public ForeachExpressionSyntax(
            TextSpan span,
            bool mutableBinding,
            Name fullVariableName,
            ExpressionSyntax typeExpression,
            ExpressionSyntax inExpression,
            BlockSyntax block)
            : base(span)
        {
            MutableBinding = mutableBinding;
            FullVariableName = fullVariableName;
            InExpression = inExpression;
            Block = block;
            TypeExpression = typeExpression;
        }

        public override string ToString()
        {
            var binding = MutableBinding ? "var " : "";
            return $"foreach {binding}{VariableName}: {TypeExpression} in {InExpression} {Block}";
        }
    }
}
