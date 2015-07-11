using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class CardTransaction
     {
          public CardInstance sourceCardInstanceRef = null;
          public CardInstance targetCardInstanceRef = null;
          public int targetSlotID;
          public int targetPlayerID;
          public int powerChangeResult = 0;
          public int strategicValue = 0;

          public CardTransaction()
          {
               targetSlotID = CardManager.CARD_LIST_LOC_INVALID;
               targetPlayerID = CardManager.PLAYER_INVALID;
          }
     }
}
