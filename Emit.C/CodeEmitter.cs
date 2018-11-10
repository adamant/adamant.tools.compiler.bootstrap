using Adamant.Tools.Compiler.Bootstrap.Emit.C.Properties;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class CodeEmitter : Emitter
    {
        [NotNull] public static string RuntimeLibraryCode => Resources.RuntimeLibraryCode.NotNull();
        [NotNull] public const string RuntimeLibraryCodeFileName = "RuntimeLibrary.c";
        [NotNull] public static string RuntimeLibraryHeader => Resources.RuntimeLibraryHeader.NotNull();
        [NotNull] public const string RuntimeLibraryHeaderFileName = "RuntimeLibrary.h";

        [NotNull] private readonly PackageEmitter packageEmitter;
        [NotNull] private readonly Code code = new Code();

        public CodeEmitter()
        {
            var nameMangler = new NameMangler();
            var typeConverter = new TypeConverter(nameMangler);
            var parameterConverter = new ParameterConverter(nameMangler, typeConverter);
            var controlFlowEmitter = new ControlFlowEmitter(nameMangler, typeConverter);
            var declarationEmitter = new DeclarationEmitter(nameMangler, parameterConverter, typeConverter, controlFlowEmitter);
            packageEmitter = new PackageEmitter(nameMangler, declarationEmitter);
            packageEmitter.EmitPreamble(code);
        }

        public override void Emit([NotNull] Package package)
        {
            packageEmitter.Emit(package, code);
        }

        public override string GetEmittedCode()
        {
            packageEmitter.EmitPostamble(code);
            return code.ToString();
        }
    }
}
