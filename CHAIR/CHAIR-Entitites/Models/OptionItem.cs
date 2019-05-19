using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CHAIR_Entities.Models
{
    public class OptionItem
    {
        public string iconKind { get; set; }
        public string name { get; set; }

        public OptionItem(string iconKind, string name)
        {
            this.iconKind = iconKind;
            this.name = name;
        }

        public OptionItem()
        {
        }
    }
}
