using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ForeachExpressionSyntax : ExpressionSyntax, IBindingSymbol
    {
        public bool IsMutableBinding { get; }
        Name ISymbol.FullName => FullVariableName;
        private Name FullVariableName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName VariableName => FullVariableName.UnqualifiedName;
        public ITypeSyntax? TypeSyntax { get; }
        public ExpressionSyntax InExpression;
        public BlockSyntax Block { get; }

        DataType IBindingSymbol.Type => VariableType ?? throw new InvalidOperationException();

        private DataType? variableType;
        public DataType? VariableType
        {
            get => variableType;
            set
            {
                if (variableType != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                variableType = value ?? throw new ArgumentNullException(nameof(Type), "Can't set type to null");
            }
        }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        SymbolSet ISymbol.ChildSymbols => SymbolSet.Empty;

        public ForeachExpressionSyntax(
            TextSpan span,
            bool isMutableBinding,
            Name fullVariableName,
            ITypeSyntax? typeSyntax,
            ExpressionSyntax inExpression,
            BlockSyntax block)
            : base(span)
        {
            IsMutableBinding = isMutableBinding;
            FullVariableName = fullVariableName;
            InExpression = inExpression;
            Block = block;
            TypeSyntax = typeSyntax;
        }

        public override string ToString()
        {
            var binding = IsMutableBinding ? "var " : "";
            return $"foreach {binding}{VariableName}: {TypeSyntax} in {InExpression} {Block}";
        }
    }
}
