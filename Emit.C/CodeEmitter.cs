using Adamant.Tools.Compiler.Bootstrap.Emit.C.Properties;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Emit.C
{
    public class CodeEmitter : Emitter
    {
        [NotNull] public static string RuntimeLibraryCode => Resources.RuntimeLibraryCode.AssertNotNull();
        [NotNull] public const string RuntimeLibraryCodeFileName = "RuntimeLibrary.c";
        [NotNull] public static string RuntimeLibraryHeader => Resources.RuntimeLibraryHeader.AssertNotNull();
        [NotNull] public const string RuntimeLibraryHeaderFileName = "RuntimeLibrary.h";

        [NotNull] private readonly IEmitter<Package> packageEmitter;

        public CodeEmitter()
        {
            var nameMangler = new NameMangler();
            var parameterConverter = new ParameterConverter();
            var typeConverter = new TypeConverter();
            var declarationEmitter = new DeclarationEmitter(nameMangler, parameterConverter, typeConverter);
            packageEmitter = new PackageEmitter(nameMangler, declarationEmitter);
        }

        public override string Emit([NotNull] Package package)
        {
            var code = new Code();
            packageEmitter.Emit(package, code);
            return code.ToString();
        }
    }
}
