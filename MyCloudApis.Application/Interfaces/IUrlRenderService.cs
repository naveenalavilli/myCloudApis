using MyCloudApis.Application.Contracts;

namespace MyCloudApis.Application.Interfaces;

public interface IUrlRenderService
{
    Task<byte[]> RenderAsync(UrlRenderRequest request, CancellationToken cancellationToken = default);
}
