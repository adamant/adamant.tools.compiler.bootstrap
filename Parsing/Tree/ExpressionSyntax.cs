using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

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

        private DataType? dataType;
        [DisallowNull]
        public DataType? DataType
        {
            [DebuggerStepThrough]
            get => dataType;
            set
            {
                if (dataType != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                dataType = value ?? throw new ArgumentNullException(nameof(DataType),
                           "Can't set type to null");
            }
        }

        private ExpressionSemantics? semantics;

        [DisallowNull]
        public ExpressionSemantics? Semantics
        {
            [DebuggerStepThrough]
            get => semantics;
            set
            {
                if (semantics != null)
                    throw new InvalidOperationException("Can't set semantics repeatedly");
                semantics = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        protected ExpressionSyntax(TextSpan span, ExpressionSemantics? semantics = null)
            : base(span)
        {
            this.semantics = semantics;
        }

        public void Poison()
        {
            Poisoned = true;
        }

        protected abstract OperatorPrecedence ExpressionPrecedence { get; }

        public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        {
            return surroundingPrecedence > ExpressionPrecedence ? $"({this})" : ToString();
        }
    }
}
