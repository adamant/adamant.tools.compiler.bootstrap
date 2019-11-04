using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.InferredSyntax
{
    internal class FunctionInvocationExpressionSyntax : IFunctionInvocationExpressionSyntax
    {
        public TextSpan Span { get; }

        public ICallableNameSyntax FunctionNameSyntax { get; }
        public Name FullName { get; }
        public FixedList<IArgumentSyntax> Arguments { get; }

        private DataType? type;
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

        public FunctionInvocationExpressionSyntax(
            TextSpan span,
            ICallableNameSyntax functionNameSyntax,
            Name fullName,
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
    }
}
