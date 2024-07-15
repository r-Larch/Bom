namespace LarchSys.Bom;

internal interface IRepository {
    bool IsIgnored(FileInfo file);
}

internal class RepositoryFactory : IDisposable {
    private static readonly IRepository Noop = new NoopRepository();

    // cache instances for each directory because there could be submodules!
    public readonly Dictionary<string, IRepository> Instances = [];
    public readonly Dictionary<string, string> DirectoryRootMap = [];

    public static string GitFolder = $"{Path.DirectorySeparatorChar}.git{Path.DirectorySeparatorChar}";


    public IRepository GetRepository(FileInfo file)
    {
        if (file.DirectoryName == null) {
            return Noop;
        }

        if (!DirectoryRootMap.TryGetValue(file.DirectoryName, out var root)) {
            root = GetRootDirectory(file);
            DirectoryRootMap.Add(file.DirectoryName, root);
        }

        if (!Instances.TryGetValue(root, out var git)) {
            git = CreateGitRepository(root);
            Instances.Add(root, git);
        }

        return git;
    }


    private static string GetRootDirectory(in FileInfo file)
    {
        // handle: /.git/
        var index = file.FullName.IndexOf(GitFolder, StringComparison.InvariantCulture);
        if (index != -1) {
            return file.FullName[..index];
        }

        var path = Executor.Exec("git", "rev-parse --show-toplevel", file.Directory!);

        var normalized = string.IsNullOrEmpty(path)
            ? string.Empty
            : new DirectoryInfo(path).FullName;

        return normalized;
    }


    private static IRepository CreateGitRepository(in string root)
    {
        if (string.IsNullOrEmpty(root)) {
            return Noop;
        }

        return new GitRepository(new DirectoryInfo(root));
    }


    public void Dispose()
    {
        DirectoryRootMap.Clear();

        foreach (var repo in Instances.Values) {
            if (repo is IDisposable disposable) {
                disposable.Dispose();
            }
        }

        Instances.Clear();
    }
}
