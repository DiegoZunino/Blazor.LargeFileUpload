using System.Net.Http.Json;
using LargeFileUpload.Shared;

namespace LargeFileUpload.Client.ApiClients;

public class FileUploadClient
{
    private readonly HttpClient _httpclient;

    public FileUploadClient(HttpClient _httpclient)
    {
        this._httpclient = _httpclient;
    }

    public async Task UploadFileChunk(FileChunk FileChunk)
    {
        var result = await _httpclient.PostAsJsonAsync("fileupload/chunk", FileChunk);
        result.EnsureSuccessStatusCode();
    }
    
    public async Task<FileUploadResponse> UploadAsync(string fileInputElementId)
    {
        await _fileUploadJsInterop.Init(token.Value);
        return await _fileUploadJsInterop.UploadFile(fileInputElementId);
    }
}