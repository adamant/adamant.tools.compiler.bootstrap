namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.EndStatements
{
    public class IfStatement : EndStatement
    {
        internal override void ToString(AsmBuilder builder)
        {
            builder.AppendLine("if goto else goto");
        }
    }
}
