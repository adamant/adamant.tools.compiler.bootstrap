using Adamant.Tools.Compiler.Bootstrap.Framework;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C.Tests
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
    }
}
