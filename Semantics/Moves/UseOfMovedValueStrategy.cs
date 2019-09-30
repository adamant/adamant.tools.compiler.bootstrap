using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Moves
{
    public class UseOfMovedValueStrategy : IDataFlowAnalysisStrategy<VariableFlags>
    {
        #region Singleton
        public static UseOfMovedValueStrategy Instance = new UseOfMovedValueStrategy();

        private UseOfMovedValueStrategy() { }
        #endregion

        public IDataFlowAnalysisChecker<VariableFlags> CheckerFor(IFunctionDeclarationSyntax function, Diagnostics diagnostics)
        {
            return new UseOfMovedValueChecker(function, diagnostics);
        }
    }
}
