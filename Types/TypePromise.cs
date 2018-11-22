using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
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
    public class TypePromise
    {
        public PromiseState State { get; private set; }
        [CanBeNull] public DataType DataType { get; private set; }

        public void Begin()
        {
            Requires.That(nameof(State), State == PromiseState.Pending);
            State = PromiseState.InProgress;
        }

        [NotNull]
        public DataType Fulfill([NotNull] DataType type)
        {
            Requires.That(nameof(State), State == PromiseState.InProgress);
            Requires.NotNull(nameof(type), type);
            State = PromiseState.Fulfilled;
            DataType = type;
            return type;
        }

        [NotNull]
        public DataType Resolve([NotNull] DataType type)
        {
            Requires.That(nameof(State), State == PromiseState.Fulfilled);
            Requires.NotNull(nameof(type), type);
            Requires.That(nameof(type), !DataType.NotNull().IsResolved);
            Requires.That(nameof(type), type.IsResolved);
            DataType = type;
            return type;
        }

        [CanBeNull]
        public static implicit operator DataType(TypePromise promise)
        {
            return promise.DataType;
        }

        [NotNull]
        public DataType Fulfilled()
        {
            if (State != PromiseState.Fulfilled)
                throw new InvalidOperationException("Promise not fulfilled");

            return DataType.NotNull();
        }

        [NotNull]
        public DataType Resolved()
        {
            if (State != PromiseState.Fulfilled)
                throw new InvalidOperationException("Promise not fulfilled");

            return DataType.NotNull().AssertResolved();
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
                    return DataType.NotNull().ToString();
                default:
                    throw NonExhaustiveMatchException.ForEnum(State);
            }
        }
    }
}
