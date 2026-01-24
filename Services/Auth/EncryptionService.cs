using System.Security.Cryptography;
using System.Text;

namespace LMS.Services.Auth
{
    public class EncryptionService
    {
        private readonly string _key;
        private readonly string _salt;

        public EncryptionService(IConfiguration configuration)
        {
            _key = configuration["EncryptionKey"] ?? throw new ArgumentNullException("Encryption Key is missing");
            _salt = configuration["EncryptionSalt"] ?? throw new ArgumentNullException("Encryption Salt is missing");
        }

        // ENCRYPT: Encrypts text using AES-256
        // Returns: Base64 string containing [IV + CipherText]
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return plainText;
            }

            using (var aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(_key);
                aes.GenerateIV(); // Random IV for each encryption

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    // Prepend IV to the stream so we can use it for decryption
                    msEncrypt.Write(aes.IV, 0, aes.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // DECRYPT: Decrypts the Base64 string
        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;
            try
            {
                var fullCipher = Convert.FromBase64String(cipherText);
                using (var aes = Aes.Create())
                {
                    aes.Key = Convert.FromBase64String(_key);
                    // Extract the IV (first 16 bytes)
                    byte[] iv = new byte[16];
                    Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                    aes.IV = iv;
                    // The rest is the actual encrypted data
                    var encryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (var msDecrypt = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                    using (var csDecrypt = new CryptoStream(msDecrypt, encryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch
            {
                // If decryption fails (e.g. wrong key, bad data), return original
                return cipherText;
            }
        }

        // HASH: Creates a one-way hash for searching (Deterministic)
        public string Hash(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            using (var sha256 = SHA256.Create())
            {
                var combinedBytes = Encoding.UTF8.GetBytes(text + _salt);
                var hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToHexString(hashBytes).ToLower();
            }
        }
    }
}