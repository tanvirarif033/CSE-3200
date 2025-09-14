using CSE3200.Infrastructure.Identity;

namespace CSE3200.Web.Services
{
    public interface IOtpService
    {
        string GenerateOtp(ApplicationUser user);
        bool ValidateOtp(ApplicationUser user, string otp);
    }
}
