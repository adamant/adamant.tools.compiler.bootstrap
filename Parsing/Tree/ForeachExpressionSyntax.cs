using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ForeachExpressionSyntax : ExpressionSyntax, IForeachExpressionSyntax
    {
        public bool IsMutableBinding { get; }
        Name IMetadata.FullName => FullVariableName;
        private Name FullVariableName { get; }
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public SimpleName VariableName => FullVariableName.UnqualifiedName;

        public bool VariableIsLiveAfterAssignment { get; set; } = true;

        public ITypeSyntax? TypeSyntax { get; }
        DataType IBindingMetadata.Type => VariableType ?? throw new InvalidOperationException();
        private DataType? variableType;

        [DisallowNull]
        public DataType? VariableType
        {
            get => variableType;
            set
            {
                if (variableType != null)
                    throw new InvalidOperationException("Can't set VariableType repeatedly");
                variableType =
                    value ?? throw new ArgumentNullException(nameof(value), "Can't set VariableType to null");
            }
        }
        private IExpressionSyntax inExpression;
        public ref IExpressionSyntax InExpression => ref inExpression;

        public IBlockExpressionSyntax Block { get; }

        public ForeachExpressionSyntax(
            TextSpan span,
            bool isMutableBinding,
            Name fullVariableName,
            ITypeSyntax? typeSyntax,
            IExpressionSyntax inExpression,
            IBlockExpressionSyntax block)
            : base(span)
        {
            IsMutableBinding = isMutableBinding;
            FullVariableName = fullVariableName;
            this.inExpression = inExpression;
            Block = block;
            TypeSyntax = typeSyntax;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            var binding = IsMutableBinding ? "var " : "";
            return $"foreach {binding}{VariableName}: {TypeSyntax} in {InExpression} {Block}";
        }
    }
}
