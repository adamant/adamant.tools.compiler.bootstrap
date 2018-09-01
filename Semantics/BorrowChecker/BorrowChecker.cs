using ILFunctionDeclaration = Adamant.Tools.Compiler.Bootstrap.IL.Declarations.FunctionDeclaration;
using ILPackage = Adamant.Tools.Compiler.Bootstrap.IL.Package;
using ILTypeDeclaration = Adamant.Tools.Compiler.Bootstrap.IL.Declarations.TypeDeclaration;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BorrowChecker
{
    public class BorrowChecker
    {
        public void Check(ILPackage package)
        {
            foreach (var declaration in package.Declarations)
                switch (declaration)
                {
                    case ILFunctionDeclaration function:
                        Check(function);
                        break;

                    case ILTypeDeclaration typeDeclaration:
                        Check(typeDeclaration);
                        break;
                }
        }

        private void Check(ILTypeDeclaration typeDeclaration)
        {
            // Currently nothing to check
        }

        private void Check(ILFunctionDeclaration function)
        {
            // Compute aliveness at point after each statement
        }
    }
}
