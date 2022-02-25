namespace LargeFileUpload.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly string _dirName;

    public FileUploadController(IConfiguration configuration)
    {
        _dirName = configuration["FileUpload:DirName"];
    }

    [HttpPost("chunk")]
    public async Task<IActionResult> ChunkUpload([FromBody] FileChunk request)
    {
        var filePath = Path.Combine(_dirName, request.FileName);
        if (request.First && System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
        await using var stream = System.IO.File.OpenWrite(filePath);
        stream.Seek(request.Offset, SeekOrigin.Begin);
        stream.Write(request.Data, 0, request.Data.Length);
        return Ok();
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
                    await using var targetStream = System.IO.File.Create(Path.Combine(_dirName, fileName));
                    await section.Body.CopyToAsync(targetStream);
                    return Ok();
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
