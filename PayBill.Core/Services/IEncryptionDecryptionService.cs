using PayBill.Core.Models;

namespace PayBill.Core.Services;

public interface IEncryptionDecryptionService
{
    Task<string> Encrypt(EncryptionDecryptionModel model);
    Task<string> Decrypt(EncryptionDecryptionModel model);
}