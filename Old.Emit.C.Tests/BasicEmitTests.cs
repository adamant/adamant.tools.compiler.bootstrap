using Adamant.Tools.Compiler.Bootstrap.API;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Emit;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Emit.C.Tests
{
    public class BasicEmitTests
    {
        [Fact]
        [Category("Emitter")]
        public void PreambleSetsUpEachSection()
        {
            var code = new Code();
            new CEmitter().EmitPreamble(code);
            const string expected =
@"#include ""RuntimeLibrary.h""

// Type ID Declarations
enum Type_ID
{
    never__0__0TypeID = 0,

// Type Declarations

// Function Declarations

// Class Declarations

// Global Definitions

// Definitions
";
            Assert.Equal(expected.NormalizeLineEndings(CCodeBuilder.LineTerminator), code.ToString());
            Assert.Equal(1, code.TypeIdDeclaration.CurrentIndentDepth);
        }

        [Fact]
        [Category("Emitter")]
        public void PostambleClosesTypeIdEnum()
        {
            var code = new Code();
            code.TypeIdDeclaration.BeginBlock();
            new CEmitter().EmitPostamble(code);
            const string expected =
@"{
};
typedef enum Type_ID Type_ID;
";
            Assert.Equal(expected.NormalizeLineEndings(CCodeBuilder.LineTerminator), code.TypeIdDeclaration.Code);
        }

        [Fact]
        [Category("Emitter")]
        public void EmitsEntryPointAdapterForNoArgVoidReturnMain()
        {
            const string code =
@"public fn main() -> void
{
}";

            var codeFile = new CodeFile(new CodePath(nameof(EmitsEntryPointAdapterForNoArgVoidReturnMain)), new CodeText(code));
            var package = new AdamantCompiler().CompilePackage(codeFile.Yield());
            var cCode = new Code();
            new CEmitter().EmitEntryPointAdapter(cCode, package);
            const string expected =
@"// Entry Point Adapter
int32_t main(const int argc, char const * const * const argv)
{
    main__0();
    return 0;
}
";
            Assert.Equal(expected.NormalizeLineEndings(CCodeBuilder.LineTerminator), cCode.Definitions.Code);
        }
    }
}
