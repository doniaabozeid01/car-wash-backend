using carwash.Service.Interfaces;
using QRCoder;

namespace carwash.Service.Services;

public class QrCodeService : IQrCodeService
{
    public string GenerateBase64(string content)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrBytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(qrBytes);
    }
}
