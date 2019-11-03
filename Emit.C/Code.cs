namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class Code
    {
        public CCodeBuilder Includes { get; } = new CCodeBuilder();
        public CCodeBuilder TypeIdDeclaration { get; } = new CCodeBuilder();
        public CCodeBuilder TypeDeclarations { get; } = new CCodeBuilder();
        public CCodeBuilder FunctionDeclarations { get; } = new CCodeBuilder();
        public CCodeBuilder StructDeclarations { get; } = new CCodeBuilder();
        public CCodeBuilder GlobalDefinitions { get; } = new CCodeBuilder();
        public CCodeBuilder Definitions { get; } = new CCodeBuilder();

        public override string ToString()
        {
            return string.Concat(
                Includes.Code,
                CCodeBuilder.LineTerminator,
                TypeIdDeclaration.Code,
                CCodeBuilder.LineTerminator,
                TypeDeclarations.Code,
                CCodeBuilder.LineTerminator,
                FunctionDeclarations.Code,
                CCodeBuilder.LineTerminator,
                StructDeclarations.Code,
                CCodeBuilder.LineTerminator,
                GlobalDefinitions.Code,
                CCodeBuilder.LineTerminator,
                Definitions.Code);
        }
    }
}
