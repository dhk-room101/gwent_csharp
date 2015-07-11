using System;

namespace Gwent.Models
{
     public class GwintEventListener
     {
          public const string HOLDER_SELECTED = "holder_selected";
          public const string HOLDER_CHOSEN = "holder_chosen";
          public const string CARD_SELECTED = "card_selected";
          public const string CARD_CHOSEN = "card_chosen";
          
          //public CardSlot _CardSlot;
          //public GwintCardHolder CardHolder;
          public GwintBoardRenderer Board;

          public GwintEventListener(GwintBoardRenderer board)
          {
               Board = board;
               Board.Changed += new GwintBoardEventHandler(BoardChanged);
          }

          private void BoardChanged(object sender, EventArgs e)
          {
               Console.WriteLine("something fired");
          }

          public void Detach()
          {
               Board.Changed -= new GwintBoardEventHandler(BoardChanged);
               Board = null;
          }
     }
}