/* movieclips/sounds/Text */
/* SEMI COMPLETED*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Gwent.Models
{
     public class GwintPlayerRenderer
     {
          //public var txtPassed:red.game.witcher3.controls.W3TextArea;
          //public var txtFactionName:red.game.witcher3.controls.W3TextArea;
          //public var mcPlayerPortrait:flash.display.MovieClip;
          //public var mcLifeIndicator:flash.display.MovieClip;
          //public var mcFactionIcon:flash.display.MovieClip;

          public TextBox txtPlayerName = new TextBox();
          public TextBox txtCardCount = new TextBox();

          public Storyboard mcPassed;
          public Storyboard mcScore;
          public Storyboard mcWinningRound;
          
          protected string _playerNameDataProvider = "INVALID_STRING_PARAM!";
          protected int _playerID;
          private int _score = -1;
          protected int _lastSetPlayerLives = -1;
          private bool _turnActive = false;
          protected bool passedShown = false;

          public GwintPlayerRenderer()
          {
               _playerID = CardManager.PLAYER_INVALID;
          }

          public void showPassed(bool show)
          {
               /*if (txtPassed) 
               {
                   txtPassed.visible = show;
               }*/
               if (mcPassed != null)
               {
                    if (show)
                    {
                         if (!passedShown)
                         {
                              //GwintGameMenu.mSingleton.playSound("gui_gwint_turn_passed");
                         }
                         passedShown = true;
                         //mcPassed.gotoAndPlay("passed");
                    }
                    else
                    {
                         passedShown = false;
                         //mcPassed.gotoAndStop("Idle");
                    }
               }
          }

          public int score
          {
               set
               {
                    {
                         //Storyboard scoreUI = null;
                         if (_score != value)
                         {
                              /*if (mcScore.currentFrameLabel == "Idle")
                              {
                                   if (_score < value)
                                   {
                                        mcScore.gotoAndPlay("Grew");
                                   }
                                   else
                                   {
                                        mcScore.gotoAndPlay("Shrank");
                                   }
                              }*/
                              _score = value;
                              /*scoreUI = mcScore.getChildByName("txtScore") as red.game.witcher3.controls.W3TextArea;
                              if (scoreUI)
                              {
                                   scoreUI.Text = _score.toString();
                              }*/
                         }
                    }
               }
          }

          public void setIsWinning(bool winning)
          {
               if (mcWinningRound != null)
               {
                    if (winning)
                    {
                         //mcWinningRound.visible = true;
                         //mcWinningRound.play();
                    }
                    else
                    {
                         //mcWinningRound.visible = false;
                         //mcWinningRound.stop();
                    }
               }
          }

          public string playerName
          {
               get
               {
                    return txtPlayerName.Text;
               }

               set
               {
                    txtPlayerName.Text = value;
               }
          }

          public string playerNameDataProvider
          {
               get
               {
                    return _playerNameDataProvider;
               }

               set
               {
                    _playerNameDataProvider = value;
               }
          }

          //portrait?
          public int playerID
          {
               get
               {
                    return _playerID;
               }

               set
               {
                    _playerID = value;
                    /*if (mcPlayerPortrait) 
                    {
                        if (_playerID != CardManager.PLAYER_1) 
                        {
                            mcPlayerPortrait.gotoAndStop("npc");
                        }
                        else 
                        {
                            mcPlayerPortrait.gotoAndStop("geralt");
                        }
                    }*/
               }
          }

          public int numCardsInHand
          {
               set
               {
                    Console.WriteLine("player renderer: value {0}", value);
                    //txtCardCount.Text = value.ToString();
               }
          }

          public bool turnActive
          {
               set
               {
                    if (value != _turnActive)
                    {
                         _turnActive = value;
                         if (_turnActive)
                         {
                              //gotoAndPlay("Selected");
                         }
                         else
                         {
                              //gotoAndPlay("Idle");
                         }
                    }
               }
          }

          /*protected override function configUI():void
{
    base.configUI();
    if (_playerNameDataProvider != red.game.witcher3.constants.CommonConstants.INVALID_STRING_PARAM) 
    {
        dispatchEvent(new red.core.events.GameEvent(red.core.events.GameEvent.REGISTER, _playerNameDataProvider, [setPlayerName]));
    }
    if (mcPassed) 
    {
        txtPassed = mcPassed.getChildByName("txtPassed") as red.game.witcher3.controls.W3TextArea;
    }
    if (mcWinningRound) 
    {
        mcWinningRound.stop();
        mcWinningRound.visible = false;
    }
    reset();
            }*/

          protected void setPlayerName(string name)
          {
               playerName = name;
          }

          public void setPlayerLives(int lives)
          {
               /*var loc1=null;
               var loc2=null;
               Console.WriteLine("GFX - Updating life for Player: " + playerName + ", to: " + lives + " and life indicator: " + mcLifeIndicator);
               if (_lastSetPlayerLives != lives) 
               {
                   _lastSetPlayerLives = lives;
                   loc1 = mcLifeIndicator.getChildByName("mcLifeGemAnim1") as flash.display.MovieClip;
                   loc2 = mcLifeIndicator.getChildByName("mcLifeGemAnim2") as flash.display.MovieClip;
                   var loc3=lives;
                   switch (loc3) 
                   {
                       case 0:
                       {
                           if (loc2.currentLabel != "play") 
                           {
                               loc2.gotoAndPlay("play");
                           }
                           if (loc1.currentLabel != "play") 
                           {
                               loc1.gotoAndPlay("play");
                           }
                           break;
                       }
                       case 1:
                       {
                           if (loc2.currentLabel != "visible") 
                           {
                               loc2.gotoAndStop("visible");
                           }
                           if (loc1.currentLabel != "play") 
                           {
                               loc1.gotoAndPlay("play");
                           }
                           break;
                       }
                       case 2:
                       {
                           if (loc2.currentLabel != "visible") 
                           {
                               loc2.gotoAndStop("visible");
                           }
                           if (loc1.currentLabel != "visible") 
                           {
                               loc1.gotoAndStop("visible");
                           }
                           break;
                       }
                   }
               }
                       }

                            public function reset():void
           {
               if (txtPassed) 
               {
                   txtPassed.text = "[[gwint_player_passed_element]]";
                   txtPassed.visible = false;
               }
               score = 0;
               setPlayerLives(2);
               txtCardCount.text = "0";*/
          }

          public void reset()
          {
               Console.WriteLine("player renderer: reset needs update");
               /*if (txtPassed)
               {
                    txtPassed.text = "[[gwint_player_passed_element]]";
                    txtPassed.visible = false;
               }*/
               score = 0;
               setPlayerLives(2);
               //txtCardCount.text = "0";
          }
     }
}
