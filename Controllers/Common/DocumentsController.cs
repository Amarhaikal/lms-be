using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QUANTM.Services.Common;

namespace QUANTM.Controllers.Common;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string? description)
    {
        try
        {
            // Get current user ID from claims if available
            int? userId = null;
            var userIdClaim = User.FindFirst("id");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedId))
            {
                userId = parsedId;
            }

            var document = await _documentService.UploadFileAsync(file, userId, description);
            return Ok(document);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while uploading the file", details = ex.Message });
        }
    }

    [HttpGet("{id}/content")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        try
        {
            var (fileData, contentType, fileName) = await _documentService.DownloadFileAsync(id);
            return File(fileData, contentType, fileName);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while downloading the file", details = ex.Message });
        }
    }
}
