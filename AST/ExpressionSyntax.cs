using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    // An expression is also a statement
    [Closed(
        typeof(UnaryExpressionSyntax),
        typeof(UnsafeExpressionSyntax),
        typeof(NewObjectExpressionSyntax),
        typeof(LiteralExpressionSyntax),
        typeof(NextExpressionSyntax),
        typeof(ReturnExpressionSyntax),
        typeof(WhileExpressionSyntax),
        typeof(MemberAccessExpressionSyntax),
        typeof(MoveExpressionSyntax),
        typeof(LoopExpressionSyntax),
        typeof(LifetimeExpressionSyntax),
        typeof(BlockSyntax),
        typeof(InvocationSyntax),
        typeof(ImplicitConversionExpression),
        typeof(ForeachExpressionSyntax),
        typeof(IfExpressionSyntax),
        typeof(AssignmentExpressionSyntax),
        typeof(BreakExpressionSyntax),
        typeof(BinaryExpressionSyntax),
        typeof(InstanceExpressionSyntax),
        typeof(NameSyntax))]
    public abstract class ExpressionSyntax : Syntax
    {
        /// <summary>
        /// If an expression has been poisoned, then it is errored in some way
        /// and we won't report errors against it in the future. We may also
        /// skip it for some processing.
        /// </summary>
        public bool Poisoned { get; private set; }

        private DataType? type;
        public DataType? Type
        {
            get => type;
            set
            {
                if (type != null)
                    throw new InvalidOperationException("Can't set type repeatedly");
                type = value ?? throw new ArgumentNullException(nameof(Type),
                           "Can't set type to null");
            }
        }

        protected ExpressionSyntax(TextSpan span)
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
