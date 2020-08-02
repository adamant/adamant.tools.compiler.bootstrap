using Adamant.Tools.Compiler.Bootstrap.CodeGen.Config;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen
{
    public static class CodeBuilder
    {
        public static string Generate(Grammar grammar)
        {
            var template = new CodeTemplate(grammar);
            return template.TransformText();
        }
    }
}
