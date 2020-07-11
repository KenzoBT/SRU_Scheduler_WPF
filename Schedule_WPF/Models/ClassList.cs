using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_WPF.Models
{
    public class ClassList : ObservableCollection<Classes>
    {
        public ClassList() : base()
        {

        }

        public byte[] Serialize()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    for (int i = 0; i < this.Count(); i++)
                    {
                        writer.Write(this[i].Serialize());
                    }
                }
                return m.ToArray();
            }
        }
    }
}
