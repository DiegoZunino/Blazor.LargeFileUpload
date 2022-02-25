namespace LargeFileUpload.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly string _filePath;

    public FileUploadController(IConfiguration configuration)
    {
        _filePath = configuration["FileUpload:FilePath"];
    }

    [HttpPost("chunk")]
    public async Task<IActionResult> ChunkUpload([FromBody] FileChunk request)
    {
        var fileName = Path.Combine(_filePath, request.FileName);
        if (request.First && System.IO.File.Exists(fileName))
        {
            System.IO.File.Delete(fileName);
        }
        await using var stream = System.IO.File.OpenWrite(fileName);
        stream.Seek(request.Offset, SeekOrigin.Begin);
        stream.Write(request.Data, 0, request.Data.Length);
        return StatusCode(500);
    }

    [HttpPost("stream")]
    [RequestSizeLimit(long.MaxValue)]
    [DisableFormValueModelBinding]
    public async Task<IActionResult> FileStream()
    {
        if (MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        {
            try
            {
                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType));
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);
                var section = await reader.ReadNextSectionAsync();
                var contentDisposition = MultipartRequestHelper.GetContentDisposition(section);

                if (MultipartRequestHelper.HasAllowedFileContentDisposition(contentDisposition))
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(contentDisposition.FileName.Value).ToLowerInvariant()}";
                    await using var targetStream = System.IO.File.Create(Path.Combine(_filePath, fileName));
                    await section.Body.CopyToAsync(targetStream);
                    return StatusCode(500);
                }
            }
            catch (InvalidDataException ex)
            {
                return BadRequest();
            }
        }
        return BadRequest();
    }
}
