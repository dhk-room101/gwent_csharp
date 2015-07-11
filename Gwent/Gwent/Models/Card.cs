using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Gwent.Models
{
     public class Card
     {
          public string Name { get; set; }
          public Rectangle Face { get; set; }
          public String Deck { get; set; }
          public bool FaceUp { get; set; }
     }
}
