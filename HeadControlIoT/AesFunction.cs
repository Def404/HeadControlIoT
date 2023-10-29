using System.Security.Cryptography;
using System.Text;

namespace ExecutingDevice;

public class AesFunction
{
    private static byte[] _key;
    private static byte[] _IV;

    public AesFunction(byte[] key, byte[] iv)
    {
        _key = key;
        _IV = iv;
    }
    
    public byte[] EncryptStringToBytes(string value)
    {
        
        if (value == null || value.Length <= 0)
            throw new ArgumentNullException("value");
        if (_key == null || _key.Length <= 0)
            throw new ArgumentNullException("_key");
        if (_IV == null || _IV.Length <= 0)
            throw new ArgumentNullException("_IV");
        
        
        byte[] encrypted;
        
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = _key;
            aesAlg.IV = _IV;
            
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(value);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }
    
    public string DecryptStringFromBytes(byte[] cipherText)
    {
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (_key == null || _key.Length <= 0)
            throw new ArgumentNullException("_key");
        if (_IV == null || _IV.Length <= 0)
            throw new ArgumentNullException("_IV");

        byte[] plaintextBytes = null;
        int plaintextBytesCount = 0;
        string plaintext = null;
       
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.KeySize = 128;
            aesAlg.Key = new byte[128 / 8];
            aesAlg.IV = new byte[128 / 8];
            aesAlg.Padding = PaddingMode.Zeros;  

            using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            {
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        plaintextBytes = new byte[cipherText.Length];
                        plaintextBytesCount = csDecrypt.Read(plaintextBytes, 0, plaintextBytes.Length);
                    }
                }
            };
            
        }

        plaintext = Encoding.UTF8.GetString(plaintextBytes, 0, plaintextBytesCount);
        return plaintext;
    }
}