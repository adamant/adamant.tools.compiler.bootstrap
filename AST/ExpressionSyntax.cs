using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    // An expression is also a statement
    public abstract class ExpressionSyntax : StatementSyntax
    {
        public TextSpan Span { get; }
        private DataType type;
        public DataType Type
        {
            get => type;
            set
            {
                if (type != null) throw new InvalidOperationException("Can't set type repeatedly");
                type = value ?? throw new ArgumentException();
            }
        }

        protected ExpressionSyntax(TextSpan span)
        {
            Span = span;
        }

        // Useful for debugging
        public abstract override string ToString();
    }
}
