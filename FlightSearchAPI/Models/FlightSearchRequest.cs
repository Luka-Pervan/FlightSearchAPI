namespace FlightSearchAPI.Models
{
    public class FlightSearchRequest
    {
        public string Origin { get; set; }  // Polazni aerodrom (IATA kod)
        public string Destination { get; set; }  // Odredišni aerodrom (IATA kod)
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }  // Povratak, može biti opcionalan
        public int Passengers { get; set; }  // Broj putnika
        public string Currency { get; set; }  // Valuta (USD, EUR, HRK)

    }
}
