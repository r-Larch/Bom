using System.IO;


namespace LarchSys.Bom {
    internal class NoopRepository : IRepository {
        public bool IsIgnored(FileInfo file)
        {
            return false;
        }
    }
}
