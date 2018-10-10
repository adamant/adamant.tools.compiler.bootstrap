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
        private readonly HashSet<Project> projects = new HashSet<Project>();

        public bool Add(Project project)
        {
            return projects.Add(project);
        }

        public IEnumerator<Project> GetEnumerator()
        {
            return projects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void LoadDependencies()
        {
            var projectQueue = new Queue<Project>(projects);
            while (projectQueue.TryDequeue(out var project))
            {
                // TODO load up actual dependencies
            }
        }

        public async Task Build(TaskScheduler taskScheduler, bool verbose)
        {
            var taskFactory = new TaskFactory(taskScheduler);
            var projectBuilds = new Dictionary<Project, Task<Package>>();

            var projectBuildsSource = new TaskCompletionSource<IReadOnlyDictionary<Project, Task<Package>>>();
            var projectBuildsTask = projectBuildsSource.Task;

            // Sort projects to detect cycles and so we can assume the tasks already exist
            var sortedProjects = TopologicalSort();
            var compiler = new AdamantCompiler();
            var consoleLock = new object();
            foreach (var project in sortedProjects)
            {
                Console.WriteLine($"Compiling {project.Name} ({project.Path})...");
                var buildTask = taskFactory.StartNew(() =>
                    Build(compiler, project, projectBuildsTask, consoleLock))
                    .Unwrap(); // Needed because StartNew doesn't work intuitively with Async methods
                if (!projectBuilds.TryAdd(project, buildTask))
                    throw new Exception("Project added to build set twice");
            }
            projectBuildsSource.SetResult(projectBuilds.AsReadOnly());

            await Task.WhenAll(projectBuilds.Values);
        }

        private static async Task<Package> Build(
            AdamantCompiler compiler,
            Project project,
            Task<IReadOnlyDictionary<Project, Task<Package>>> projectBuildsTask,
            object consoleLock)
        {
            var projectBuilds = await projectBuildsTask;
            var sourceDir = Path.Combine(project.Path, "src");
            var sourcePaths = Directory.EnumerateFiles(sourceDir, "*.ad", SearchOption.AllDirectories);
            // TODO switch to the async version of the compiler
            //var codeFiles = sourcePaths.Select(p => new CodePath(p)).ToList();
            //var package = await compiler.CompilePackageAsync(codeFiles);
            var codeFiles = sourcePaths.Select(CodeFile.Load).ToList();
            var package = compiler.CompilePackage(project.Name, codeFiles);
            var diagnostics = package.Diagnostics;
            if (diagnostics.Any())
            {
                lock (consoleLock)
                {
                    Console.WriteLine($"Build FAILED {project.Name} ({project.Path})");
                    foreach (var diagnostic in diagnostics
                        .OrderBy(d => d.File.Reference.ToString())
                        .ThenBy(d => d.StartPosition))
                    {
                        Console.WriteLine($"{diagnostic.File.Reference}:{diagnostic.StartPosition.Line}:{diagnostic.StartPosition.Column} {diagnostic.Level} {diagnostic.ErrorCode}");
                        Console.WriteLine(diagnostic.Message);
                    }
                }
                return package;
            }
            var cCode = new CodeEmitter().Emit(package);
            var cacheDir = Path.Combine(project.Path, ".forge-cache");
            Directory.CreateDirectory(cacheDir); // Ensure the cache directory exists
            var outputPath = Path.Combine(cacheDir, "program.c");
            File.WriteAllText(outputPath, cCode, Encoding.ASCII);
            lock (consoleLock)
            {
                Console.WriteLine($"Build SUCCEEDED {project.Name} ({project.Path})");
            }
            return package;
        }

        private List<Project> TopologicalSort()
        {
            var projectAlive = projects.ToDictionary(p => p, p => SortState.Alive);
            var sorted = new List<Project>(projects.Count);
            foreach (var project in projects)
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
                    foreach (var referencedProject in project.ReferencedProjects)
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
