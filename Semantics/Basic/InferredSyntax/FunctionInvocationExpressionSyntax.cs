using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.InferredSyntax
{
    internal class FunctionInvocationExpressionSyntax : IFunctionInvocationExpressionSyntax
    {
        public TextSpan Span { get; }

        public IInvocableNameSyntax FunctionNameSyntax { get; }
        public MaybeQualifiedName FullName { get; }
        public FixedList<IArgumentSyntax> Arguments { get; }

        private DataType? dataType;
        [DisallowNull]
        public DataType? DataType
        {
            get => dataType;
            set
            {
                if (dataType != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                dataType = value ?? throw new ArgumentNullException(nameof(DataType), "Can't set type to null");
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

        public FunctionInvocationExpressionSyntax(
            TextSpan span,
            IInvocableNameSyntax functionNameSyntax,
            MaybeQualifiedName fullName,
            FixedList<IArgumentSyntax> arguments)
        {
            Span = span;
            FunctionNameSyntax = functionNameSyntax;
            FullName = fullName;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"{FullName}({string.Join(", ", Arguments)})";
        }

        public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        {
            return ToString();
        }
    }
}
