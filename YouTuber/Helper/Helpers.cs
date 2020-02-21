using System.Diagnostics;
using System.Runtime.CompilerServices;

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

        /// <summary>
        /// Get current method name
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }
    }
}
