using System.Text.Json;
using CSE3200.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace CSE3200.Web.Services
{
    public class GoogleMapsService : IMapsService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleMapsService> _logger;

        public GoogleMapsService(IConfiguration configuration, HttpClient httpClient, ILogger<GoogleMapsService> logger)
        {
            _apiKey = configuration["GoogleMaps:ApiKey"];
            _httpClient = httpClient;
            _logger = logger;
        }

        public string GenerateMapUrl(string location, string coordinates = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(coordinates))
                {
                    // Use coordinates for precise mapping
                    return $"https://www.google.com/maps/embed/v1/place?key={_apiKey}&q={Uri.EscapeDataString(coordinates)}&zoom=12";
                }
                else
                {
                    // Fallback to location name
                    var encodedLocation = Uri.EscapeDataString(location);
                    return $"https://www.google.com/maps/embed/v1/place?key={_apiKey}&q={encodedLocation}&zoom=10";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating map URL for location: {Location}", location);
                return null;
            }
        }

        public async Task<string> GetLocationCoordinatesAsync(string location)
        {
            try
            {
                var encodedLocation = Uri.EscapeDataString(location);
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedLocation}&key={_apiKey}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(content);

                var status = jsonDocument.RootElement.GetProperty("status").GetString();

                if (status == "OK")
                {
                    var results = jsonDocument.RootElement.GetProperty("results");
                    if (results.GetArrayLength() > 0)
                    {
                        var locationData = results[0].GetProperty("geometry").GetProperty("location");
                        var lat = locationData.GetProperty("lat").GetDouble();
                        var lng = locationData.GetProperty("lng").GetDouble();

                        return $"{lat},{lng}";
                    }
                }
                else
                {
                    _logger.LogWarning("Geocoding API returned status: {Status} for location: {Location}", status, location);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting coordinates for location: {Location}", location);
                return null;
            }
        }
    }
}