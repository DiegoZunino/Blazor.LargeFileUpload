namespace LargeFileUpload.Shared;

public class FileChunk
{
    public string FileName { get; set; } = "";
    public long Offset { get; set; }
    public byte[] Data { get; set; }
    public bool First = false;
}