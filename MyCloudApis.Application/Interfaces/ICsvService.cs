using MyCloudApis.Application.Contracts;

namespace MyCloudApis.Application.Interfaces;

public interface ICsvService
{
    Task<byte[]> ExportAsync(CsvExportRequest request, CancellationToken cancellationToken = default);
    Task<CsvImportResult> ImportAsync(Stream csvStream, string delimiter = ",", CancellationToken cancellationToken = default);
}
