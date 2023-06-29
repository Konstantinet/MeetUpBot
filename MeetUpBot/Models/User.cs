using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpBot.Models
{
    class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string TelegramID { get; set; }
        public long ChatID { get; set; }
        public ICollection<MeetUp> Meetings { get; set; }
        public ICollection<Invitation> Invitations { get; set; }
        public bool TimeChosen { get; set; } = false;
        public User()
        {
            Meetings = new List<MeetUp>();
        }
    }
}
