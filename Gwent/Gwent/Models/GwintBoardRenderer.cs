/* movieclips/sounds/text */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

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
               CardSlot cardSlot = new CardSlot();//placeholder?
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
                    Console.WriteLine("boards renderer: activate all holders not implemented yet!");
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
     }
}
