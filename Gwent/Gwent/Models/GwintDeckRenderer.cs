/* movieclips/sounds/text */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class GwintDeckRenderer
     {
          private int _cardCount = 0;
          private string _factionString;
          //public var mcCardCount:flash.display.MovieClip;
          //public var mcDeckTop:flash.display.MovieClip;
          
          public GwintDeckRenderer()
          {

          }

          public int cardCount
          {
               set
               {
                    _cardCount = value;
                    Console.WriteLine("deck renderer: card count overlapping/text not implemented yet");
                    /*if (_cardCount != 0) 
                    {
                        gotoAndStop(Math.min(50, _cardCount));
                        mcDeckTop.visible = true;
                    }
                    else 
                    {
                        gotoAndStop(1);
                        mcDeckTop.visible = false;
                    }*/
                    /*var clip=mcCardCount ? mcCardCount.getChildByName("txtCount") as red.game.witcher3.controls.W3TextArea : null;
                    if (clip) 
                    {
                        clip.text = _cardCount.toString();
                    }*/
               }
          }

          //TO DO display deck top image based on faction string
          public string factionString
          {
               get { return _factionString; }
               set { _factionString = value; }
               //mcDeckTop.gotoAndStop(arg1);
          }
     }
}
