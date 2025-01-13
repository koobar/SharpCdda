using System;

namespace SharpCdda
{
    public static class Engine
    {
        // Private fields.
        private static string imapiClientName;
        private static bool isStarted;

        internal static string GetIMAPIClientName()
        {
            return imapiClientName;
        }

        internal static void ThrowExceptionIfNotStarted()
        {
            if (!isStarted)
            {
                throw new Exception("The library has not been initialized. Please call the Engine.Startup method first.");
            }
        }

        /// <summary>
        /// Starts the library. Must be called before using the library.<br/>
        /// NOTE: Normally, it is recommended to call this function as soon as the application is launched.
        /// </summary>
        /// <param name="clientName"></param>
        public static void Startup(string clientName = null)
        {
            if (isStarted)
            {
                return;
            }

            if (string.IsNullOrEmpty(clientName))
            {
                clientName = "NTrackWriter";
            }

            imapiClientName = clientName;
            isStarted = true;
        }

        /// <summary>
        /// Shutdown the library. Call this when you are finished using the library.<br/>
        /// NOTE: It is usually recommended to call this at the end of your application.
        /// </summary>
        public static void Shutdown()
        {
            if (!isStarted)
            {
                return;
            }
        }
    }
}
