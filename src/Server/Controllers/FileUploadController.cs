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
    public async Task<IActionResult> UploadFileChunk([FromBody] FileChunk request)
    {
        var fileName = Path.Combine(_filePath, request.FileName);
        if (request.First && System.IO.File.Exists(fileName))
        {
            System.IO.File.Delete(fileName);
        }
        await using var stream = System.IO.File.OpenWrite(fileName);
        stream.Seek(request.Offset, SeekOrigin.Begin);
        stream.Write(request.Data, 0, request.Data.Length);

        return Ok();
    }

    [HttpGet("stream")]
    [RequestSizeLimit(long.MaxValue)]
    public async Task<IActionResult> StreamedUpload()
    {
        return Ok(new {message = "STREAMED"});
    }

    [HttpGet("standard")]
    public async Task<IActionResult> StandardUpload()
    {
        return Ok(new {message = "STANDARD"});
    }
}
