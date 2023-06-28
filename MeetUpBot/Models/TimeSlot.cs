using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpBot.Models
{
    class TimeSlot
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public DateTime Start { get; set; }
    }
}
