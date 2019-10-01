using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class ExpressionSyntax : Syntax, IExpressionSyntax
    {
        /// <summary>
        /// If an expression has been poisoned, then it is errored in some way
        /// and we won't report errors against it in the future. We may also
        /// skip it for some processing.
        /// </summary>
        public bool Poisoned { get; private set; }

        private DataType? type;

        [DisallowNull]
        public DataType? Type
        {
            get => type;
            set
            {
                if (type != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                type = value ?? throw new ArgumentNullException(nameof(Type),
                           "Can't set type to null");
            }
        }

        protected ExpressionSyntax(TextSpan span)
            : base(span)
        {
        }

        public void Poison()
        {
            Poisoned = true;
        }

        // Useful for debugging
        public abstract override string ToString();
    }
}
