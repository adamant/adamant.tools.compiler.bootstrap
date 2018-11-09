using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class CallStatement : Statement
    {
        public readonly Place Place;
        public readonly string FunctionName;
        public IReadOnlyList<Place> Arguments { get; }
        private readonly List<Place> arguments = new List<Place>();

        public CallStatement(Place lvalue, string functionName)
        {
            Place = lvalue;
            FunctionName = functionName;
            Arguments = arguments.AsReadOnly();
        }
    }
}
