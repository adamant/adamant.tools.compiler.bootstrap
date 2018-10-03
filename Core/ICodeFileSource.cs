using System.Threading.Tasks;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// <summary>
    /// A source for
    /// </summary>
    public interface ICodeFileSource
    {
        CodeFile Load();
        Task<CodeFile> LoadAsync();
    }
}
