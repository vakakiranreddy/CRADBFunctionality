namespace ConferenceRoomBooking.Business.Interfaces.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
    }
}








