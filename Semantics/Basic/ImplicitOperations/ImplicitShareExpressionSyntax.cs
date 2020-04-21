using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    internal class ImplicitShareExpressionSyntax : IShareExpressionSyntax
    {
        public TextSpan Span { get; }

        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification =
            "Can't be readonly because a reference to it is exposed")]
        private IExpressionSyntax referent;
        public ref IExpressionSyntax Referent => ref referent;


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

        private DataType? type;

        public ImplicitShareExpressionSyntax(TextSpan span, INameExpressionSyntax referent)
        {
            Span = span;
            this.referent = referent;
        }

        public override string ToString()
        {
            return $"(share {Referent})";
        }
    }
}
