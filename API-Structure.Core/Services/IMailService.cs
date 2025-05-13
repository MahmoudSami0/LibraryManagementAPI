using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Structure.Core.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string email, string token);
    }
}
