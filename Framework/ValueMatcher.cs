using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public struct ValueMatcher<T>
    {
        private readonly T value;

        [DebuggerStepThrough]
        public ValueMatcher(T value)
        {
            this.value = value;
        }

        [DebuggerStepThrough]
        public void With(Action<MatchBuilder<T>> buildArms)
        {
            var arms = new List<MatchArm<T>>();
            buildArms(new MatchBuilder<T>(arms));
            foreach (var arm in arms)
            {
                if (arm.Condition(value))
                {
                    arm.Action(value);
                    return;
                }
            }

            throw new NonExhaustiveMatchException(value?.GetType()?.Name);
        }
    }

    public struct ValueMatcher<T, R>
    {
        private readonly T value;

        [DebuggerStepThrough]
        public ValueMatcher(T value)
        {
            this.value = value;
        }

        [DebuggerStepThrough]
        public R With(Action<MatchBuilder<T, R>> buildArms)
        {
            var arms = new List<MatchArm<T, R>>();
            buildArms(new MatchBuilder<T, R>(arms));

            foreach (var arm in arms)
            {
                if (arm.Condition(value))
                    return arm.Action(value);
            }

            throw new NonExhaustiveMatchException($"Matching value of type {value?.GetType().FullName}");
        }
    }
}
