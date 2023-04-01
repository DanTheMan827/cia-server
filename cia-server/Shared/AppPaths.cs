namespace cia_server.Shared
{
    public static class AppPaths
    {
        public static string WwwRoot { get; private set; }
        public static string CiaServerPath { get; private set; }
        public static string BaseUri { get; private set; }

        public static void SetEnvironment(IWebHostEnvironment env)
        {
            WwwRoot = env.WebRootPath;
            CiaServerPath = Path.Combine(WwwRoot, "cia");
        }
    }
}
