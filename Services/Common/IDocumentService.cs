using Microsoft.AspNetCore.Http;
using QUANTM.Models.Common;

namespace QUANTM.Services.Common;

public interface IDocumentService
{
    Task<Document> UploadFileAsync(IFormFile file, int? userId, string? description = null);
    Task<(byte[] FileData, string ContentType, string FileName)> DownloadFileAsync(int documentId);
}
