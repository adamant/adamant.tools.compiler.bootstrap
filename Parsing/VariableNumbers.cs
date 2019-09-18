using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    /// <summary>
    /// Used to track which variable number we are on for each variable so we
    /// can assign unique names to them.
    /// </summary>
    internal class VariableNumbers
    {
        private readonly Dictionary<string, int> nextNumber = new Dictionary<string, int>();

        public SimpleName VariableName(string text)
        {
            //if (text == "_") return SpecialName.Underscore;
            // Just try putting in first index, if it isn't already there, no problem
            nextNumber.TryAdd(text, 0);
            var number = nextNumber[text];
            nextNumber[text] += 1;
            return SimpleName.Variable(text, number);
        }
    }
}
