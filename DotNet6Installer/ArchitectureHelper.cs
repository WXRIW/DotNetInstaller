using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Lyricify_for_Spotify.Helpers
{
    public class ArchitectureHelper
    {
        private static Architecture? osArchitecture = null;

        public static Architecture OSArchitecture
        {
            get
            {
                osArchitecture ??= GetOSArchitecture();
                return osArchitecture.Value;
            }
        }

        private static Architecture GetOSArchitecture()
        {
            switch (RuntimeInformation.OSArchitecture)
            {
                case Architecture.X86:
                    return IsOSArchitectureArm() ? Architecture.Arm64 : Architecture.X86;
                case Architecture.X64:
                    return IsOSArchitectureArm() ? Architecture.Arm64 : Architecture.X64;
                default:
                    return RuntimeInformation.OSArchitecture;
            }
        }

        private static bool IsOSArchitectureArm()
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 16299))
            {
                return ArmHelper.IsRunningOnArm;
            }
            return false;
        }
    }

    // https://github.com/files-community/Files/blob/bf2ee008822cb93888813875b17d1ca81070dd0f/src/Files.App/Helpers/NativeWinApiHelper.cs#L215
    [SupportedOSPlatform("Windows10.0.16299.0")]
    public class ArmHelper
    {
        // https://learn.microsoft.com/windows/win32/api/wow64apiset/nf-wow64apiset-iswow64process2
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool IsWow64Process2(
            IntPtr process,
            out ushort processMachine,
            out ushort nativeMachine
        );

        // https://stackoverflow.com/questions/54456140/how-to-detect-were-running-under-the-arm64-version-of-windows-10-in-net
        // https://learn.microsoft.com/windows/win32/sysinfo/image-file-machine-constants
        private static bool? isRunningOnArm = null;

        public static bool IsRunningOnArm
        {
            get
            {
                isRunningOnArm ??= IsArmProcessor();
                return isRunningOnArm.Value;
            }
        }

        private static bool IsArmProcessor()
        {
            var handle = System.Diagnostics.Process.GetCurrentProcess().Handle;
            if (!IsWow64Process2(handle, out _, out var nativeMachine))
            {
                return false;
            }
            return (nativeMachine == 0xaa64 ||
                    nativeMachine == 0x01c0 ||
                    nativeMachine == 0x01c2 ||
                    nativeMachine == 0x01c4);
        }
    }
}
