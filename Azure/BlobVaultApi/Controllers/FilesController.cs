using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
 
namespace BlobVaultApi.Controllers;
 
[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly BlobContainerClient _container;
    private const string ContainerName = "uploads";
 
    public FilesController(BlobServiceClient blobServiceClient)
    {
        _container = blobServiceClient.GetBlobContainerClient(ContainerName);
        _container.CreateIfNotExists();
    }
 
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");
 
        var blobClient = _container.GetBlobClient(file.FileName);
        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);
 
        return Ok(new { fileName = file.FileName, url = blobClient.Uri.ToString() });
    }
 
    [HttpGet("download/{fileName}")]
        public async Task<IActionResult> Download(string fileName)
    {
        var blobClient = _container.GetBlobClient(fileName);
        if (!await blobClient.ExistsAsync())
            return NotFound();
 
        var download = await blobClient.DownloadContentAsync();
        return File(download.Value.Content.ToArray(), "application/octet-stream", fileName);
    }
}
