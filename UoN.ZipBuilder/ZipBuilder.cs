using System;
using System.IO;
using System.IO.Compression;

namespace UoN.ZipBuilder
{
    public class ZipBuilder : IZipBuilder
    {
        private MemoryStream ZipStream { get; set; }
        private ZipArchive Zip { get; set; }

        public ZipBuilder CreateZipStream()
        {
            if (ZipStream != null) throw new InvalidOperationException(
                "This builder already has a MemoryStream");
            if (Zip != null) throw new InvalidOperationException(
                "This builder already has a ZipArchive");

            ZipStream = new MemoryStream();

            Zip = new ZipArchive(ZipStream, ZipArchiveMode.Create, true);

            return this;
        }

        public ZipBuilder AddFile(string sourcePath, string entryName)
        {
            Zip.CreateEntryFromFile(sourcePath, entryName);
            return this;
        }

        public ZipBuilder AddDirectoryShallow(string path, string entryName)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException("Directory Path is invalid", nameof(path));

            foreach (var f in Directory.EnumerateFiles(path))
                Zip.CreateEntryFromFile(f, Path.Combine(entryName, Path.GetFileName(f)));

            return this;
        }

        public ZipBuilder AddTextContent(string content, string entryName)
        {
            var entry = Zip.CreateEntry(entryName);

            using (var entryStream = entry.Open())
            using (var streamWriter = new StreamWriter(entryStream))
                streamWriter.Write(content);

            return this;
        }

        public byte[] AsByteArray()
        {
            Zip.Dispose();
            var result = ZipStream.ToArray();
            ZipStream.Dispose();
            return result;
        }
    }
}