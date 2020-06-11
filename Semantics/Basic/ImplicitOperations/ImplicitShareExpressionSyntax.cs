using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    internal class ImplicitShareExpressionSyntax : IShareExpressionSyntax
    {
        public TextSpan Span { [DebuggerStepThrough] get; }

        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification =
            "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;
        public ref IExpressionSyntax Referent
        {
            [DebuggerStepThrough]
            get => ref referent;
        }

        private readonly DataType type;
        [DisallowNull]
        public DataType? Type
        {
            [DebuggerStepThrough]
            get => type;
            // Type is always set by the constructor, so it can't be set again
            set => throw new InvalidOperationException("Can't set type repeatedly");
        }

        [DisallowNull]
        public IBindingSymbol SharedSymbol { [DebuggerStepThrough] get; }

        public ImplicitShareExpressionSyntax(IExpressionSyntax referent, DataType type, IBindingSymbol sharedSymbol)
        {
            Span = referent.Span;
            this.referent = referent;
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.SharedSymbol = sharedSymbol ?? throw new ArgumentNullException(nameof(sharedSymbol));
        }

        public override string ToString()
        {
            return $"⟦share⟧ {Referent}";
        }

        public string ToGroupedString()
        {
            return $"({ToString()})";
        }
    }
}
