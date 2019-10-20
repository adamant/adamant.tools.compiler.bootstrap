using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    internal class ImplicitMoveTransferSyntax : IMoveTransferSyntax
    {
        public TextSpan Span { get; }

        private IExpressionSyntax expression;
        public ref IExpressionSyntax Expression => ref expression;

        [DisallowNull]
        public DataType? Type
        {
            get => type;
            set
            {
                if (type != null) throw new InvalidOperationException("Can't set type repeatedly");
                type = value ?? throw new ArgumentNullException(nameof(Type), "Can't set type to null");
            }
        }

        private DataType? type;
        public ImplicitMoveTransferSyntax(TextSpan span, IExpressionSyntax expression)
        {
            Span = span;
            this.expression = expression;
        }
    }
}
