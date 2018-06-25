using System;
using System.IO;
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

            ZipStream = new ZipOutputStream(DataStream); // new ZipArchive(ZipStream, ZipArchiveMode.Create, true);

            return this;
        }

        public ZipBuilder AddFile(string sourcePath, string entryName)
        {
            var entry = new ZipEntry(entryName);
            entry.DateTime = DateTime.Now;

            ZipStream.PutNextEntry(entry);

            StreamUtils.Copy(
                new FileStream(sourcePath, FileMode.Open, FileAccess.Read),
                ZipStream, new byte[4096]);

            ZipStream.CloseEntry();

            //Zip.CreateEntryFromFile(sourcePath, entryName);
            return this;
        }

        public ZipBuilder AddDirectoryShallow(string path, string entryName)
        {
            if (!Directory.Exists(path))
                throw new ArgumentException("Directory Path is invalid", nameof(path));

            foreach (var f in Directory.EnumerateFiles(path))
                AddFile(f, Path.Combine(entryName, Path.GetFileName(f)));
                //Zip.CreateEntryFromFile(f, Path.Combine(entryName, Path.GetFileName(f)));

            return this;
        }

        public ZipBuilder AddTextContent(string content, string entryName)
        {
            var entry = new ZipEntry(entryName);
            entry.DateTime = DateTime.Now;

            ZipStream.PutNextEntry(entry);

            //var entry = Zip.CreateEntry(entryName);

            //using (var entryStream = entry.Open())
            using (var streamWriter = new StreamWriter(ZipStream))
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

            //Zip.Dispose();
            var result = DataStream.ToArray();
            ZipStream.Dispose();
            return result;
        }
    }
}