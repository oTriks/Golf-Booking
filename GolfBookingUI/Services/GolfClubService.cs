using System.Net.Http.Json;
using GolfBookingUI.Models;

namespace GolfBookingUI.Services
{
    public class GolfClubService
    {
        private readonly HttpClient _httpClient;

        public GolfClubService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ClubReadDto>> GetClubsAsync()
        {
            var clubs = await _httpClient.GetFromJsonAsync<List<ClubReadDto>>("api/GolfClub");
            return clubs ?? new List<ClubReadDto>();
        }
    }
}
