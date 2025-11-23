using MyCloudApis.Application.Contracts;

namespace MyCloudApis.Application.Interfaces;

public interface IExcelExportService
{
    Task<byte[]> ExportAsync(JsonToExcelRequest request, CancellationToken cancellationToken = default);
}
