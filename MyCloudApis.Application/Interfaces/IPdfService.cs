namespace MyCloudApis.Application.Interfaces;

public interface IPdfService
{
    Task<byte[]> FromHtmlAsync(string html, CancellationToken cancellationToken = default);
}
