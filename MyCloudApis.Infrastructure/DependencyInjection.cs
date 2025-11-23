using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCloudApis.Application.Contracts;
using MyCloudApis.Application.Interfaces;
using MyCloudApis.Infrastructure.Email;
using MyCloudApis.Infrastructure.Excel;
using MyCloudApis.Infrastructure.Pdf;
using MyCloudApis.Infrastructure.Templates;
using MyCloudApis.Infrastructure.Web;
using MyCloudApis.Infrastructure.Csv;

namespace MyCloudApis.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailOptions>(configuration.GetSection("Email"));
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IPdfService, PdfService>();
        services.AddSingleton<IExcelExportService, ExcelExportService>();
        services.AddSingleton<ITemplateService, TemplateService>();
        services.AddSingleton<IUrlRenderService, UrlRenderService>();
        services.AddSingleton<ICsvService, CsvService>();
        return services;
    }
}
