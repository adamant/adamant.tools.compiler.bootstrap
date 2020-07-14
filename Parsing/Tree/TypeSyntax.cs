using System;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class TypeSyntax : Syntax, ITypeSyntax
    {
        private DataType? namedType;

        [DisallowNull]
        public DataType? NamedType
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
            : base(span)
        {
        }

        public void Poison()
        {
            Poisoned = true;
        }

        // Useful for debugging
        public abstract override string ToString();
    }
}
