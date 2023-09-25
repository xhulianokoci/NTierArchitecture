using System.Text;

namespace Cryptography;

public interface ICryptoUtils
{
    /// <summary>
    /// Return MD5 hash of a given string
    /// </summary>
    /// <param name="input">input string to hash</param>
    /// <param name="encoding">used encoding to get bytes array from inputString (default ASCII)</param>
    /// <returns></returns>
    string GetMd5Hash(string input, Encoding? encoding = null);

    /// <summary>
    /// Encrypt a given string using AES symmetric algorithm
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    string Encrypt(string plainText, string? key = null, string? iv = null);

    /// <summary>
    /// Decrypt a given cipher string using AES symmetric algorithm
    /// </summary>
    /// <param name="cipherText"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    string Decrypt(string cipherText, string? key = null, string? iv = null);

    /// <summary>
    /// Return the SHA256 bit encoded string calculated from input string
    /// </summary>
    /// <param name="inputString">string to hash</param>
    /// <param name="encoding">used encoding to get bytes array from inputString (default UTF8)</param>
    /// <returns></returns>
    string GetSha256String(string inputString, Encoding? encoding = null);

    /// <summary>
    /// Generate new random AES symmetric algorithm Key and IV,
    /// 256 bit - CBC mode
    /// </summary>
    /// <returns></returns>
    (string key, string iv) GenerateAesKey();
}
