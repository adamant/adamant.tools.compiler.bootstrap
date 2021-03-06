using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.InferredSyntax
{
    internal class FunctionInvocationExpressionSyntax : IUnqualifiedInvocationExpressionSyntax
    {
        private readonly FixedSet<FunctionSymbol> possibleReferents;
        public LexicalScope ContainingLexicalScope
        {
            [DebuggerStepThrough]
            get => throw new NotSupportedException($"{nameof(ContainingLexicalScope)} not supported on inferred {nameof(FunctionInvocationExpressionSyntax)}");
            [DebuggerStepThrough]
            set => throw new NotSupportedException($"{nameof(ContainingLexicalScope)} not supported on inferred {nameof(FunctionInvocationExpressionSyntax)}");
        }
        public TextSpan Span { get; }
        public NamespaceName Namespace { get; }
        public Name InvokedName { get; }
        public TextSpan InvokedNameSpan { get; }
        public FixedList<IArgumentSyntax> Arguments { get; }
        public Promise<FunctionSymbol?> ReferencedSymbol { get; } = new Promise<FunctionSymbol?>();
        IPromise<InvocableSymbol?> IInvocationExpressionSyntax.ReferencedSymbol => ReferencedSymbol;

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
            NamespaceName ns,
            Name invokedName,
            TextSpan invokedNameSpan,
            FixedList<IArgumentSyntax> arguments,
            FixedSet<FunctionSymbol> possibleReferents)
        {
            this.possibleReferents = possibleReferents;
            InvokedName = invokedName;
            InvokedNameSpan = invokedNameSpan;
            Span = span;
            Namespace = ns;
            Arguments = arguments;
        }

        public IEnumerable<IPromise<FunctionSymbol>> LookupInContainingScope()
        {
            return possibleReferents.Select(AcyclicPromise.ForValue);
        }

        public override string ToString()
        {
            return Namespace == NamespaceName.Global
                ? $"{InvokedName}({string.Join(", ", Arguments)})"
                : $"{Namespace}.{InvokedName}({string.Join(", ", Arguments)})";
        }

        public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        {
            return ToString();
        }
    }
}
