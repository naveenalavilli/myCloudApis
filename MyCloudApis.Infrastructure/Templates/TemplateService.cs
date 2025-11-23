using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;
using RazorLight;

namespace MyCloudApis.Infrastructure.Templates;

/// <summary>
/// Renders Razor templates from strings with a dynamic model.
/// </summary>
public class TemplateService : ITemplateService
{
    private readonly RazorLightEngine _engine;

    public TemplateService()
    {
        _engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(TemplateService))
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<string> RenderHtmlAsync(TemplateRenderRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Template))
        {
            throw new ArgumentException("Template content is required.", nameof(request.Template));
        }

        var key = $"template-{request.Template.GetHashCode()}";
        return await _engine.CompileRenderStringAsync(key, request.Template, request.Model);
    }
}
