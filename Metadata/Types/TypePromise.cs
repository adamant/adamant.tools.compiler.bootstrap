using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

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
        private TType DataType { get; set; }

        [DebuggerHidden]
        public void BeginFulfilling()
        {
            Requires.That(nameof(State), State == PromiseState.Pending, "must be pending is " + State);
            State = PromiseState.InProgress;
        }

        [DebuggerHidden]
        public TType Fulfill(TType type)
        {
            Requires.That(nameof(State), State == PromiseState.InProgress, "must be in progress is " + State);
            State = PromiseState.Fulfilled;
            DataType = type ?? throw new ArgumentNullException(nameof(type));
            return type;
        }

        [DebuggerHidden]
        public TType Fulfilled()
        {
            if (State != PromiseState.Fulfilled)
                throw new InvalidOperationException("Promise not fulfilled");

            return DataType;
        }

        [DebuggerHidden]
        public TType Resolved()
        {
            if (State != PromiseState.Fulfilled)
                throw new InvalidOperationException("Promise not fulfilled");

            return (TType)DataType.AssertResolved();
        }

        // Useful for debugging
        public override string ToString()
        {
            switch (State)
            {
                case PromiseState.Pending:
                    return "⧼pending⧽";
                case PromiseState.InProgress:
                    return "⧼in progress⧽";
                case PromiseState.Fulfilled:
                    return DataType.ToString();
                default:
                    throw NonExhaustiveMatchException.ForEnum(State);
            }
        }
    }

    public class TypePromise : TypePromise<DataType>
    {
    }
}
