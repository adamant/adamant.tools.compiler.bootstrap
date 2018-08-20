namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.EndStatements
{
    public class ReturnStatement : EndStatement
    {
        internal override void ToString(AsmBuilder builder)
        {
            builder.AppendLine("return");
        }
    }
}
