﻿/* movieclips/sounds/text */
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

          public string factionString
          {
               set { ;}
               //mcDeckTop.gotoAndStop(arg1);
          }
     }
}