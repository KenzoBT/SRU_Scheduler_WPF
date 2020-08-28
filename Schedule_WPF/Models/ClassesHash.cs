using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_WPF.Models
{
    class ClassesHash
    {
        public ClassesHash(string ID, string hash)
        {
            ClassID = ID;
            Hash = hash;
        }

        public string ClassID { get; set; }
        public string Hash { get; set; }
    }
}
