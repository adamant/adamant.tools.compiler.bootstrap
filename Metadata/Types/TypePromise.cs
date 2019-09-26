using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

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
    /// Once a <see cref="TypePromise"/> has been fulfilled it can't be changed.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public class TypePromise
    {
        public PromiseState State { get; private set; }
        private DataType? dataType;

        [DebuggerHidden]
        public void BeginFulfilling()
        {
            Requires.That(nameof(State), State == PromiseState.Pending, "must be pending is " + State);
            State = PromiseState.InProgress;
        }

        [DebuggerHidden]
        public DataType Fulfill(DataType type)
        {
            Requires.That(nameof(State), State == PromiseState.InProgress, "must be in progress is " + State);
            State = PromiseState.Fulfilled;
            dataType = type ?? throw new ArgumentNullException(nameof(type));
            return type;
        }

        [DebuggerHidden]
        public DataType Fulfilled()
        {
            if (State != PromiseState.Fulfilled)
                throw new InvalidOperationException("Promise not fulfilled");

            return dataType!;
        }


        /// <summary>
        /// Asserts that the type has been fulfilled and is known.
        /// </summary>
        [DebuggerHidden]
        public DataType Known()
        {
            if (State != PromiseState.Fulfilled)
                throw new InvalidOperationException("Promise not fulfilled");

            return dataType!.AssertKnown();
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
                    return dataType!.ToString();
                default:
                    throw ExhaustiveMatch.Failed(State);
            }
        }
    }
}
