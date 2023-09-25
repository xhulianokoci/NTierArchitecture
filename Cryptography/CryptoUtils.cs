using System.Security.Cryptography;
using System.Text;

namespace Cryptography;

public class CryptoUtils : ICryptoUtils
{
    private Encoding AesEncoding { get; } = Encoding.GetEncoding("ISO-8859-1");
    private string DefaultAesKey => "VWWdjXHCV8B3GWDUAz7yvK5PCQGvRxMT";
    private string DefaultAesIv => "j3c5kBVaXER3q6zX";

    public string GetMd5Hash(string input, Encoding? encoding = null)
    {
        var md5 = MD5.Create();
        byte[] bytes = encoding is null ? Encoding.ASCII.GetBytes(input) : encoding.GetBytes(input);
        var array = md5.ComputeHash(bytes).ToList();
        StringBuilder stringBuilder = new StringBuilder();
        array.ForEach(b => stringBuilder.Append(b.ToString("X2")));
        return stringBuilder.ToString();
    }

    public string Encrypt(string plainText, string? key = null, string? iv = null)
    {
        if (key is null)
            key = DefaultAesKey;

        if (iv is null)
            iv = DefaultAesIv;

        byte[] inArray;
        using (Aes aes = Aes.Create())
        {
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.BlockSize = 128;
            aes.Key = AesEncoding.GetBytes(key);
            aes.IV = AesEncoding.GetBytes(iv);

            using MemoryStream memoryStream = new MemoryStream();
            using (CryptoStream stream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                using StreamWriter streamWriter = new StreamWriter(stream, AesEncoding);
                streamWriter.Write(plainText);
            }
            inArray = memoryStream.ToArray();
        }
        return Convert.ToBase64String(inArray);
    }

    public string Decrypt(string cipherText, string? key = null, string? iv = null)
    {
        if (key is null)
            key = DefaultAesKey;

        if (iv is null)
            iv = DefaultAesIv;

        string result;
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.BlockSize = 128;
        aes.Key = AesEncoding.GetBytes(key);
        aes.IV = AesEncoding.GetBytes(iv);
        using var decryptor = aes.CreateDecryptor();
        var fullCipher = Convert.FromBase64String(cipherText);
        using (var msDecrypt = new MemoryStream(fullCipher))
        {
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            result = srDecrypt.ReadToEnd();
        }
        return result;
    }

    public string GetSha256String(string inputString, Encoding? encoding = null)
    {
        SHA256 sha256 = SHA256.Create();
        byte[] bytes = encoding is null ? Encoding.UTF8.GetBytes(inputString) : encoding.GetBytes(inputString);
        return GetStringFromHash(sha256.ComputeHash(bytes));
    }

    private string GetStringFromHash(byte[] hash)
    {
        StringBuilder stringBuilder = new StringBuilder();
        hash.ToList().ForEach(b => stringBuilder.Append(b.ToString("X2")));
        return stringBuilder.ToString();
    }

    public (string key, string iv) GenerateAesKey()
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.BlockSize = 128;
        aes.GenerateKey();
        aes.GenerateIV();
        return (Convert.ToBase64String(aes.Key), Convert.ToBase64String(aes.IV));
    }
}