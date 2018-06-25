namespace UoN.ZipBuilder
{
    public enum Encryption
    {
        /// <summary>
        /// No encryption
        /// </summary>
        None = 0,

        /// <summary>
        /// PKZIP 2.0 "Classic" encryption.
        /// 
        /// This is the original PKWare zip encryption spec,
        /// it's very weak and is NOT RECOMMENDED,
        /// but is supported by some older zip clients that don't support newer encryption,
        /// such as Windows Compressed Folders.
        /// </summary>
        Classic = 1,

        /// <summary>
        /// WinZip AES encryption using a 128-bit key
        /// </summary>
        Aes128 = 128,

        /// <summary>
        /// WinZip AES encryption using a 192-bit key
        /// </summary>
        Aes192 = 192,

        /// <summary>
        /// WinZip AES encryption using a 256-bit key
        /// </summary>
        Aes256 = 256,

        /// <summary>
        /// WinZip AES encryption using the default key size (256-bit)
        /// </summary>
        Aes = 256
    }
}
