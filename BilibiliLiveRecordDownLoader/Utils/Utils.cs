﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BilibiliLiveRecordDownLoader.Utils
{
    public static class Utils
    {
        public static string CountSize(long size)
        {
            var mStrSize = string.Empty;
            const double step = 1024.00;
            var factSize = size;
            if (factSize == 0)
            {
                mStrSize = $@"{factSize:F2} Byte";
            }
            else if (factSize < step)
            {
                mStrSize = $@"{factSize:F2} Bytes";
            }
            else if (factSize >= step && factSize < 1048576)
            {
                mStrSize = $@"{factSize / step:F2} KB";
            }
            else if (factSize >= 1048576 && factSize < 1073741824)
            {
                mStrSize = $@"{factSize / step / step:F2} MB";
            }
            else if (factSize >= 1073741824 && factSize < 1099511627776)
            {
                mStrSize = $@"{factSize / step / step / step:F2} GB";
            }
            else if (factSize >= 1099511627776)
            {
                mStrSize = $@"{factSize / step / step / step / step:F2} TB";
            }
            return mStrSize;
        }

        public static (long, long) GetDiskUsage(string path)
        {
            try
            {
                var allDrives = DriveInfo.GetDrives();
                foreach (var d in allDrives)
                {
                    if (d.Name != Path.GetPathRoot(path)) continue;

                    if (d.IsReady)
                    {
                        return (d.AvailableFreeSpace, d.TotalSize);
                    }
                }
            }
            catch
            {
                // ignored
            }

            return (0, 0);
        }

        public static bool OpenUrl(string path)
        {
            try
            {
                new Process { StartInfo = new ProcessStartInfo(path) { UseShellExecute = true } }.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool OpenDir(string dir)
        {
            if (Directory.Exists(dir))
            {
                try
                {
                    return OpenUrl(dir);
                }
                catch
                {
                    // ignored
                }
            }
            return false;
        }

        public static string GetExecutablePath()
        {
            var p = Process.GetCurrentProcess();
            if (p.MainModule != null)
            {
                var res = p.MainModule.FileName;
                return res;
            }

            var dllPath = GetDllPath();
            return Path.Combine(Path.GetDirectoryName(dllPath) ?? throw new InvalidOperationException(), $@"{Path.GetFileNameWithoutExtension(dllPath)}.exe");
        }

        public static string GetDllPath()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        public static string EnsureDir(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    return Path.GetFullPath(path);
                }
            }
            catch
            {
                // ignored
            }
            return Path.GetDirectoryName(GetExecutablePath());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NoWarning(this Task _) { }
    }
}
