namespace carwash.Service.Interfaces;

public interface IQrCodeService
{
    string GenerateBase64(string content);
}
