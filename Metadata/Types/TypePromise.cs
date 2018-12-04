using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// This represents a DataType that may not be computed yet. There are three
    /// states:
    ///
    /// * Pending
    /// * In Progress
    /// * Fulfilled
    ///
    /// Once a <see cref="TypePromise"/> has been fulfilled it can't be changed
    /// except to resolve an unresolved type.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public class TypePromise<TType>
        where TType : DataType
    {
        public PromiseState State { get; private set; }
        [CanBeNull] private TType DataType { get; set; }

        [DebuggerHidden]
        public void BeginFulfilling()
        {
            Requires.That(nameof(State), State == PromiseState.Pending);
            State = PromiseState.InProgress;
        }

        [DebuggerHidden]
        [NotNull]
        public TType Fulfill([NotNull] TType type)
        {
            Requires.That(nameof(State), State == PromiseState.InProgress);
            Requires.NotNull(nameof(type), type);
            State = PromiseState.Fulfilled;
            DataType = type;
            return type;
        }

        [DebuggerHidden]
        [NotNull]
        public TType Fulfilled()
        {
            if (State != PromiseState.Fulfilled)
                throw new InvalidOperationException("Promise not fulfilled");

            return DataType.NotNull();
        }

        [DebuggerHidden]
        [NotNull]
        public TType Resolved()
        {
            if (State != PromiseState.Fulfilled)
                throw new InvalidOperationException("Promise not fulfilled");

            return (TType)DataType.NotNull().AssertResolved();
        }

        // Useful for debugging
        [NotNull]
        public override string ToString()
        {
            switch (State)
            {
                case PromiseState.Pending:
                    return "⧼pending⧽";
                case PromiseState.InProgress:
                    return "⧼in progress⧽";
                case PromiseState.Fulfilled:
                    return DataType.NotNull().ToString();
                default:
                    throw NonExhaustiveMatchException.ForEnum(State);
            }
        }
    }

    public class TypePromise : TypePromise<DataType>
    {
    }
}
