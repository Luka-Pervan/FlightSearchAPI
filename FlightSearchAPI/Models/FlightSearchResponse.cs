using FlightSearchAPI.Models.Amadeus;

namespace FlightSearchAPI.Models
{
    public class FlightSearchResponse
    {
        public Meta Meta { get; set; }
        public List<FlightOfferData> Data { get; set; }
        public Dictionaries Dictionaries { get; set; }


    }
}
