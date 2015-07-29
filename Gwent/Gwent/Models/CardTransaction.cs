using Gwent.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class CardTransaction
     {
          public CardSlot sourceCard { get; set; }
          public CardSlot targetCard { get; set; }
          public int targetHolderID { get; set; }
          public int targetPlayerID { get; set; }
          public int powerChangeResult = 0;
          public int strategicValue = 0;

          public CardTransaction()
          {
               targetHolderID = ValuesRepository.CARD_LIST_LOC_INVALID;
               targetPlayerID = ValuesRepository.PLAYER_INVALID;
          }
     }
}
