using System.Diagnostics;

namespace cia_server.Shared.CIA
{
    public class CiaFile
    {
        public string DisplayTitle => Titles?.English?.ShortDescription ?? System.IO.Path.GetFileName(Path);
        public string Path { get; private set; }
        public long Size { get; private set; }
        private Smdh? Smdh { get; set; }
        public Smdh.SmdhTitles? Titles => Smdh?.Titles;
        public byte[]? SmallIcon => Smdh?.SmallIcon;
        public byte[]? LargeIcon => Smdh?.LargeIcon;
        private TMD Tmd { get; set; }
        public string TitleID => Tmd.TitleID;
        public TitleType Type = CiaFile.TitleType.Unknown;

        public enum TitleType
        {
            Game, DLC, Update, DsiWare, Unknown
        }

        private static Dictionary<string, TitleType> types =
        new Dictionary<string, TitleType>() {
            { "0000", TitleType.Game },
            { "008c", TitleType.DLC },
            { "000e", TitleType.Update },
            { "8004", TitleType.DsiWare }
        };

        public CiaFile(string ciaPath)
        {
            Path = ciaPath;
            Debug.WriteLine(ciaPath);

            using (var file = new FileStream(ciaPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader reader = new BinaryReader(file))
            {
                // Read the archive header size
                var archiveHeaderSize = reader.ReadUInt32();

                // Read the type
                var type = reader.ReadInt16();

                // Read the version
                var version = reader.ReadInt16();

                // Read the certificate chain size
                var certChainSize = reader.ReadInt32();

                // Read the ticket size
                var ticketSize = reader.ReadInt32();

                // Read the TMD file size
                var tmdSize = reader.ReadInt32();

                // Read the meta size
                var metaSize = reader.ReadInt32();

                // Read the content size
                var contentSize = reader.ReadInt64();

                // Read the content index
                var contentIndex = reader.ReadBytes(0x2000);

                if (metaSize > 0)
                {
                    file.Position = HelperFunctions.AlignTo(archiveHeaderSize) + HelperFunctions.AlignTo(certChainSize) + HelperFunctions.AlignTo(ticketSize) + HelperFunctions.AlignTo(tmdSize) + HelperFunctions.AlignTo(contentSize) + 0x400;
                    var meta = new byte[0x36c0];
                    file.Read(meta, 0, meta.Length);
                    Smdh = new Smdh(meta);
                }

                if (tmdSize > 0)
                {
                    file.Position = HelperFunctions.AlignTo(archiveHeaderSize) + HelperFunctions.AlignTo(certChainSize) + HelperFunctions.AlignTo(ticketSize);
                    Tmd = new TMD(reader.ReadBytes(tmdSize));
                    var typeString = TitleID.Substring(4, 4).ToLower();
                    if (types.ContainsKey(typeString))
                    {
                        Type = types[typeString];
                    }
                }
            }
        }
    }
}
