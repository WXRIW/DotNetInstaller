namespace Lyricify_for_Spotify.Helpers
{
    public static class SingleInstance
    {
        private static Mutex? mutex;
        private static bool? isMainInstance;
        private static object locker = new object();

        public static bool IsMainInstance
        {
            get
            {
                if (!isMainInstance.HasValue)
                {
                    lock (locker)
                    {
                        if (!isMainInstance.HasValue)
                        {
                            mutex = new Mutex(true, "DotNetInstaller", out var createdNew);
                            isMainInstance = createdNew;
                            if (!createdNew)
                            {
                                mutex.Dispose();
                                mutex = null;
                            }
                        }
                    }
                }

                return isMainInstance.Value;
            }
        }

        internal static void TryReleaseMutex()
        {
            mutex?.Dispose();
            mutex = null;

            isMainInstance = false;
        }
    }
}