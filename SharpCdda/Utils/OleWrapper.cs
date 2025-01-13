using IMAPI2;
using System;
using System.Runtime.InteropServices;

namespace SharpCdda.Utils
{
    internal static class OleWrapper
    {
        [DllImport("ole32.dll")]
        public static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out IStream ppstm);
    }
}
