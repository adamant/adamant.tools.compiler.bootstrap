using Adamant.Tools.Compiler.Bootstrap.CodeGen.Config;

namespace Adamant.Tools.Compiler.Bootstrap.CodeGen
{
    public static class CodeBuilder
    {
        public static string GenerateTree(Grammar grammar)
        {
            var template = new TreeCodeTemplate(grammar);
            return template.TransformText();
        }

        public static string GenerateChildren(Grammar grammar)
        {
            var template = new ChildrenCodeTemplate(grammar);
            return template.TransformText();
        }
    }
}
