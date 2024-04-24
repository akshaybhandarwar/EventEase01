

namespace EventEase_01.Models
{
    public class EventModel
    {
        public string EventName { get; set; }   

        public string EventDescription { get; set; }

        public DateTime EventDate { get; set; }
        //public string VenueName { get; set; }   
        public Guid VenueId { get; set; } 
        public List<Venue> VenueOptions { get; set; }
    }
}
