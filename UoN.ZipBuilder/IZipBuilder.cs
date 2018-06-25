namespace UoN.ZipBuilder
{
    /// <summary>
    /// Interface for a Builder Pattern class which produces zip files.
    /// </summary>
    public interface IZipBuilder
    {
        #region Init

        /// <summary>
        /// Initialises a stream to use for the underlying zip data
        /// this Zipbuilder instance will manipulate.
        /// </summary>
        /// <returns>The ZipBuilder instance.</returns>
        ZipBuilder CreateZipStream();

        #endregion

        #region Config

        /// <summary>
        /// Implementations of this class default to using Zip64.
        /// 
        /// If you need pre-NT6 (Vista) compressed folders or other older code support,
        /// You should use this directive, as they don't support Zip64.
        /// 
        /// This will disallow files larger than 4GB.
        /// </summary>
        /// <returns>The ZipBuilder instance.</returns>
        ZipBuilder DisableZip64();

        /// <summary>
        /// Add password encryption to all files in the zip file.
        /// 
        /// While zip supports encryption on a per-entry basis,
        /// this builder currently assumes encryption of all files in the archive.
        /// 
        /// Therefore, this setting must be applied before any entries, and can only be done once.
        /// </summary>
        /// <param name="password">The password to use for decrypting the zip.</param>
        /// <param name="method">The encryption method to use. Defaults to AES with a default key size.</param>
        /// <returns></returns>
        ZipBuilder UseEncryption(string password, Encryption method = Encryption.Aes);

        #endregion

        #region Content

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

        #endregion

        #region Output

        /// <summary>
        /// Returns the zip represented by the ZipBuilder state as a byte array,
        /// for use elsewhere.
        /// </summary>
        /// <returns>A byte array representing the zip.</returns>
        byte[] AsByteArray();

        #region
    }
}