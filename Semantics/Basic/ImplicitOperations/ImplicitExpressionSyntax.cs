using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    internal abstract class ImplicitExpressionSyntax
    {
        [SuppressMessage("Microsoft.Performance", "CA1822: Mark members as static",
            Justification = "This IS instance data!")]
        public TextSpan Span { [DebuggerStepThrough] get; }

        private readonly DataType dataType;
        [DisallowNull]
        public DataType? DataType
        {
            [DebuggerStepThrough]
            get => dataType;
            // Type is always set by the constructor, so it can't be set again
            set => throw new InvalidOperationException("Can't set type repeatedly");
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

        protected ImplicitExpressionSyntax(DataType dataType, TextSpan span, ExpressionSemantics? semantics = null)
        {
            Span = span;
            this.dataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
            this.semantics = semantics;
        }
    }
}
