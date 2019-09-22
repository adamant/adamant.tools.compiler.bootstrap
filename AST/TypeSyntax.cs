using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(ReferenceLifetimeSyntax),
        typeof(SelfTypeSyntax),
        typeof(TypeNameSyntax),
        typeof(MutableTypeSyntax))]
    public abstract class TypeSyntax : Syntax
    {
        public TextSpan Span { get; }

        private DataType namedType;

        public DataType NamedType
        {
            get => namedType;
            set
            {
                if (namedType != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                namedType = value ?? throw new ArgumentNullException(nameof(NamedType),
                           "Can't set type to null");
            }
        }

        /// <summary>
        /// If an type has been poisoned, then it is errored in some way
        /// and we won't report errors against it in the future. We may also
        /// skip it for some processing.
        /// </summary>
        public bool Poisoned { get; private set; }

        protected TypeSyntax(TextSpan span)
        {
            Span = span;
        }

        public void Poison()
        {
            Poisoned = true;
        }

        // Useful for debugging
        public abstract override string ToString();
    }
}
