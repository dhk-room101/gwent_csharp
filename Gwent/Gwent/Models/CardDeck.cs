using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class CardDeck
     {
          public List<int> cardIndicesInDeck { get; set; }
          public List<int> cardIndicesSpecial { get; set; }
          public int deckTypeIndex { get; set; }
          public string deckTypeName { get; set; }

          public CardDeck()
          {
               cardIndicesInDeck = new List<int>();
               cardIndicesSpecial = new List<int>();
          }
     }
}
