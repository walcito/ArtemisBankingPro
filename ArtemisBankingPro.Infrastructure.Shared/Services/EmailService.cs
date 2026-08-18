using ArtemisBankingPro.Core.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ArtemisBankingPro.Infrastructure.Shared.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body)
    {
        _logger.LogInformation("Enviando correo a {To} con asunto: {Subject}", to, subject);
        return Task.CompletedTask;
    }
}
