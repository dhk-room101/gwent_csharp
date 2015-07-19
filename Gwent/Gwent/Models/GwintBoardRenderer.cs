using Gwent.ViewModels;
/* movieclips/sounds/text */
//TO DO boardRendererRef?
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Gwent.Models
{
     public delegate void GwintBoardEventHandler(object sender, EventArgs e);

     public class GwintBoardRenderer
     {
          public event GwintBoardEventHandler Changed;

          private List<GwintCardHolder> allRenderers;
          private Dictionary<int, CardSlot> allCardSlotInstances;
          private CardManager cardManager;
          private int _selectedIndex;//inherited from slot base

          public GwintCardHolder mcWeather;
          public GwintCardHolder mcP1LeaderHolder;
          public GwintCardHolder mcP2LeaderHolder;
          public GwintCardHolder mcP1Deck;
          public GwintCardHolder mcP2Deck;
          public GwintCardHolder mcP1Hand;
          public GwintCardHolder mcP2Hand;
          public GwintCardHolder mcP1Graveyard;
          public GwintCardHolder mcP2Graveyard;
          public GwintCardHolder mcP1Siege;
          public GwintCardHolder mcP2Siege;
          public GwintCardHolder mcP1Range;
          public GwintCardHolder mcP2Range;
          public GwintCardHolder mcP2SiegeModif;
          public GwintCardHolder mcP1Melee;
          public GwintCardHolder mcP2Melee;
          public GwintCardHolder mcP1SiegeModif;
          public GwintCardHolder mcP1RangeModif;
          public GwintCardHolder mcP2RangeModif;
          public GwintCardHolder mcP1MeleeModif;
          public GwintCardHolder mcP2MeleeModif;

          public Storyboard mcTransitionAnchor;
          //public var mcGodCardHolder:flash.display.MovieClip;
          //public var rowScoreP2Seige:flash.display.MovieClip;
          //public var rowScoreP2Ranged:flash.display.MovieClip;
          //public var rowScoreP2Melee:flash.display.MovieClip;
          //public var rowScoreP1Melee:flash.display.MovieClip;
          //public var rowScoreP1Ranged:flash.display.MovieClip;
          //public var rowScoreP1Seige:flash.display.MovieClip;
          //public var mcTransactionTooltip:flash.display.MovieClip;

          public GwintBoardRenderer()
          {
               allCardSlotInstances = new Dictionary<int, CardSlot>();

               updateRowScores(0, 0, 0, 0, 0, 0);

               fillRenderersList();
               
               /*if (mcTransactionTooltip) 
               {
                   mcTransactionTooltip.visible = false;
                   mcTransactionTooltip.alpha = 0;
               }*/

               //setupCardHolders();//not needed?

               //not needed?
               cardManager = CardManager.getInstance();
               CardManager.getInstance().boardRenderer = this;
          }

          protected virtual void OnChanged( EventArgs e)
          {
               if ( Changed != null)
               {
                    Changed(this, e);
               }
          }

          public void updateRowScores(int siegeP1, int rangeP1, int meleeP1, int meleeP2, int rangeP2, int siegeP2)
          {
               /*Storyboard rowScore=null;
               rowScore = rowScoreP1Seige.getChildByName("txtScore") as red.game.witcher3.controls.W3TextArea;
               if (rowScore) 
               {
                   rowScore.text = siegeP1.toString();
               }
               rowScore = rowScoreP1Ranged.getChildByName("txtScore") as red.game.witcher3.controls.W3TextArea;
               if (rowScore) 
               {
                   rowScore.text = rangeP1.toString();
               }
               rowScore = rowScoreP1Melee.getChildByName("txtScore") as red.game.witcher3.controls.W3TextArea;
               if (rowScore) 
               {
                   rowScore.text = meleeP1.toString();
               }
               rowScore = rowScoreP2Melee.getChildByName("txtScore") as red.game.witcher3.controls.W3TextArea;
               if (rowScore) 
               {
                   rowScore.text = meleeP2.toString();
               }
               rowScore = rowScoreP2Ranged.getChildByName("txtScore") as red.game.witcher3.controls.W3TextArea;
               if (rowScore) 
               {
                   rowScore.text = rangeP2.toString();
               }
               rowScore = rowScoreP2Seige.getChildByName("txtScore") as red.game.witcher3.controls.W3TextArea;
               if (rowScore) 
               {
                   rowScore.text = siegeP2.toString();
               }*/
          }

          public GwintCardHolder getCardHolder(int cardHolderID, int playerID)
          {
               int counter = 0;
               GwintCardHolder cardHolder = null;
               while (counter < allRenderers.Count)
               {
                    cardHolder = allRenderers[counter];
                    if (cardHolder.cardHolderID == cardHolderID && cardHolder.playerID == playerID)
                    {
                         return cardHolder;
                    }
                    ++counter;
               }
               return null;
          }

          public CardSlot getCardSlotById(int slotID)
          {
               return allCardSlotInstances[slotID];
          }

          public void updateTransactionCardTooltip(CardSlot cardSlot)
          {
               /*var loc1=null;
               var loc2=null;
               var loc3=null;
               var loc4=null;
               var loc5=null;
               if (mcTransactionTooltip) 
               {
                   if (cardSlot == null) 
                   {
                       if (mcTransactionTooltip.visible) 
                       {
                           com.gskinner.motion.GTweener.removeTweens(mcTransactionTooltip);
                           com.gskinner.motion.GTweener.to(mcTransactionTooltip, 0.2, {"alpha":0}, {"onComplete":onTooltipHideEnded});
                       }
                   }
                   else 
                   {
                       visible = true;
                       com.gskinner.motion.GTweener.removeTweens(mcTransactionTooltip);
                       com.gskinner.motion.GTweener.to(mcTransactionTooltip, 0.2, {"alpha":1}, {});
                       if (cardManager) 
                       {
                           loc1 = cardManager.getCardTemplate(cardSlot.cardIndex);
                           loc2 = loc1.tooltipString;
                           loc3 = mcTransactionTooltip.getChildByName("txtTooltipTitle") as flash.text.TextField;
                           loc4 = mcTransactionTooltip.getChildByName("txtTooltip") as flash.text.TextField;
                           if (loc2 == "" || !loc3 || !loc4) 
                           {
                               mcTransactionTooltip.visible = false;
                           }
                           else 
                           {
                               mcTransactionTooltip.visible = true;
                               if (loc1.index >= 1000) 
                               {
                                   loc3.text = "[[gwint_leader_ability]]";
                               }
                               else 
                               {
                                   loc3.text = "[[" + loc2 + "_title]]";
                               }
                               loc4.text = "[[" + loc2 + "]]";
                               loc5 = mcTransactionTooltip.getChildByName("mcTooltipIcon") as flash.display.MovieClip;
                               if (loc5) 
                               {
                                   loc5.gotoAndStop(loc1.tooltipIcon);
                               }
                           }
                       }
                   }
               }*/
          }

          public void wasAddedToList(CardInstance cardInstance, int cardHolderID, int playerID)
          {
               GwintCardHolder cardHolder = getCardHolder(cardHolderID, playerID);
               CardSlot cardSlot = allCardSlotInstances[cardInstance.instanceId];
               if (cardHolder == null || cardSlot == null)
               {
                    throw new ArgumentException("GFX ---- spawnCardInstance failed because it was called with unknown params, sourceTypeID: " + cardHolderID.ToString() + ", sourcePlayerID: " + playerID.ToString());
               }
               cardHolder.cardAdded(cardSlot);
          }

          public void wasRemovedFromList(CardInstance cardInstance, int cardHolderID, int playerID)
          {
               GwintCardHolder cardHolder = getCardHolder(cardHolderID, playerID);
               CardSlot cardSlot = allCardSlotInstances[cardInstance.instanceId];
               if (cardHolder == null || cardSlot == null)
               {
                    throw new ArgumentException("GFX ---- spawnCardInstance failed because it was called with unknown params, sourceTypeID: " + cardHolderID.ToString() + ", sourcePlayerID: " + playerID.ToString());
               }
               cardHolder.cardRemoved(cardSlot);
          }

          public void spawnCardInstance(CardInstance card, int cardHolderID, int playerID)
          {
               GwintCardHolder cardHolder = getCardHolder(cardHolderID, playerID);
               if (cardHolder == null)
               {
                    throw new ArgumentException("GFX ---- spawnCardInstance failed because it was called with unknown params, sourceTypeID: " + cardHolderID.ToString() + ", sourcePlayerID: " + playerID.ToString());
               }
               //CardSlot cardSlot = (CardSlot)(new _slotRendererRef());//new instance of card slot model?
               //handle images for card slot
               
               /*Uri uri;
               Image image = new Image();
               uri = new Uri("pack://application:,,,/Images/Cards/" + card.templateRef.imageLoc + ".jpg");
               image.Source = new BitmapImage(uri);*/
               //MainWindow_ViewModel.mSingleton.
               //cardImage = image;
               
               //CardSlot cardSlot = new CardSlot();//placeholder?
               MainWindow_ViewModel.mSingleton.Dispatcher.Invoke((Action)(() =>
               {
                    CardSlot cardSlotRef = MainWindow_ViewModel.mSingleton.getSlot(card);
               }));

               CardSlot cardSlot = MainWindow_ViewModel.mSingleton.ReferenceSlot;
               //CardSlot cardSlot = MainWindow_ViewModel.mSingleton.getSlot(card);
               
               //cardSlot.useContextMgr = false;//slot base class?
               cardSlot.instanceId = card.instanceId;
               cardSlot.cardState = CardSlot.STATE_BOARD;
               //mcGodCardHolder.addChild(cardSlot);//display?
               allCardSlotInstances[card.instanceId] = cardSlot;
               cardSlot.setCallbacksToCardInstance(card);
               cardHolder.spawnCard(cardSlot);
          }

          public void selectCard(CardSlot cardSlot)
          {
               selectCardInstance(cardSlot.cardInstance);
          }

          public void selectCardHolder(int cardHolderID, int playerID)
          {
               selectCardHolderAdv(getCardHolder(cardHolderID, playerID));
          }

          public void selectCardHolderAdv(GwintCardHolder cardHolder)
          {
               if (cardHolder == null)
               {
                    return;
               }
               int index = allRenderers.IndexOf(cardHolder);
               if (index > -1)
               {
                    selectedIndex = index;
                    if (cardHolder.selectedCardIdx == -1)
                    {
                         cardHolder.selectedCardIdx = 0;
                    }
               }
          }
          
          public int selectedIndex
          {
               get { return _selectedIndex; }
               set { _selectedIndex = value; }
          }//inherited from slot base?

          public void selectCardInstance(CardInstance cardInstance)
          {
               GwintCardHolder cardHolder = null;
               if (cardInstance != null)
               {
                    cardHolder = getCardHolder(cardInstance.inList, cardInstance.listsPlayer);
                    if (cardHolder != null)
                    {
                         selectCardHolderAdv(cardHolder);
                         cardHolder.selectCardInstance(cardInstance);
                    }
                    else
                    {
                         throw new ArgumentException("GFX [ERROR] - tried to select card with no matching card holder on board! list: " + cardInstance.inList + ", listsPlayer: " + cardInstance.listsPlayer);
                    }
               }
               else
               {
                    throw new ArgumentException("GFX [ERROR] - tried to select card slot with unknown card instance. Should not happen in this context: " + cardInstance);
               }
          }

          public void activateAllHolders(bool isSelectable)
          {
               foreach (GwintCardHolder cardHolder in allRenderers)
               {
                    //Console.WriteLine("boards renderer: activate all holders not implemented yet!");
                    /*//TO DO
                    cardHolder.selectable = isSelectable;
                    cardHolder.disableNavigation = false;
                    cardHolder.cardSelectionEnabled = true;
                    cardHolder.alwaysHighlight = false;
                     */
               }
          }

          public GwintCardHolder getSelectedCardHolder()
          {
               if (selectedIndex == -1)
               {
                    return null;
               }
               //return (GwintCardHolder)getSelectedRenderer();
               //TO DO get selected renderer?
               return allRenderers[0];
          }

          public CardSlot getSelectedCard()
          {
               GwintCardHolder cardHolder = getSelectedCardHolder();
               if (cardHolder == null)
               {
                    return null;
               }
               return cardHolder.getSelectedCardSlot();
          }

          public void activateHoldersForCard(CardInstance cardInstance, bool isActive = false)
          {
               GwintCardHolder cardHolder = null;
               bool isAvailable = false;
               List<CardInstance> creaturesList = new List<CardInstance>();
               int counter = 0;
               while (counter < allRenderers.Count)
               {
                    cardHolder = allRenderers[counter];
                    isAvailable = false;
                    if (cardInstance.templateRef.hasEffect(CardTemplate.CardEffect_UnsummonDummy) && cardHolder.playerID == cardInstance.owningPlayer && (cardHolder.cardHolderID == CardManager.CARD_LIST_LOC_MELEE || cardHolder.cardHolderID == CardManager.CARD_LIST_LOC_RANGED || cardHolder.cardHolderID == CardManager.CARD_LIST_LOC_SEIGE) && cardHolder.playerID == cardInstance.owningPlayer)
                    {
                         creaturesList = new List<CardInstance>();
                         CardManager.getInstance().getAllCreaturesNonHero(cardHolder.cardHolderID, cardHolder.playerID, creaturesList);
                         isAvailable = creaturesList.Count > 0;
                    }
                    else
                    {
                         cardHolder.cardSelectionEnabled = false;
                         if (cardInstance.canBePlacedInSlot(cardHolder.cardHolderID, cardHolder.playerID))
                         {
                              isAvailable = true;
                         }
                    }
                    Console.WriteLine("GFX ----- Analyzing slot for placement, valid: " + isAvailable + ", for slot: " + cardHolder);
                    cardHolder.selectable = isAvailable;
                    cardHolder.alwaysHighlight = isAvailable;
                    if (isAvailable && isActive)
                    {
                         selectedIndex = counter;
                         isActive = false;
                    }
                    ++counter;
               }
          }

          public void returnToDeck(CardInstance card)
          {
               GwintCardHolder cardHolder = null;
               CardSlot cardSlot = allCardSlotInstances[card.instanceId];
               if (cardSlot != null)
               {
                    cardHolder = getCardHolder(CardManager.CARD_LIST_LOC_DECK, card.owningPlayer);
                    Console.WriteLine("board renderer: return to deck tween not implemented");
                    //CardTweenManager.getInstance().tweenTo(cardSlot, cardHolder.x + CardSlot.CARD_BOARD_WIDTH / 2, cardHolder.y + CardSlot.CARD_BOARD_HEIGHT / 2, onReturnToDeckEnded);
               }
          }

          public void clearAllCards()
          {
               mcP1Deck.clearAllCards();
               mcP2Deck.clearAllCards();
               mcP1Hand.clearAllCards();
               mcP2Hand.clearAllCards();
               mcP1Graveyard.clearAllCards();
               mcP2Graveyard.clearAllCards();
               mcP1Siege.clearAllCards();
               mcP2Siege.clearAllCards();
               mcP1Range.clearAllCards();
               mcP2Range.clearAllCards();
               mcP1Melee.clearAllCards();
               mcP2Melee.clearAllCards();
               mcP1SiegeModif.clearAllCards();
               mcP2SiegeModif.clearAllCards();
               mcP1RangeModif.clearAllCards();
               mcP2RangeModif.clearAllCards();
               mcP1MeleeModif.clearAllCards();
               mcP2MeleeModif.clearAllCards();
               mcP1LeaderHolder.clearAllCards();
               mcP2LeaderHolder.clearAllCards();
               /*while (mcGodCardHolder.numChildren > 0) 
               {
                   mcGodCardHolder.removeChildAt(0);
               }*/
          }

          protected void fillRenderersList()
          {
               allRenderers = new List<GwintCardHolder>();

               mcWeather = new GwintCardHolder { cardHolderID = 9, playerID = 2, uniqueID = 1 };
               mcP1LeaderHolder = new GwintCardHolder { cardHolderID = 10, playerID = 0, uniqueID = 2 };
               mcP2LeaderHolder = new GwintCardHolder { cardHolderID = 10, playerID = 1, uniqueID = 3 };
               mcP1Deck = new GwintCardHolder { cardHolderID = 0, playerID = 0, uniqueID = 4 };
               mcP2Deck = new GwintCardHolder { cardHolderID = 0, playerID = 1, uniqueID = 5 };
               mcP1Hand = new GwintCardHolder { cardHolderID = 1, playerID = 0, uniqueID = 6 };
               mcP2Hand = new GwintCardHolder { cardHolderID = 1, playerID = 1, uniqueID = 7 };
               mcP1Graveyard = new GwintCardHolder { cardHolderID = 2, playerID = 0, uniqueID = 8 };
               mcP2Graveyard = new GwintCardHolder { cardHolderID = 2, playerID = 1, uniqueID = 9 };
               mcP1Siege = new GwintCardHolder { cardHolderID = 3, playerID = 0, uniqueID = 10 };
               mcP2Siege = new GwintCardHolder { cardHolderID = 3, playerID = 1, uniqueID = 11 };
               mcP1Range = new GwintCardHolder { cardHolderID = 4, playerID = 0, uniqueID = 12 };
               mcP2Range = new GwintCardHolder { cardHolderID = 4, playerID = 1, uniqueID = 13 };
               mcP1Melee = new GwintCardHolder { cardHolderID = 5, playerID = 0, uniqueID = 14 };
               mcP2Melee = new GwintCardHolder { cardHolderID = 5, playerID = 1, uniqueID = 15 };
               mcP1SiegeModif = new GwintCardHolder { cardHolderID = 6, playerID = 0, uniqueID = 16 };
               mcP2SiegeModif = new GwintCardHolder { cardHolderID = 6, playerID = 1, uniqueID = 17 };
               mcP1RangeModif = new GwintCardHolder { cardHolderID = 7, playerID = 0, uniqueID = 18 };
               mcP2RangeModif = new GwintCardHolder { cardHolderID = 7, playerID = 1, uniqueID = 19 };
               mcP1MeleeModif = new GwintCardHolder { cardHolderID = 8, playerID = 0, uniqueID = 20 };
               mcP2MeleeModif = new GwintCardHolder { cardHolderID = 8, playerID = 1, uniqueID = 21 };

               allRenderers.Add(mcWeather);
               allRenderers.Add(mcP1LeaderHolder);
               allRenderers.Add(mcP2LeaderHolder);
               allRenderers.Add(mcP1Deck);
               allRenderers.Add(mcP2Deck);
               allRenderers.Add(mcP1Hand);
               allRenderers.Add(mcP2Hand);
               allRenderers.Add(mcP1Graveyard);
               allRenderers.Add(mcP2Graveyard);
               allRenderers.Add(mcP1Siege);
               allRenderers.Add(mcP2Siege);
               allRenderers.Add(mcP1Range);
               allRenderers.Add(mcP2Range);
               allRenderers.Add(mcP1Melee);
               allRenderers.Add(mcP2Melee);
               allRenderers.Add(mcP1SiegeModif);
               allRenderers.Add(mcP2SiegeModif);
               allRenderers.Add(mcP1RangeModif);
               allRenderers.Add(mcP2RangeModif);
               allRenderers.Add(mcP1MeleeModif);
               allRenderers.Add(mcP2MeleeModif);

               allRenderers.Sort(cardHolderSorter);
               
               //extends slot base
               /*int index = 0;
               while (index < allRenderers.Count)
               {
                    allRenderers[index].boardRendererRef = this;
                    _renderers.Add(allRenderers[index]);
                    ++index;
               }
               _renderersCount = allRenderers.Count;*/
          }

          protected int cardHolderSorter(GwintCardHolder cardHolder1, GwintCardHolder cardHolder2)
          {
               return cardHolder1.uniqueID - cardHolder2.uniqueID;
          }

          public static string getCardHolderByUniqueID(int uniqueID)
          {
               switch(uniqueID)
               {
                    case 1:
                         {
                              return "weather holder";
                         }
                    case 2:
                         {
                              return "player 1 leader holder";
                         }
                    case 3:
                         {
                              return "player 2/AI leader holder";
                         }
                    case 4:
                         {
                              return "player 1 deck holder";
                         }
                    case 5:
                         {
                              return "player 2/AI deck holder";
                         }
                    case 6:
                         {
                              return "player 1 hand holder";
                         }
                    case 7:
                         {
                              return "player 2/AI hand holder";
                         }
                    case 8:
                         {
                              return "player 1 graveyard holder";
                         }
                    case 9:
                         {
                              return "player 2/AI graveyard holder";
                         }
                    case 10:
                         {
                              return "player 1 siege holder";
                         }
                    case 11:
                         {
                              return "player 2/AI siege holder";
                         }
                    case 12:
                         {
                              return "player 1 range holder";
                         }
                    case 13:
                         {
                              return "player 2/AI range holder";
                         }
                    case 14:
                         {
                              return "player 1 melee holder";
                         }
                    case 15:
                         {
                              return "player 2/AI melee holder";
                         }
                    case 16:
                         {
                              return "player 1 siege modifier holder";
                         }
                    case 17:
                         {
                              return "player 2/AI siege modifier holder";
                         }
                    case 18:
                         {
                              return "player 1 range modifier holder";
                         }
                    case 19:
                         {
                              return "player 2/AI range modifier holder";
                         }
                    case 20:
                         {
                              return "player 1 melee modifier holder";
                         }
                    case 21:
                         {
                              return "player 2/AI melee modifier holder";
                         }
                    default:
                         {
                              throw new ArgumentException("unique ID unknown!");
                         }
               }
          }
     }
}
