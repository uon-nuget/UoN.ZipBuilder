namespace UoN.ZipBuilder
{
    /// <summary>
    /// Interface for a Builder Pattern class which produces zip files.
    /// </summary>
    public interface IZipBuilder
    {
        /// <summary>
        /// Adds the top level contents of a directory (no resursion) to the zip,
        /// preserving the filenames of the files inside, and adding them to provided entryname.
        /// </summary>
        /// <param name="path">Path to the directory containing files to add.</param>
        /// <param name="entryName">Entryname (can be a path) for the directory inside the zip.</param>
        /// <returns>The ZipBuilder instance.</returns>
        ZipBuilder AddDirectoryShallow(string path, string entryName);

        /// <summary>
        /// Adds a single file from disk to the zip.
        /// </summary>
        /// <param name="sourcePath">Path to the file to add.</param>
        /// <param name="entryName">Entryname for the file inside the zip.</param>
        /// <returns>The ZipBuilder instance.</returns>
        ZipBuilder AddFile(string sourcePath, string entryName);

        /// <summary>
        /// Writes a string to an entry inside the zip.
        /// </summary>
        /// <param name="content">The string content to write.</param>
        /// <param name="entryName">The entryname for the resulting text file inside the zip.</param>
        /// <returns>The ZipBuilder instance.</returns>
        ZipBuilder AddTextContent(string content, string entryName);

        /// <summary>
        /// Returns the zip represented by the ZipBuilder state as a byte array,
        /// for use elsewhere.
        /// </summary>
        /// <returns>A byte array representing the zip.</returns>
        byte[] AsByteArray();

        /// <summary>
        /// Initialises a stream to use for the underlying zip data
        /// this Zipbuilder instance will manipulate.
        /// </summary>
        /// <returns>The ZipBuilder instance.</returns>
        ZipBuilder CreateZipStream();
    }
}