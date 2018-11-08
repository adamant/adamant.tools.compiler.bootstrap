using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    /// <summary>
    /// The type of something under analysis may be in one of three states
    /// represented by the
    /// </summary>
    public class TypeAnalysis
    {
        public AnalysisState State { get; private set; }
        [CanBeNull] public DataType DataType { get; private set; }

        public void BeginComputing()
        {
            Requires.That(nameof(State), State == AnalysisState.NotComputed);
            State = AnalysisState.BeingComputed;
        }

        [NotNull]
        public DataType Computed([NotNull] DataType type)
        {
            Requires.That(nameof(State), State == AnalysisState.BeingComputed);
            Requires.NotNull(nameof(type), type);
            State = AnalysisState.Computed;
            DataType = type;
            return type;
        }

        [NotNull]
        public DataType Resolve([NotNull] DataType type)
        {
            Requires.That(nameof(State), State == AnalysisState.Computed);
            Requires.NotNull(nameof(type), type);
            Requires.That(nameof(type), !DataType.AssertNotNull().IsResolved);
            Requires.That(nameof(type), type.IsResolved);
            DataType = type;
            return type;
        }

        [CanBeNull]
        public static implicit operator DataType(TypeAnalysis analysis)
        {
            return analysis.DataType;
        }

        [NotNull]
        public DataType AssertComputed()
        {
            if (State != AnalysisState.Computed)
                throw new InvalidOperationException("Type analysis still being computed");

            return DataType.AssertNotNull();
        }

        [NotNull]
        public DataType AssertResolved()
        {
            if (State != AnalysisState.Computed)
                throw new InvalidOperationException("Type analysis still being computed");

            return DataType.AssertNotNull().AssertResolved();
        }
    }
}
