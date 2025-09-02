using CSE3200.Domain.Entities;

namespace CSE3200.Web.Services
{
    public interface IMapsService
    {
        string GenerateMapUrl(string location, string coordinates = null);
        Task<string> GetLocationCoordinatesAsync(string location);
    }
}