using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analayze
{
    public class DeclaredTypesAnalyzer
    {
        public PackageSyntaxSymbol GetDeclaredTypes(PackageSyntax package, SyntaxAnnotation<DataType> typeAnnotations)
        {
            var packageSymbol = new PackageSyntaxSymbol(package);
            // TODO actually get the declared symbols once there are some to get
            return packageSymbol;
        }
    }
}