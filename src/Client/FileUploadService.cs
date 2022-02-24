namespace LargeFileUpload.Client;

public class FileUploadService
{
    private readonly FileUploadClient _apiClient;
    private readonly long _chunkSize;

    public FileUploadService(FileUploadClient fileUploadClient)
    {
        _chunkSize = 1048576; //1Mb
        _apiClient = fileUploadClient;
    }

        
    public async Task ChunksUpload(IBrowserFile file)
    {
        var first = true;
        long  offset = 0;
            
        var numChunks = file.Size / _chunkSize;
        var remainder = file.Size % _chunkSize;
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.Name).ToLowerInvariant()}";

        await using var inStream = file.OpenReadStream(long.MaxValue);
        for (var i = 0; i < numChunks; i++)
        {
            var buffer = new byte[_chunkSize];
            await inStream.ReadAsync(buffer, 0, buffer.Length);
                    
            var chunk = new FileChunk
            {
                Data = buffer,
                FileName = fileName,
                Offset = offset += _chunkSize,
                First = first
            };
            await _apiClient.UploadFileChunk(chunk);
            first = false;
        }

        if (remainder > 0)
        {
            var buffer = new byte[remainder];
            await inStream.ReadAsync(buffer, 0, buffer.Length);
                    
            var chunk = new FileChunk
            {
                Data = buffer,
                FileName = fileName,
                Offset = offset,
                First = first
            };
            await _apiClient.UploadFileChunk(chunk);
        }
    }
    
    public async Task<string> JsStream(string fileInputElementId)
    {
        try
        {
            var response = await _apiClient.UploadAsync(fileInputElementId);

            if (response.Status == ResponseStatus.Error)
            {
                NotifyErrorOccurred(this, "UploadBadRequest");
                return null;
            }

            NotifyFileUploaded(this);
            return response.FileName;
        }
        catch
        {
            NotifyErrorOccurred(this, "GenericError");
            return null;
        }

    }
}