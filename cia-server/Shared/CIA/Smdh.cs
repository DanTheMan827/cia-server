using System.Text;

namespace cia_server.Shared.CIA
{
    public class Smdh
    {
        public class SmdhTitle
        {
            public string ShortDescription { get; private set; }
            public string LongDescription { get; private set; }
            public string Publisher { get; private set; }

            public SmdhTitle(byte[] titleArray)
            {
                if (titleArray.Length != 0x200)
                {
                    throw new ArgumentException("Invalid title byte array size.");
                }

                ShortDescription = Encoding.Unicode.GetString(titleArray.Take(0x80).ToArray()).TrimEnd('\0').Trim();
                LongDescription = Encoding.Unicode.GetString(titleArray.Skip(0x80).Take(0x100).ToArray()).TrimEnd('\0').Trim();
                Publisher = Encoding.Unicode.GetString(titleArray.Skip(0x180).Take(0x80).ToArray()).TrimEnd('\0').Trim();
            }
        }
        public class SmdhTitles
        {
            public SmdhTitle Japanese { get; private set; }
            public SmdhTitle English { get; private set; }
            public SmdhTitle French { get; private set; }
            public SmdhTitle German { get; private set; }
            public SmdhTitle Italian { get; private set; }
            public SmdhTitle Spanish { get; private set; }
            public SmdhTitle SimplifiedChinese { get; private set; }
            public SmdhTitle Korean { get; private set; }
            public SmdhTitle Dutch { get; private set; }
            public SmdhTitle Portuguese { get; private set; }
            public SmdhTitle Russian { get; private set; }
            public SmdhTitle TraditionalChinese { get; private set; }

            public SmdhTitles(byte[] titlesArray)
            {
                if (titlesArray.Length != 0x2000)
                {
                    throw new ArgumentException("Invalid titles byte array size.");
                }

                using (var ms = new MemoryStream(titlesArray))
                using (var reader = new BinaryReader(ms))
                {
                    Japanese = new SmdhTitle(reader.ReadBytes(0x200));
                    English = new SmdhTitle(reader.ReadBytes(0x200));
                    French = new SmdhTitle(reader.ReadBytes(0x200));
                    German = new SmdhTitle(reader.ReadBytes(0x200));
                    Italian = new SmdhTitle(reader.ReadBytes(0x200));
                    Spanish = new SmdhTitle(reader.ReadBytes(0x200));
                    SimplifiedChinese = new SmdhTitle(reader.ReadBytes(0x200));
                    Korean = new SmdhTitle(reader.ReadBytes(0x200));
                    Dutch = new SmdhTitle(reader.ReadBytes(0x200));
                    Portuguese = new SmdhTitle(reader.ReadBytes(0x200));
                    Russian = new SmdhTitle(reader.ReadBytes(0x200));
                    TraditionalChinese = new SmdhTitle(reader.ReadBytes(0x200));
                }
            }
        }
        public class SmdhSettings
        {

        }

        public string Magic { get; private set; }
        public int Version { get; private set; }
        private long Reserved { get; set; }
        public SmdhTitles Titles { get; private set; }
        private byte[] ApplicationSettings { get; set; }
        public byte[] SmallIcon { get; private set; }
        public byte[] LargeIcon { get; private set; }
        public Smdh(byte[] smdh)
        {
            if (smdh.Length != 0x36C0)
            {
                throw new ArgumentException("Invalid smdh size");
            }

            using (var ms = new MemoryStream(smdh))
            using (var reader = new BinaryReader(ms))
            {
                Magic = Encoding.ASCII.GetString(reader.ReadBytes(4));
                Version = reader.ReadUInt16();
                Reserved = reader.ReadUInt16();
                Titles = new SmdhTitles(reader.ReadBytes(0x2000));
                ApplicationSettings = reader.ReadBytes(0x30);
                Reserved = reader.ReadInt64();
                SmallIcon = reader.ReadBytes(0x480);
                LargeIcon = reader.ReadBytes(0x1200);
            }
        }
    }
}
