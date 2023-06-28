using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpBot.Models
{
    class Meeting
    {
        public string Theme;

        public string Description;

        public DateTime SuggestedTime;

        public List<Employee> Participants;

        public void GetApprovement()
        {
            foreach (var person in Participants)
            {

            }

        }

        void FinalizeTime()
        {
           
        }
    }
}
