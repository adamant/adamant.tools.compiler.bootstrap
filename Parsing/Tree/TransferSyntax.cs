using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class TransferSyntax : Syntax, ITransferSyntax
    {
        private IExpressionSyntax expression;
        public ref IExpressionSyntax Expression => ref expression;

        private DataType? type;

        [DisallowNull]
        public DataType? Type
        {
            get => type;
            set
            {
                if (type != null) throw new InvalidOperationException("Can't set type repeatedly");
                type = value ?? throw new ArgumentNullException(nameof(System.Type), "Can't set type to null");
            }
        }

        protected TransferSyntax(TextSpan span, IExpressionSyntax expression)
            : base(span)
        {
            this.expression = expression;
        }

    }
}
