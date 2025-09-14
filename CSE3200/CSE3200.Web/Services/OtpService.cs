using CSE3200.Infrastructure.Identity;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace CSE3200.Web.Services
{
    public class OtpService : IOtpService
    {
        private readonly IDistributedCache _cache;
        private readonly TimeSpan _otpExpiration = TimeSpan.FromMinutes(10);

        public OtpService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public string GenerateOtp(ApplicationUser user)
        {
            // Generate a 6-digit OTP
            var random = new Random();
            string otp = random.Next(100000, 999999).ToString();

            // Store OTP in cache with user email as key
            var cacheKey = $"OTP_{user.Email}";
            var cacheValue = JsonSerializer.Serialize(new OtpData
            {
                Code = otp,
                Expiration = DateTime.UtcNow.Add(_otpExpiration)
            });

            _cache.Set(cacheKey, Encoding.UTF8.GetBytes(cacheValue), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _otpExpiration
            });

            return otp;
        }

        public bool ValidateOtp(ApplicationUser user, string otp)
        {
            var cacheKey = $"OTP_{user.Email}";
            var cachedData = _cache.Get(cacheKey);

            if (cachedData == null)
                return false;

            var otpData = JsonSerializer.Deserialize<OtpData>(Encoding.UTF8.GetString(cachedData));

            if (otpData == null || DateTime.UtcNow > otpData.Expiration)
                return false;

            return otpData.Code == otp;
        }

        private class OtpData
        {
            public string Code { get; set; }
            public DateTime Expiration { get; set; }
        }
    }
}