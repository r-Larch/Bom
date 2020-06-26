using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;


namespace LarchSys.Bom {
    public class Executor : IDisposable {
        private StreamWriter? _input;
        private Process? _process;
        private readonly Thread _thread;


        public static string Exec(string exe, string args, DirectoryInfo workingDirectory)
        {
            var process = new Process {
                StartInfo = new ProcessStartInfo(exe, args) {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDirectory.FullName
                },
            };

            var std = new StringBuilder();
            process.OutputDataReceived += (sender, eventArgs) => {
                std.Append(eventArgs.Data);
            };

            //var error = new StringBuilder();
            //process.ErrorDataReceived += (sender, eventArgs) => {
            //    error.AppendLine(eventArgs.Data);
            //};

            if (process.Start()) {
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                //var log = error.ToString().Trim(' ', '\r', '\n', '\t');
                //if (!string.IsNullOrEmpty(log)) {
                //    //_log.LogError(log);
                //}
            }

            if (process.ExitCode == 0) {
                return std.ToString().Trim(' ', '\r', '\n', '\t');
            }

            return string.Empty;
        }


        public Executor(string command, string args, string workingDirectory)
        {
            var started = new AutoResetEvent(false);
            _thread = new Thread(() => Start(command, args, workingDirectory, started));
            _thread.Start();
            started.WaitOne();
        }


        private void Start(string command, string args, string workingDirectory, EventWaitHandle started)
        {
            _process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = command,
                    Arguments = args,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            // start
            _process.Start();
            // start read
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            // write
            _input = _process.StandardInput;

            started.Set();
            _process.WaitForExit();
        }


        public string Write(string input, out string? error)
        {
            error = null;

            if (_process == null || _input == null) {
                throw new Exception($"You must call Start() first!");
            }

            var wait = new AutoResetEvent(false);
            var result = string.Empty;
            var stdErr = string.Empty;
            try {
                _process.OutputDataReceived += Handler;
                _process.ErrorDataReceived += Error;
                _input.Write(input);
                _input.Flush();

                wait.WaitOne();

                error = string.IsNullOrEmpty(stdErr) ? null : stdErr;

                return result;
            }
            finally {
                _process.OutputDataReceived -= Handler;
                _process.ErrorDataReceived -= Error;
            }

            void Handler(object sender, DataReceivedEventArgs args)
            {
                result += args.Data;
                wait.Set();
            }

            void Error(object sender, DataReceivedEventArgs args)
            {
                stdErr += args.Data;
                wait.Set();
            }
        }


        public void Dispose()
        {
            _input?.Dispose();
            _process?.WaitForExit((int) new TimeSpan(0, 0, 5).TotalMilliseconds);
            _process?.Close();
            _thread.Join(10);
        }
    }
}
