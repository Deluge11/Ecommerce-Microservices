

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Business_Layer.Business;

public class ImagesBusiness 
{
    public ILogger<ImagesBusiness> Logger { get; }

    public ImagesBusiness(ILogger<ImagesBusiness> logger)
    {
        Logger = logger;
    }

    public bool IsAnimatedWebP(Stream webpStream)
    {
        if (webpStream == null || webpStream.Length == 0)
        {
            return false;
        }

        byte[] buffer = new byte[256];
        webpStream.Read(buffer, 0, buffer.Length);

        string content = System.Text.Encoding.ASCII.GetString(buffer);

        return content.Contains("ANIM");
    }
    public async Task<bool> StreamImage(string filePath, IFormFile file)
    {
        if (filePath == null || file == null)
        {
            return false;
        }

        filePath = filePath.Trim();

        if (filePath.Length < 1 || file.Length < 1)
        {
            return false;
        }

        try
        {
            using var fileStream = new FileStream(filePath, FileMode.CreateNew);
           await file.CopyToAsync(fileStream);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError("Failed to save image : {ex}", ex);
            return false;
        }
    }
    public async Task<bool> IsValidImage(IFormFile file)
    {
        if (file == null || file.Length < 1) 
            return false;

        const long maxFileSizeBytes = 3 * 1024 * 1024;
        if (file.Length > maxFileSizeBytes) 
            return false;

        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
        if (extension != ".webp") 
            return false;

        var contentType = file.ContentType?.ToLowerInvariant();
        if (contentType != "image/webp") 
            return false;

        byte[] header = new byte[64];
        using var stream = file.OpenReadStream();
        int read = await stream.ReadAsync(header, 0, header.Length);
        if (read < 12) 
            return false;

        if (!(header[0] == (byte)'R' && header[1] == (byte)'I' && header[2] == (byte)'F' && header[3] == (byte)'F'))
            return false;

        if (!(header[8] == (byte)'W' && header[9] == (byte)'E' && header[10] == (byte)'B' && header[11] == (byte)'P'))
            return false;

        if (IsAnimatedWebP(stream)) 
            return false;

        return true;
    }


}
