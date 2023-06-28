using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetUpBot
{
    interface IIncludable
    {

    }
    class Department:IIncludable
    {
        public string Name;

        public List<IIncludable> Workers;
    }
}
