using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QUANTM.Data;
using QUANTM.Models.Common;

namespace QUANTM.Services.Common;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;
    private readonly string _uploadDirectory;

    public DocumentService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        // Check Environment Variable first, then appsettings, then default
        var envPath = Environment.GetEnvironmentVariable("FILE_UPLOAD_DIR");
        _uploadDirectory = !string.IsNullOrEmpty(envPath)
            ? envPath
            : (configuration["FileStorage:UploadDirectory"] ?? "Uploads");
    }

    public async Task<Document> UploadFileAsync(IFormFile file, int? userId, string? description = null)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        // Ensure directory exists
        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }

        // Create unique filename
        var fileExtension = Path.GetExtension(file.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(_uploadDirectory, uniqueFileName);

        // Save file to disk
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Save metadata to DB
        var document = new Document
        {
            FileName = file.FileName,
            FilePath = filePath,
            ContentType = file.ContentType,
            FileExtension = fileExtension,
            FileSize = file.Length,
            Description = description,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return document;
    }

    public async Task<(byte[] FileData, string ContentType, string FileName)> DownloadFileAsync(int documentId)
    {
        var document = await _context.Documents.FindAsync(documentId);
        if (document == null)
            throw new FileNotFoundException("Document not found in database");

        if (!File.Exists(document.FilePath))
            throw new FileNotFoundException("Physical file not found");

        var fileData = await File.ReadAllBytesAsync(document.FilePath);
        return (fileData, document.ContentType ?? "application/octet-stream", document.FileName);
    }
}
