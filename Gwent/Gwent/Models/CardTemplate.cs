using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class CardTemplate
     {
          public int index { get; set; }
          public int power { get; set; }
          public String title { get; set; }
          public String description { get; set; }
          public String imageLoc { get; set; }
          public int factionIdx { get; set; }
          public List<int> typeFlags { get; set; }
          public List<int> effectFlags { get; set; }
          public List<int> summonFlags { get; set; }
     }
}
