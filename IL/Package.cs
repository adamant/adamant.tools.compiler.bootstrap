using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IL.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.IL
{
    public class Package
    {
        public readonly string Name;
        public IReadOnlyList<Declaration> Declarations { get; }
        private readonly List<Declaration> declarations = new List<Declaration>();

        public Package(string name)
        {
            Name = name;
            Declarations = declarations.AsReadOnly();
        }

        public void Add(Declaration declaration)
        {
            declarations.Add(declaration);
        }
    }
}
