using System;
using System.IO;
using System.Linq;
using System.Text;


#nullable enable

namespace LarchSys.Bom {
    public class Program {
        public static void Main(string[] args)
        {
            if (ParseBool("-h", "--help", args)) {
                Help();
            }
            else {
                try {
                    new Program().Run(args);
                }
                catch (Exception e) {
                    Std.WriteLine($"[Fatal] -> {e.Message}", e);
                }
            }

            //Std.GreenLine("  -- press any key to exit -- ");
            //Console.ReadLine();
        }


        private void Run(string[] args)
        {
            var value = ParseValueArg(args) ?? string.Empty;
            var path = Path.GetFullPath(value, Directory.GetCurrentDirectory());

            if (Directory.Exists(path)) {
                path = Path.Combine(path, "*");
            }

            var filePattern = new FileInfo(path);
            var customPattern = !string.IsNullOrEmpty(filePattern.Name) && (filePattern.Name.Contains("*") || filePattern.Exists);

            var pattern = customPattern ? filePattern.Name : "*";
            var dir = filePattern.Directory;

            var recursive = ParseBool("-r", "--recurse", args);
            var showAll = ParseBool("-a", "--all", args);
            var showWithoutBom = ParseBool("-n", "--no-bom", args) || showAll;
            var showWithBom = ParseBool("-b", "--bom", args) || showAll;
            var convert = ParseBool("-c", "--convert", args);
            var addBom = ParseBool("-d", "--add-bom", args);
            var git = ParseBool("-g", "--git", args);

            if (!showWithBom && !showWithoutBom) {
                showWithoutBom = true;
            }

            var files = dir!.GetFiles(pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            using var factory = new RepositoryFactory();

            foreach (var file in files) {
                try {
                    if (git) {
                        var repo = factory.GetRepository(file);
                        if (repo.IsIgnored(file)) {
                            continue;
                        }
                    }

                    if (FileHasBom(file)) {
                        if (showWithBom) {
                            Std.CyanLine($"BOM:         {file.FullName}");
                        }
                    }
                    else if (showWithoutBom) {
                        Std.MagentaLine($"missing BOM: {file.FullName}");

                        if (convert) {
                            ConvertFile(file, addBom: addBom);
                        }
                    }
                }
                catch (Exception e) {
                    Std.WriteLine(e);
                }
            }
        }


        public bool FileHasBom(in FileInfo file)
        {
            var preamble = new UTF8Encoding(true).GetPreamble();

            var buffer = new byte[preamble.Length];
            using (var fs = file.OpenRead()) {
                fs.Read(buffer);
            }

            if (preamble.Where((p, i) => p != buffer[i]).Any()) {
                return false;
            }

            return true;
        }

        private static void ConvertFile(in FileInfo file, in bool addBom, Encoding? sourceEncoding = null)
        {
            sourceEncoding ??= Encoding.Default;

            var utf8 = addBom ? new UTF8Encoding(true) : new UTF8Encoding(false);

            File.WriteAllText(file.FullName + ".tmp", File.ReadAllText(file.FullName, sourceEncoding), utf8);
            File.Delete(file.FullName);
            File.Move(file.FullName + ".tmp", file.FullName);
        }


        private static void Help()
        {
            Std.WriteLine();
            Std.DarkGrayLine(" @author: René Larch");
            Std.WriteLine();
            Std.WriteLine(" usage:  bom [ <directory> OPTIONS ]");
            Std.WriteLine("  <directory>     if not specified current the working directory is used");
            Std.WriteLine("  -r  --recurse   recursive list files");
            Std.WriteLine("  -a  --all       show all files");
            Std.WriteLine("  -n  --no-bom    show files without BOM");
            Std.WriteLine("  -b  --bom       show files with BOM");
            Std.WriteLine("  -c  --convert   convert files without BOM to uft-8");
            Std.WriteLine("  -d  --add-bom   add a BOM when converting");
            Std.WriteLine("  -g  --git       use .gitignore to filter files");
            Std.WriteLine();
            Std.WriteLine(" > bom -r -c -d *.cs");
        }


        private static string? ParseValueArg(in string[] args)
        {
            return args.FirstOrDefault(_ => !_.StartsWith("-"));
        }

        private static bool ParseBool(in string shortName, in string longName, in string[] args)
        {
            return args.Contains(shortName) || args.Contains(longName);
        }
    }
}
