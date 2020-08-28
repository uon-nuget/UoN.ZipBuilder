using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace UoN.ZipBuilder
{
    public class ZipBuilder : IZipBuilder
    {
        /// <summary>
        /// The underlying memorystream used as a data store for the zip.
        /// </summary>
        private MemoryStream DataStream { get; set; }

        /// <summary>
        /// SharpZipLib zip stream which DEFLATEs on the way into the underlying stream.
        /// </summary>
        private ZipOutputStream ZipStream { get; set; }

        /// <summary>
        /// The encryption method to use for all files in the zip.
        /// </summary>
        private Encryption EncryptionMethod { get; set; } = Encryption.None;

        /// <summary>
        /// Initialise a zip archive and an underlying memorystream for it to work with.
        /// This implementation always uses a memory stream at this time.
        /// </summary>
        /// <returns>The ZipBuilder instance.</returns>
        public ZipBuilder CreateZipStream()
        {
            if (DataStream != null) throw new InvalidOperationException(
                "This builder already has a MemoryStream");
            if (ZipStream != null) throw new InvalidOperationException(
                "This builder already has a ZipStream");

            DataStream = new MemoryStream();

            ZipStream = new ZipOutputStream(DataStream);

            return this;
        }

        public ZipBuilder AddBytes(byte[] bytes, string entryName)
        {
            ZipStream.PutNextEntry(CreateEntry(entryName));

            ZipStream.Write(bytes, 0, bytes.Length);

            ZipStream.CloseEntry();
            return this;
        }

        public ZipBuilder AddFile(string sourcePath, string entryName)
        {
            ZipStream.PutNextEntry(CreateEntry(entryName));

            StreamUtils.Copy(
                new FileStream(sourcePath, FileMode.Open, FileAccess.Read),
                ZipStream, new byte[4096]);

            ZipStream.CloseEntry();
            return this;
        }

        public ZipBuilder AddDirectoryShallow(string path, string entryName)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException("Directory Path is invalid", nameof(path));

            foreach (var f in Directory.EnumerateFiles(path))
                AddFile(f, Path.Combine(entryName, Path.GetFileName(f)));

            return this;
        }

        public ZipBuilder AddTextContent(string content, string entryName)
        {
            ZipStream.PutNextEntry(CreateEntry(entryName));

            // write directly to the ZipStream, but be sure to leave it open!
            using (var streamWriter = new StreamWriter(ZipStream, Encoding.UTF8, 4096, leaveOpen: true))
                streamWriter.Write(content);

            ZipStream.CloseEntry();

            return this;
        }

        public byte[] AsByteArray()
        {
            // we're gonna close out the zip stream,
            // but we want the underlying memory stream open so we can get its bytes!
            ZipStream.IsStreamOwner = false; // leave the memory stream open
            ZipStream.Finish();

            var result = DataStream.ToArray();
            ZipStream.Dispose();
            return result;
        }

        public ZipBuilder DisableZip64()
        {
            ZipStream.UseZip64 = UseZip64.Off;

            return this;
        }

        public ZipBuilder UseEncryption(string password, Encryption method = Encryption.Aes)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("A password must be specified", nameof(method));

            EncryptionMethod = method;

            if (method != Encryption.None) ZipStream.Password = password;

            return this;
        }

        /// <summary>
        /// Stateful helper for creating entries,
        /// ready to be added to the ZipStream and have data written to them.
        /// </summary>
        /// <param name="entryName">The entryname.</param>
        /// <returns>A new ZipEntry.</returns>
        private ZipEntry CreateEntry(string entryName)
        {
            var entry = new ZipEntry(entryName);
            entry.DateTime = DateTime.Now;

            EncryptEntry(ref entry);

            return entry;
        }

        /// <summary>
        /// Stateful helper for encrypting zip entries based on the Builder's configured method.
        /// </summary>
        /// <param name="entry">The zip entry to encrypt.</param>
        private void EncryptEntry(ref ZipEntry entry)
        {
            try
            {
                entry.AESKeySize = (int)EncryptionMethod;
            }
            catch (ZipException)
            {
                if (EncryptionMethod == Encryption.Aes192)
                    throw new InvalidOperationException(
                        "SharpZipLib doesn't support encrypting with AES at 192 bits. Please use a different key strength.");

                // otherwise no AES Encryption
                entry.AESKeySize = 0;

                // SharpZipLib figures out the rest (Classic vs None) by itself.
            }
        }
    }
}
