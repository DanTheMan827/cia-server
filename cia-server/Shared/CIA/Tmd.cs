﻿namespace cia_server.Shared.CIA
{
    public struct TMDContent
    {
        /// <summary>
        /// The ID in hexadecimal form.
        /// </summary>
        public readonly byte[] ID;

        /// <summary>
        /// The ID in hexadecimal string form.
        /// </summary>
        public readonly string IDString;

        /// <summary>
        /// The content type.
        /// </summary>
        public readonly ushort Type;

        /// <summary>
        /// The size of the app.
        /// </summary>
        public readonly uint Size;

        /// <summary>
        /// If there is a corresponding .h3 file to the app.
        /// </summary>
        public readonly bool HasH3;

        /// <summary>
        /// Hash of the decrypted contents.
        /// </summary>
        public readonly byte[] Sha256Hash;

        public TMDContent(byte[] contentBytes)
        {
            if (contentBytes.Length != 0x30)
            {
                throw new ArgumentOutOfRangeException("contentBytes", contentBytes, "Expected 48 bytes, received " + contentBytes.Length);
            }

            this.ID = contentBytes.Take(0x04).ToArray();
            this.IDString = HelperFunctions.ByteArrayToHexString(this.ID);
            this.Type = BitConverter.ToUInt16(contentBytes.Skip(0x06).Take(2).Reverse().ToArray(), 0);
            this.Size = BitConverter.ToUInt32(contentBytes.Skip(0x08).Take(8).Reverse().ToArray(), 0);
            this.HasH3 = (this.Type & 0x2) > 0;
            this.Sha256Hash = contentBytes.Skip(0x10).Take(20).ToArray();
        }
    }
    public struct TMD
    {
        private const short tk = 0x140;
        private const short contentStart = 0xB04;
        private const byte contentSize = 0x30;

        public readonly TMDContent[] Content;
        public readonly ushort TitleVersion;
        public readonly long TitleContentSize;
        public readonly string TitleID;
        public readonly byte[] rawBytes;

        public TMD(byte[] tmdBytes)
        {
            this.TitleContentSize = 0;
            short contentCount = BitConverter.ToInt16(tmdBytes.Skip(tk + 0x9E).Take(0x02).Reverse().ToArray(), 0);
            var content = new List<TMDContent> { };
            for (int i = 0; i < contentCount; i++)
            {
                var tmdContent = new TMDContent(tmdBytes.Skip(contentStart + (i * contentSize)).Take(contentSize).ToArray());
                content.Add(tmdContent);
                this.TitleContentSize += tmdContent.Size;
            }

            this.rawBytes = tmdBytes;
            this.TitleVersion = BitConverter.ToUInt16(tmdBytes.Skip(tk + 0x9C).Take(0x02).Reverse().ToArray(), 0);
            this.TitleID = HelperFunctions.ByteArrayToHexString(tmdBytes.Skip(0x18C).Take(0x08).ToArray());
            this.Content = content.ToArray();
        }
        public TMD(bool o)
        {
            this.Content = new TMDContent[] { };
            this.TitleVersion = 0;
            this.TitleContentSize = 0;
            this.TitleID = "";
            this.rawBytes = new byte[] { };
        }
    }
}
