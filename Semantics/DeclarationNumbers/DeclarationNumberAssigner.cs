using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Promises;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DeclarationNumbers
{
    public class DeclarationNumberAssigner : SyntaxWalker
    {
        private readonly Dictionary<Name, Promise<int?>> lastDeclaration = new Dictionary<Name, Promise<int?>>();
        private DeclarationNumberAssigner() { }

        public static void AssignIn(IEnumerable<IEntityDeclarationSyntax> entities)
        {
            foreach (var entity in entities)
            {
                var assigner = new DeclarationNumberAssigner();
                assigner.WalkNonNull(entity);
                assigner.AssignSingleDeclarationsNull();
            }
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IClassDeclarationSyntax _:
                    // Skip, will see members separately
                    return;
                case INamedParameterSyntax syn:
                    ProcessDeclaration(syn.Name, syn.DeclarationNumber);
                    break;
                case IVariableDeclarationStatementSyntax syn:
                    ProcessDeclaration(syn.Name, syn.DeclarationNumber);
                    break;
                case IForeachExpressionSyntax syn:
                    ProcessDeclaration(syn.VariableName, syn.DeclarationNumber);
                    break;
            }
            WalkChildren(syntax);
        }

        private void ProcessDeclaration(Name name, Promise<int?> declarationNumber)
        {
            declarationNumber.BeginFulfilling();
            if (lastDeclaration.TryGetValue(name, out var previousDeclarationNumber))
            {
                if (previousDeclarationNumber.State == PromiseState.InProgress)
                    // There is at least two declarations, start counting from 1
                    previousDeclarationNumber.Fulfill(1);
                declarationNumber.Fulfill(previousDeclarationNumber.Result + 1);
                lastDeclaration[name] = declarationNumber;
            }
            else
                lastDeclaration.Add(name, declarationNumber);
        }

        private void AssignSingleDeclarationsNull()
        {
            var unfulfilledDeclarationNumbers = lastDeclaration.Values
                                                    .Where(declarationNumber => declarationNumber.State == PromiseState.InProgress);
            foreach (var declarationNumber in unfulfilledDeclarationNumbers)
                // Only a single declaration, don't apply unique numbers
                declarationNumber.Fulfill(null);
        }
    }
}
