using System.Security.Cryptography;
using System.Text;

namespace Docker.Cryptography.Services;

public class SecretKeyManager
{
  private readonly string _keyFilePath;

  public SecretKeyManager()
  {
    _keyFilePath = "/app/secrets/aesKey";
  }

  public string GetSecretKey()
  {
    string secretKey;

    if (File.Exists(_keyFilePath))
    {
      secretKey = File.ReadAllText(_keyFilePath);
    }
    else
    {
      secretKey = GenerateSecretKey(256 / 8);

      File.WriteAllText(_keyFilePath, secretKey, Encoding.ASCII);
    }

    // Check if the key is 256 bits long in hex.
    if (secretKey.Length != 256 / 4)
    {
      throw new ArgumentException("Key is not 256 bits long.");
    }

    return secretKey;
  }

  private static string GenerateSecretKey(int keySizeInBytes)
  {
    if (keySizeInBytes <= 0 || keySizeInBytes % 8 != 0)
    {
      throw new ArgumentException("Key size must be a positive multiple of 8 bits.");
    }

    using (var rng = RandomNumberGenerator.Create())
    {
      byte[] keyBytes = new byte[keySizeInBytes];
      rng.GetBytes(keyBytes);

      // Convert the key to hex.
      return BitConverter.ToString(keyBytes).Replace("-", "").ToLower();
    }
  }
}