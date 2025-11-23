using MyCloudApis.Application.Contracts;

namespace MyCloudApis.Application.Interfaces;

public interface ITemplateService
{
    Task<string> RenderHtmlAsync(TemplateRenderRequest request, CancellationToken cancellationToken = default);
}
