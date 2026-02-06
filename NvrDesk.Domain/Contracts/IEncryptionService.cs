namespace NvrDesk.Domain.Contracts;

public interface IEncryptionService
{
    string Encrypt(string plainText);
    string Decrypt(string encryptedText);
}
