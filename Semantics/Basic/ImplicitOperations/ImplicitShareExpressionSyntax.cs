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

        // TODO maybe set from constructor and disallow all changes
        private IBindingSymbol? sharedSymbol;
        [DisallowNull]
        public IBindingSymbol? SharedSymbol
        {
            [DebuggerStepThrough]
            get => sharedSymbol;
            set
            {
                if (sharedSymbol != null) throw new InvalidOperationException("Can't set referenced symbol repeatedly");
                sharedSymbol = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public ImplicitShareExpressionSyntax(IExpressionSyntax referent, DataType type)
        {
            Span = referent.Span;
            this.referent = referent;
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public override string ToString()
        {
            return $"⟦share⟧ {Referent}";
        }
    }
}
