using Lyricify_for_Spotify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNet6Installer
{
    internal class DetectHelper
    {
        /// <summary>
        /// Detect if the selected architecture of .NET 6 Desktop Runtime is installed
        /// </summary>
        /// <param name="architecture">Architecture to check</param>
        /// <returns><see langword="True"/> if installed, <see langword="false"/> if not installed, <see langword="null"/> if architecture not supported by the OS.</returns>
        /// <exception cref="NotSupportedException">Unsupported architecture</exception>
        public static bool? IsRuntimeInstalled(Architecture architecture)
        {
            return ArchitectureHelper.OSArchitecture switch
            {
                Architecture.X86 => IsRuntimeInstalled_x86(architecture),
                Architecture.X64 => IsRuntimeInstalled_x64(architecture),
                Architecture.Arm64 => IsRuntimeInstalled_Arm64(architecture),
                _ => throw new NotSupportedException()
            };
        }

        private static bool? IsRuntimeInstalled_x86(Architecture architecture)
        {
            if (architecture != Architecture.X86) return null;

            string systemDrive = Environment.GetEnvironmentVariable("SystemDrive") ?? "C:";
            string programFiles = systemDrive + @"\Program Files";
            string desktopRuntimePath = programFiles + @"\dotnet\shared\Microsoft.WindowsDesktop.App";
            return IsRuntimeFoundInProgramFiles(desktopRuntimePath);
        }

        private static bool? IsRuntimeInstalled_x64(Architecture architecture)
        {
            if (architecture != Architecture.X86 && architecture != Architecture.X64) return null;

            string systemDrive = Environment.GetEnvironmentVariable("SystemDrive") ?? "C:";
            string programFiles = systemDrive + (architecture == Architecture.X64 ? @"\Program Files" : @"\Program Files (x86)");
            string desktopRuntimePath = programFiles + @"\dotnet\shared\Microsoft.WindowsDesktop.App";
            return IsRuntimeFoundInProgramFiles(desktopRuntimePath);
        }

        private static bool? IsRuntimeInstalled_Arm64(Architecture architecture)
        {
            if (architecture != Architecture.X86 && architecture != Architecture.X64 && architecture != Architecture.Arm64) return null;

            string systemDrive = Environment.GetEnvironmentVariable("SystemDrive") ?? "C:";
            string desktopRuntimePath = systemDrive;
            switch (architecture)
            {
                case Architecture.Arm64:
                    desktopRuntimePath += @"\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App";
                    break;
                case Architecture.X64:
                    desktopRuntimePath += @"\Program Files\dotnet\x64\shared\Microsoft.WindowsDesktop.App";
                    break;
                case Architecture.X86:
                    desktopRuntimePath += @"\Program Files (x86)\dotnet\shared\Microsoft.WindowsDesktop.App";
                    break;
            }
            return IsRuntimeFoundInProgramFiles(desktopRuntimePath);
        }

        private static bool IsRuntimeFoundInProgramFiles(string desktopRuntimePath)
        {
            if (Directory.Exists(desktopRuntimePath))
            {
                var folders = Directory.GetDirectories(desktopRuntimePath);
                if (folders == null || folders.Length == 0)
                {
                    return false;
                }
                bool flag = false;
                foreach (var folder in folders)
                {
                    if (folder.StartsWith(desktopRuntimePath + @"\6.0"))
                    {
                        flag = true;
                        break;
                    }
                }
                return flag;
            }
            else
            {
                return false;
            }
        }
    }
}
