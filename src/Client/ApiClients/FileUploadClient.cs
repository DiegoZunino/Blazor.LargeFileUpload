using System.Net.Http.Json;
using LargeFileUpload.Client.Interop;

namespace LargeFileUpload.Client.ApiClients;

public class FileUploadClient
{
    private readonly HttpClient _httpclient;
    private readonly FileUploadJsInterop _fileUploadJsInterop;

    public FileUploadClient(HttpClient httpclient, FileUploadJsInterop fileUploadJsInterop)
    {
        _httpclient = httpclient;
        _fileUploadJsInterop = fileUploadJsInterop;
    }

    public async Task ChunkUpload(FileChunk FileChunk)
    {
        var result = await _httpclient.PostAsJsonAsync("fileupload/chunk", FileChunk);
        result.EnsureSuccessStatusCode();
    }
    
    public async Task FileStream(string fileInputElementId)
    {
        var result = await _fileUploadJsInterop.FileStream(fileInputElementId);
        if (result.StatusCode != 200)
        {
            throw new HttpRequestException();
        }
    }
}