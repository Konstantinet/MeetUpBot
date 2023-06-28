using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpBot
{
    class Employee:IIncludable
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        private string TelegramID;
    }
}
