using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Schedule_WPF.Models
{
    [XmlRoot("Pairs")]
    public class Pairs
    {
        [XmlArray("ColorPairings"), XmlArrayItem(typeof(ProfColors), ElementName = "ProfColors")]
        public List<ProfColors> ColorPairings { get; set; }
    }
}
