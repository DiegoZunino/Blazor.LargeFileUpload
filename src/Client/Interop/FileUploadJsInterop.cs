namespace LargeFileUpload.Client.Interop;

public class FileUploadJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public FileUploadJsInterop(IJSRuntime jsRuntime)
    {
        var importPath = "./js/fileUploadJsInterop.js";
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", importPath).AsTask());
    }

    public async ValueTask<FileStreamResponse> FileStream(string fileInputElementId)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<FileStreamResponse>("fileStream", fileInputElementId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}