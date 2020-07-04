using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_WPF.Models
{
    class MainLoaded
    {
        public static volatile bool isLoaded = false;
        public static volatile bool isClosed = false;

        public static void setLoaded()
        {
            isLoaded = true;
        }
        public static void setClosed()
        {
            isClosed = true;
        }
    }
}
