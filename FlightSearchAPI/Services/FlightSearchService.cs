using FlightSearchAPI.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace FlightSearchAPI.Services
{
    public class FlightSearchService
    {
        #region Fields
        private readonly HttpClient _httpClient;
        private string _flightOffersURL = "https://test.api.amadeus.com/v2/shopping/flight-offers";
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly IMemoryCache _cache;

        #endregion

        #region Constructors
        public FlightSearchService(HttpClient httpClient, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AmadeusAPI:ApiKey"];
            _apiSecret = configuration["AmadeusAPI:ApiSecret"];
            _cache = memoryCache;

        }

        #endregion

        #region Methods
        public async Task<FlightSearchResponse> SearchFlightsAsync(FlightSearchRequest request)
        {
            // Priprema URL-a za Amadeus API zahtjev
            string returnDate = "";
            if (request != null && request.ReturnDate.HasValue)
                returnDate = string.Format($"&returnDate={request.ReturnDate:yyyy-MM-dd}");
            string apiUrl = $"{_flightOffersURL}?originLocationCode={request.Origin}&destinationLocationCode={request.Destination}&departureDate={request.DepartureDate:yyyy-MM-dd}{returnDate}&adults={request.Passengers}&currencyCode={request.Currency}";

            // Dohvaćanje access tokena
            var accessToken = await GetAccessTokenAsync();

            // Ključ za cache generiran na temelju parametara pretrage
            var cacheKey = $"{request.Origin}-{request.Destination}-{request.DepartureDate}-{returnDate}-{request.Passengers}-{request.Currency}";
            // Provjeravamo postoji li rezultat u cache-u
            if (_cache.TryGetValue(cacheKey, out FlightSearchResponse cachedResponse))
            {
                // Ako postoji u cache-u, vraćamo ga
                return cachedResponse;
            }

            // Postavi autentifikaciju s API ključem
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Pošalji zahtjev
            var response = await _httpClient.GetAsync(apiUrl);

            // Provjeri je li zahtjev uspješan
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error fetching flight data from Amadeus API.");
            }

            // Deserijalizacija odgovora u listu letova
            var jsonResult = await response.Content.ReadAsStringAsync();
            var flightResults = JsonConvert.DeserializeObject<FlightSearchResponse>(jsonResult);

            // Postavljamo rezultat u cache, s vremenskim ograničenjem na 10 minuta
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            _cache.Set(cacheKey, flightResults, cacheEntryOptions);

            return flightResults;
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://test.api.amadeus.com/v1/security/oauth2/token");

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _apiKey),
                new KeyValuePair<string, string>("client_secret", _apiSecret),
            });

            request.Content = content;
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve access token from Amadeus API.");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);
            return tokenResponse.AccessToken;
        }

        #endregion

    }
}
