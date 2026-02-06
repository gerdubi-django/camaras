using System.Security.Cryptography;
using System.Text;
using NvrDesk.Domain.Contracts;

namespace NvrDesk.Infrastructure.Security;

public sealed class DpapiEncryptionService : IEncryptionService
{
    public string Encrypt(string plainText)
    {
        var bytes = Encoding.UTF8.GetBytes(plainText);
        var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(encrypted);
    }

    public string Decrypt(string encryptedText)
    {
        var bytes = Convert.FromBase64String(encryptedText);
        var decrypted = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
        return Encoding.UTF8.GetString(decrypted);
    }
}
