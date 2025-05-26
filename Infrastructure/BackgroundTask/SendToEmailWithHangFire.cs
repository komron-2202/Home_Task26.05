using Domain.DTOs.Email;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundTasks;

public class SendToEmailWithHangFire(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<SendToEmailWithHangFire> logger)
{
    public async Task Send()
    {
        await using var scope = serviceScopeFactory.CreateAsyncScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var users = await dataContext.Users.ToListAsync();

        var now = DateTime.UtcNow;

        foreach (var user in users)
        {
            var emailDto = new EmailDto()
            {
                To = user.Email!,
                Subject = "Цена за заказ",
                Body =
                    $"<h1>Привет {user.UserName}.</h1>\n\n Цена вашего заказа составило 52 сомони.", 
            };

            await emailService.SendEmailAsync(emailDto);
        }
    }
}
