using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public static class ExpressionSemanticsExtensions
    {
        public static string Action(this ExpressionSemantics valueSemantics)
        {
            string mutability = valueSemantics switch
            {
                ExpressionSemantics.Never => "never",
                ExpressionSemantics.Void => "void",
                ExpressionSemantics.Move => "move",
                ExpressionSemantics.Copy => "copy",
                ExpressionSemantics.Acquire => "own",
                ExpressionSemantics.Borrow => "borrow",
                ExpressionSemantics.Share => "share",
                ExpressionSemantics.CreateReference => "ref",
                _ => throw ExhaustiveMatch.Failed(valueSemantics),
            };

            return mutability;
        }

        /// <summary>
        /// Validates that expression semantics have been assigned.
        /// </summary>
        [DebuggerHidden]
        public static ExpressionSemantics Assigned(this ExpressionSemantics? semantics)
        {
            return semantics ?? throw new InvalidOperationException("Expression semantics not assigned");
        }
    }
}
