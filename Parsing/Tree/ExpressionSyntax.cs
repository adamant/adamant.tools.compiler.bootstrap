using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class ExpressionSyntax : Syntax, IExpressionSyntax
    {
        /// <summary>
        /// If an expression has been poisoned, then it is errored in some way
        /// and we won't report errors against it in the future. We may also
        /// skip it for some processing.
        /// </summary>
        public bool Poisoned { [DebuggerStepThrough] get; private set; }

        private DataType? type;
        [DisallowNull]
        public DataType? Type
        {
            [DebuggerStepThrough]
            get => type;
            set
            {
                if (type != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                type = value ?? throw new ArgumentNullException(nameof(Type),
                           "Can't set type to null");
            }
        }

        private ExpressionSemantics? valueSemantics;

        [DisallowNull]
        public ExpressionSemantics? Semantics
        {
            [DebuggerStepThrough]
            get => valueSemantics;
            set
            {
                if (valueSemantics != null)
                    throw new InvalidOperationException("Can't set semantics repeatedly");
                valueSemantics = value ?? throw new ArgumentNullException(nameof(value));
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

        protected abstract OperatorPrecedence ExpressionPrecedence { get; }

        public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        {
            return surroundingPrecedence > ExpressionPrecedence ? $"({this})" : ToString();
        }
    }
}
