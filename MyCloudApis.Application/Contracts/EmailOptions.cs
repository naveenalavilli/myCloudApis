using MyCloudApis.Domain;

namespace MyCloudApis.Application.Contracts;

public class EmailOptions
{
    public SmtpSettings? DefaultSmtp { get; set; }
}
