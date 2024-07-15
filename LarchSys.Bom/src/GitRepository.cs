namespace LarchSys.Bom;

internal class GitRepository : IRepository, IDisposable {
    private readonly DirectoryInfo _repoRootDir;
    private readonly string _gitDirectoryName;
    private readonly Executor _exec;

    public GitRepository(DirectoryInfo repoRootDir)
    {
        _repoRootDir = repoRootDir;
        _gitDirectoryName = Path.Combine(_repoRootDir.FullName, ".git") + Path.DirectorySeparatorChar;
        _exec = new Executor("git", "check-ignore -z --stdin -v --non-matching", repoRootDir.FullName);
    }


    public bool IsIgnored(FileInfo file)
    {
        if (IsHiddenGitFolder(file)) {
            return true;
        }

        var relativePath = file.FullName.Replace(_repoRootDir.FullName + "\\", "");
        var unixPath = relativePath.Replace("\\", "/");

        var result = _exec.Write(unixPath + "\n\0", out var error);
        if (error != null) {
            throw new Exception(error);
        }

        var data = result.TrimStart('\0').Split('\0');

        if (data.Length == 1 && data[0] == unixPath) {
            return false;
        }

        if (data.Length == 4) {
            return true;
        }

        throw new Exception($"Something is weird got: {string.Join(",", data)}");
    }


    private bool IsHiddenGitFolder(in FileInfo file)
    {
        return file.FullName.StartsWith(_gitDirectoryName);
    }


    public void Dispose()
    {
        _exec.Dispose();
    }
}
