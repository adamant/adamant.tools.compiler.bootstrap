using System.Text;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Emit.C
{
    public class NameMangler
    {
        private static readonly Regex UnderscoreRuns = new Regex("(_[_]+)", RegexOptions.Compiled);

        public string FunctionName(FunctionDeclaration function)
        {
            var builder = new StringBuilder(UnderscoreRuns.Replace(function.Name, "_$0"));
            builder.Append("__");
            builder.Append(function.Parameters.Count);
            return builder.ToString();
        }
    }
}
