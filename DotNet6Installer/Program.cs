using Lyricify_for_Spotify.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DotNet6Installer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (!args.Contains("-f"))
            {
                if (!SingleInstance.IsMainInstance)
                {
                    Console.WriteLine("Found existing instance, exiting");
                    return;
                }
            }

            Architecture? architecture = null;
            if (args.Contains("x86"))
            {
                architecture = Architecture.X86;
            }
            else if (args.Contains ("x64"))
            {
                architecture = Architecture.X64;
            }
            else if (args.Contains("arm64"))
            {
                architecture = Architecture.Arm64;
            }
            architecture ??= ArchitectureHelper.OSArchitecture;

            if (!args.Contains("-i"))
            {
                // Check currently installed version
                if (DetectHelper.IsRuntimeInstalled(architecture.Value) != false)
                {
                    Console.WriteLine("Installed or not supported by the OS");
                    return;
                }
            }

            string filePath = DownloadDotNet6(architecture.Value).Result;

            //int messageIndex = Array.IndexOf(args, "-m");
            //if (messageIndex!=-1 && messageIndex < args.Length - 1)
            //{
            //    if (!args[messageIndex + 1].StartsWith("-"))
            //    {
            //        MessageBox.Show(args[messageIndex + 1]);
            //    }
            //}

            InstallDotNet(filePath);
        }

        public static async Task<string> DownloadDotNet6(Architecture architecture)
        {
            string url = architecture switch
            {
                Architecture.X86 => "https://download.visualstudio.microsoft.com/download/pr/ea0e40d2-e326-453b-8cac-2719cbbefeca/b26458b139a500d3067ec25987030497/windowsdesktop-runtime-6.0.16-win-x86.exe",
                Architecture.X64 => "https://download.visualstudio.microsoft.com/download/pr/85473c45-8d91-48cb-ab41-86ec7abc1000/83cd0c82f0cde9a566bae4245ea5a65b/windowsdesktop-runtime-6.0.16-win-x64.exe",
                Architecture.Arm64 => "https://download.visualstudio.microsoft.com/download/pr/7a490e4e-5a43-4e3e-8311-028e1a5436cb/d2b0bd46d8202676bb8c9f4c97f8ec58/windowsdesktop-runtime-6.0.16-win-arm64.exe",
                _ => throw new NotSupportedException()
            };
            string filePath = await DownloadFromUrl(url);
            return filePath;
        }

        public static void InstallDotNet(string filePath)
        {
            using Process process = new();
            process.StartInfo = new(filePath, "/install /quiet /norestart")
            {
                CreateNoWindow = true
            };
            process.Start();
        }

        public static async Task<string> DownloadFromUrl(string url)
        {
            string tempPath = Path.GetTempPath();
            string fileName = Path.GetFileName(url);

            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync(url);
            using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
            string fileToWriteTo = tempPath + fileName;
            using Stream streamToWriteTo = File.Open(fileToWriteTo, FileMode.Create);
            await streamToReadFrom.CopyToAsync(streamToWriteTo);

            return fileToWriteTo;
        }
    }
}