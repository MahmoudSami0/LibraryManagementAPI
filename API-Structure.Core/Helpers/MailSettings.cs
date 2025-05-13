
namespace API_Structure.Core.Helpers
{
    public class MailSettings
    {

        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public bool EnableSsl { get; set; }
        public string ConfirmationUrl { get; set; }
    }
}
