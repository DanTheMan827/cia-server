using System.Text;

namespace cia_server.Shared
{
    public static class HelperFunctions
    {
        public static string ByteArrayToHexString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                _ = hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            int NumberChars = hex.Length;

            if (NumberChars % 2 == 1)
            {
                throw new FormatException("String length is not an even number.");
            }

            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }
        public static string GetUrl(string path, string requestUrl)
        {
            var wwwrootPath = AppPaths.WwwRoot;
            var relativePath = Path.GetRelativePath(wwwrootPath, path);
            var baseUrl = requestUrl == null ? "" : requestUrl;
            var uri = new Uri(baseUrl);
            var url = new Uri(uri, new Uri($"{uri.Scheme}://{uri.Authority}/{relativePath}")).ToString().Replace(" ", "%20");
            return url;
        }

        public static long AlignTo(long offset, long alignment = 64)
        {
            return (long)Math.Ceiling((double)offset / alignment) * alignment;
        }
    }
}
