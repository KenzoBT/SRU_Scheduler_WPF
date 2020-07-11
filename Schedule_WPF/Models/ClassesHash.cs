using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_WPF.Models
{
    class ClassesHash
    {
        public ClassesHash(int crn, string hash)
        {
            CRN = crn;
            Hash = hash;
        }

        public int CRN { get; set; }
        public string Hash { get; set; }
    }
}
