namespace LargeFileUpload.Client.Interop;

public class FileUploadJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private DotNetObjectReference<FileUploadJsInterop> _reference;

    public FileUploadJsInterop(IJSRuntime jsRuntime)
    {
        var importPath = "./js/fileUploadJsInterop.js";
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", importPath).AsTask());
    }

    public async ValueTask<bool> Init()
    {
        var module = await _moduleTask.Value;
        _reference = DotNetObjectReference.Create(this);
        return await module.InvokeAsync<bool>("init", _reference);
    }

    public async ValueTask<FileUploadResponse> UploadFile(string fileInputElementId)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<FileUploadResponse>("uploadFile", fileInputElementId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }

        _reference?.Dispose();
        GC.SuppressFinalize(this);
    }
}