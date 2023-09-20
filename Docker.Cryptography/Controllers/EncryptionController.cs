using Docker.Cryptography.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Docker.Cryptography.Controllers;

[ApiController]
[Route("api")]
public class EncryptionController : ControllerBase
{
  private readonly ILogger<EncryptionController> _logger;
  private readonly SecretKeyManager _keyManager;

  public EncryptionController(
    ILogger<EncryptionController> logger,
    SecretKeyManager keyManager)
  {
    _logger = logger;
    _keyManager = keyManager;
  }

  public static byte[] HexToByteArray(string hexString)
  {
    byte[] byteArray = new byte[hexString.Length / 2];

    for (int i = 0; i < byteArray.Length; i++)
    {
      byteArray[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
    }

    return byteArray;
  }

  [HttpPost("encrypt")]
  public IActionResult Encrypt([FromBody] string plainText)
  {
    _logger.LogInformation("Handling encryption request for {Host}", HttpContext.Connection.RemoteIpAddress);

    try
    {
      using var aes = Aes.Create();
      aes.Key = HexToByteArray(_keyManager.GetSecretKey());
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      // Generate a random IV for each encryption.
      aes.GenerateIV();
      byte[] iv = aes.IV;

      using var encryptor = aes.CreateEncryptor(aes.Key, iv);

      using var memoryStream = new MemoryStream();
      using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
      using var streamWriter = new StreamWriter(cryptoStream);

      streamWriter.Write(plainText);
      streamWriter.Flush();
      cryptoStream.FlushFinalBlock();

      byte[] encrypted = iv.Concat(memoryStream.ToArray()).ToArray();
      string cipherText = Convert.ToBase64String(encrypted);

      return Ok(cipherText);
    }
    catch (Exception ex)
    {
      return BadRequest(new { Error = ex.Message });
    }
  }

  [HttpPost("decrypt")]
  public IActionResult Decrypt([FromBody] string cipherText)
  {
    _logger.LogInformation("Handling decryption request for {Host}", HttpContext.Connection.RemoteIpAddress);

    try
    {
      using var aes = Aes.Create();
      aes.Key = HexToByteArray(_keyManager.GetSecretKey());
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      byte[] encrypted = Convert.FromBase64String(cipherText);
      byte[] iv = encrypted.Take(16).ToArray();
      byte[] data = encrypted.Skip(16).ToArray();

      using var decryptor = aes.CreateDecryptor(aes.Key, iv);

      using var memoryStream = new MemoryStream(data);
      using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
      using var streamReader = new StreamReader(cryptoStream);

      string plainText = streamReader.ReadToEnd();

      return Ok(plainText);
    }
    catch (Exception ex)
    {
      return BadRequest(new { Error = ex.Message });
    }
  }
}