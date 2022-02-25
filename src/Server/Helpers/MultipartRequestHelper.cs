namespace LargeFileUpload.Server.Helpers;

public static class MultipartRequestHelper
{
    public static string GetBoundary(MediaTypeHeaderValue contentType)
    {
        var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;
        if (!string.IsNullOrWhiteSpace(boundary))
        {
            return boundary;
        }

        throw new InvalidDataException("Missing Content-Type boundary.");
    }

    public static ContentDispositionHeaderValue GetContentDisposition(MultipartSection section)
    {
        if (ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
        {
            return contentDisposition;
        }

        throw new InvalidDataException("Invalid Content-Disposition header.");
    }

    public static bool IsMultipartContentType(string contentType)
    {
        return !string.IsNullOrEmpty(contentType)
               && contentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase);
    }

    public static bool HasAllowedFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
    {
        return contentDisposition != null
               && contentDisposition.DispositionType.Equals("form-data")
               && !string.IsNullOrEmpty(contentDisposition.FileName.Value);
    }
}