namespace FlightSearchAPI.Models.Amadeus
{
    public class Dictionaries
    {
        public Dictionary<string, Location> Locations { get; set; }
        public Dictionary<string, string> Aircraft { get; set; }
        public Dictionary<string, string> Currencies { get; set; }
        public Dictionary<string, string> Carriers { get; set; }
    }

    public class Location
    {
        public string CityCode { get; set; }
        public string CountryCode { get; set; }
    }
}
