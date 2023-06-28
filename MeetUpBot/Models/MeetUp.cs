using System;
using System.Collections.Generic;

namespace MeetUpBot.Models
{
    class MeetUp
    {
        public int Id { get; set; }
        public string Theme { get; set; }
        public string Description { get; set; }
        public ICollection<User> Participants { get;set; }
        public string Stage { get; set; }
        public DateTime Time { get; set; }
        public MeetUp()
        {
            Participants = new List<User>();
        }
    }
}
