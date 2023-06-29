using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpBot.Models
{
    class Invitation
    {
        public int MeetUpId { get; set; }
        public MeetUp MeetUp { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public bool TimeApproved { get; set; }
    }
}
