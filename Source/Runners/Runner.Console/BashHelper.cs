using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Runner.Console
{
    public static class BashHelper
    {
        public static async Task<(List<string> stdout, List<string> stderr)> RunCommand(string cmd, params string[] args)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo(cmd, string.Join(" ", args))
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var stdout = new List<string>();
            var stderr = new List<string>();

            await Task.Delay(TimeSpan.FromSeconds(10));
            
            if(!process.HasExited) process.Kill(true);
            
            var line = await process.StandardOutput.ReadLineAsync();
            while (line != null)
            {
                stdout.Add(line);
                line = await process.StandardOutput.ReadLineAsync();
            }

            line = await process.StandardError.ReadLineAsync();
            while (line != null)
            {
                stderr.Add(line);
                line = await process.StandardError.ReadLineAsync();
            }

            return (stdout, stderr);
        }
    }
    

}