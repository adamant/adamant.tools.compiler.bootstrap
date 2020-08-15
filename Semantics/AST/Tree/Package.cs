using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class Package
    {
        public FixedList<IDeclaration> Declarations { get; }
        public FixedList<INonMemberDeclaration> NonMemberDeclarations { get; }

        public Package(FixedList<INonMemberDeclaration> nonMemberDeclarations)
        {
            Declarations = GetAllDeclarations(nonMemberDeclarations).ToFixedList();
            NonMemberDeclarations = nonMemberDeclarations;
        }

        private static IEnumerable<IDeclaration> GetAllDeclarations(
            IEnumerable<INonMemberDeclaration> nonMemberDeclarations)
        {
            var declarations = new Queue<IDeclaration>();
            declarations.EnqueueRange(nonMemberDeclarations);
            while (declarations.TryDequeue(out var declaration))
            {
                yield return declaration;
                if (declaration is IClassDeclaration syn)
                    declarations.EnqueueRange(syn.Members);
            }
        }
    }
}
