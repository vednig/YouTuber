using System.Diagnostics;

namespace YouTuber.Helper
{
    public static class Helpers
    {
        /// <summary>
        /// Get build version number
        /// </summary>
        /// <returns></returns>
        public static string GetVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = versionInfo.FileVersion;
            return version;
        }
    }
}
