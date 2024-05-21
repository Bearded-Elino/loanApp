using System.Threading.Tasks;

namespace Loanapp.Utilities
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}


