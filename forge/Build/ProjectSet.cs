using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adamant.Tools.Compiler.Bootstrap.API;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using Adamant.Tools.Compiler.Bootstrap.Forge.Config;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics;

namespace Adamant.Tools.Compiler.Bootstrap.Forge.Build
{
    /// <summary>
    /// Represents and determines an order to build packages in
    /// </summary>
    internal class ProjectSet : IEnumerable<Project>
    {
        private readonly Dictionary<string, Project> projects = new Dictionary<string, Project>();

        public void AddAll(ProjectConfigSet configs)
        {
            foreach (var config in configs)
                GetOrAdd(config, configs);
        }

        private Project GetOrAdd(ProjectConfig config, ProjectConfigSet configs)
        {
            var projectDir = Path.GetDirectoryName(config.FullPath);
            if (projects.TryGetValue(projectDir, out var existingProject))
                return existingProject;

            // Add a placeholder to prevent cycles
            projects.Add(projectDir, null);
            var dependencies = config.Dependencies.Select(d =>
            {
                var dependencyConfig = configs[d.Key];
                var dependencyProject = GetOrAdd(dependencyConfig, configs);
                return new ProjectReference(d.Key, dependencyProject, d.Value.Trusted);
            }).ToList();

            var project = new Project(config, dependencies);
            projects[projectDir] = project;
            return project;
        }

        public IEnumerator<Project> GetEnumerator()
        {
            return projects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public async Task Build(TaskScheduler taskScheduler, bool verbose)
        {
            var taskFactory = new TaskFactory(taskScheduler);
            var projectBuilds = new Dictionary<Project, Task<Package>>();

            var projectBuildsSource = new TaskCompletionSource<FixedDictionary<Project, Task<Package>>>();
            var projectBuildsTask = projectBuildsSource.Task;

            // Sort projects to detect cycles and so we can assume the tasks already exist
            var sortedProjects = TopologicalSort();
            var compiler = new AdamantCompiler();
            var consoleLock = new object();
            foreach (var project in sortedProjects)
            {
                var buildTask = taskFactory.StartNew(() =>
                    Build(compiler, project, projectBuildsTask, consoleLock))
                    .Unwrap(); // Needed because StartNew doesn't work intuitively with Async methods
                if (!projectBuilds.TryAdd(project, buildTask))
                    throw new Exception("Project added to build set twice");
            }
            projectBuildsSource.SetResult(projectBuilds.ToFixedDictionary());

            await Task.WhenAll(projectBuilds.Values);
        }

        private static async Task<Package> Build(
            AdamantCompiler compiler,
            Project project,
            Task<FixedDictionary<Project, Task<Package>>> projectBuildsTask,
            object consoleLock)
        {
            var projectBuilds = await projectBuildsTask;
            var sourceDir = Path.Combine(project.Path, "src");
            var sourcePaths = Directory.EnumerateFiles(sourceDir, "*.ad", SearchOption.AllDirectories);
            // Wait for the references, unfortunately, this requires an ugly loop.
            var referenceTasks = project.References.ToDictionary(r => r.Name, r => projectBuilds[r.Project]);
            var references = new Dictionary<string, Package>();
            foreach (var referenceTask in referenceTasks)
                references.Add(referenceTask.Key, await referenceTask.Value);

            lock (consoleLock)
            {
                Console.WriteLine($"Compiling {project.Name} ({project.Path})...");
            }
            var codeFiles = sourcePaths.Select(p => LoadCode(p, sourceDir, project.RootNamespace)).ToList();
            var package = compiler.CompilePackage(project.Name, codeFiles, references.ToFixedDictionary());
            // TODO switch to the async version of the compiler
            //var codeFiles = sourcePaths.Select(p => new CodePath(p)).ToList();
            //var references = project.References.ToDictionary(r => r.Name, r => projectBuilds[r.Project]);
            //var package = await compiler.CompilePackageAsync(project.Name, codeFiles, references);
            if (OutputDiagnostics(project, package, consoleLock))
                return package;

            var cacheDir = PrepareCacheDir(project);
            var codePath = EmitCode(project, package, cacheDir);
            var success = CompileCode(project, cacheDir, codePath, consoleLock);

            lock (consoleLock)
            {
                Console.WriteLine(success
                    ? $"Build SUCCEEDED {project.Name} ({project.Path})"
                    : $"Build FAILED {project.Name} ({project.Path})");
            }

            return package;
        }

        private static CodeFile LoadCode(
            string path,
            string sourceDir,
            FixedList<string> rootNamespace)
        {
            var relativeDirectory = Path.GetDirectoryName(Path.GetRelativePath(sourceDir, path));
            var ns = rootNamespace.Concat(relativeDirectory.SplitOrEmpty(Path.DirectorySeparatorChar)).ToFixedList();
            return CodeFile.Load(path, ns);
        }

        private static string PrepareCacheDir(Project project)
        {
            var cacheDir = Path.Combine(project.Path, ".forge-cache");
            Directory.CreateDirectory(cacheDir); // Ensure the cache directory exists

            // Clear the cache directory?
            var dir = new DirectoryInfo(cacheDir);
            foreach (var file in dir.EnumerateFiles()) file.Delete();
            foreach (var subDirectory in dir.EnumerateDirectories()) subDirectory.Delete(true);

            return cacheDir;
        }

        private static bool OutputDiagnostics(
            Project project,
            Package package,
            object consoleLock)
        {
            var diagnostics = package.Diagnostics;
            if (!diagnostics.Any()) return false;
            lock (consoleLock)
            {
                Console.WriteLine($@"Build FAILED {project.Name} ({project.Path})");
                foreach (var group in diagnostics.GroupBy(d => d.File))
                {
                    var fileDiagnostics = @group.ToList();
                    foreach (var diagnostic in fileDiagnostics.Take(10))
                    {
                        Console.WriteLine(
                            $@"{diagnostic.File.Reference}:{diagnostic.StartPosition.Line}:{diagnostic.StartPosition.Column} {diagnostic.Level} {diagnostic.ErrorCode}");
                        Console.WriteLine(@"    " + diagnostic.Message);
                    }

                    if (fileDiagnostics.Count > 10)
                    {
                        Console.WriteLine($"{@group.Key.Reference}");
                        Console.WriteLine(
                            $"    {fileDiagnostics.Skip(10).Count(d => d.Level >= DiagnosticLevel.CompilationError)} more errors not shown.");
                        Console.WriteLine(
                            $"    {fileDiagnostics.Skip(10).Count(d => d.Level == DiagnosticLevel.Warning)} more warnings not shown.");
                    }
                }
            }

            return true;
        }

        private static string EmitCode(Project project, Package package, string cacheDir)
        {
            var emittedPackages = new HashSet<Package>();
            var packagesToEmit = new Queue<Package>();
            packagesToEmit.Enqueue(package);

            var codeEmitter = new CodeEmitter();
            while (packagesToEmit.TryDequeue(out var currentPackage))
            {
                if (!emittedPackages.Contains(currentPackage))
                {
                    codeEmitter.Emit(currentPackage);
                    emittedPackages.Add(currentPackage);
                    packagesToEmit.EnqueueRange(currentPackage.References.Values);
                }
            }

            string outputPath;
            switch (project.Template)
            {
                case ProjectTemplate.App:
                {
                    outputPath = System.IO.Path.Combine(cacheDir, "program.c");
                }
                break;
                case ProjectTemplate.Lib:
                {
                    outputPath = System.IO.Path.Combine(cacheDir, "lib.c");
                }
                break;
                default:
                    throw NonExhaustiveMatchException.ForEnum(project.Template);
            }

            File.WriteAllText(outputPath, codeEmitter.GetEmittedCode(), Encoding.UTF8);

            return outputPath;
        }

        private static bool CompileCode(
            Project project,
            string cacheDir,
            string codePath,
            object consoleLock)
        {
            var compiler = new CLangCompiler();

            var runtimeLibrarySourcePath = System.IO.Path.Combine(cacheDir, CodeEmitter.RuntimeLibraryCodeFileName);
            File.WriteAllText(runtimeLibrarySourcePath, CodeEmitter.RuntimeLibraryCode, Encoding.UTF8);
            var runtimeLibraryHeaderPath = System.IO.Path.Combine(cacheDir, CodeEmitter.RuntimeLibraryHeaderFileName);
            File.WriteAllText(runtimeLibraryHeaderPath, CodeEmitter.RuntimeLibraryHeader, Encoding.UTF8);

            var sourceFiles = new[] { codePath, runtimeLibrarySourcePath };
            var headerSearchPaths = new[] { cacheDir };
            string outputPath;
            switch (project.Template)
            {
                case ProjectTemplate.App:
                {
                    outputPath = System.IO.Path.ChangeExtension(codePath, "exe");
                }
                break;
                case ProjectTemplate.Lib:
                {
                    outputPath = System.IO.Path.ChangeExtension(codePath, "dll");
                }
                break;
                default:
                    throw NonExhaustiveMatchException.ForEnum(project.Template);
            }

            lock (consoleLock)
            {
                Console.WriteLine($"CLang Compiling {project.Name} ({project.Path})...");
                var exitCode = compiler.Compile(ConsoleCompilerOutput.Instance, sourceFiles,
                    headerSearchPaths, outputPath);

                return exitCode == 0;
            }
        }

        private List<Project> TopologicalSort()
        {
            var projectAlive = projects.Values.ToDictionary(p => p, p => SortState.Alive);
            var sorted = new List<Project>(projects.Count);
            foreach (var project in projects.Values)
                TopologicalSortVisit(project, projectAlive, sorted);

            return sorted;
        }

        private static void TopologicalSortVisit(Project project, Dictionary<Project, SortState> state, List<Project> sorted)
        {
            switch (state[project])
            {
                case SortState.Dead: // Already visited
                    return;

                case SortState.Undead:// Cycle
                    throw new Exception("Dependency Cycle");

                case SortState.Alive:
                    state[project] = SortState.Undead;
                    foreach (var referencedProject in project.References.Select(r => r.Project))
                        TopologicalSortVisit(referencedProject, state, sorted);
                    state[project] = SortState.Dead;
                    sorted.Add(project);
                    return;

                default:
                    throw new InvalidEnumArgumentException("state[project]", (int)state[project], typeof(SortState));
            }
        }

        private enum SortState
        {
            Alive,
            Undead,
            Dead,
        }
    }
}
