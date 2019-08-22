using System;
using System.Collections.Generic;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NextExpressionSyntax : ExpressionSyntax
    {
        public NextExpressionSyntax(TextSpan span)
            : base(span)
        {
        }

        public override string ToString()
        {
            return "next";
        }
    }
}
