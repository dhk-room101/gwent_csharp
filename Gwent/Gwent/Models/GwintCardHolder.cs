/* movieclips/sounds/text */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Gwent.Models
{
     public class GwintCardHolder
     {
          //public var mcHighlight:flash.display.MovieClip;
          //public var mcSelected:flash.display.MovieClip;

          protected int _cardHolderID = -1;
          protected int _playerID = -1;
          protected int _paddingX = 3;
          protected int _paddingY = 5;
          protected int _uniqueID = 0;
          public GwintBoardRenderer boardRendererRef;
          public Storyboard mcStatus;
          public List<CardSlot> cardSlotsList;
          protected bool _selected;
          protected bool _selectable;//inherited from slot base
          protected int _selectedCardIdx = -1;
          protected int centerX;
          protected bool _disableNavigation;
          protected bool _cardSelectionEnabled = true;
          protected bool _alwaysHighlight = false;
          private CardSlot _lastSelectedCard;

          public GwintCardHolder()
          {
               cardSlotsList = new List<CardSlot>();
          }

          public int cardHolderID
          {
               get
               {
                    return _cardHolderID;
               }
               set
               {
                    _cardHolderID = value;
               }
          }

          public int playerID
          {
               get
               {
                    return _playerID;
               }
               set
               {
                    _playerID = value;
               }
          }

          //visual update?
          public void updateLeaderStatus(bool isLeader)
          {
               CardSlot cardSlot = null;
               if (cardSlotsList.Count > 0)
               {
                    cardSlot = cardSlotsList[0];
               }
               if (cardSlot == null)
               {
                    return;
               }
               CardLeaderInstance cardLeader = (CardLeaderInstance)cardSlot.cardInstance;
               if (cardLeader == null)
               {
                    return;
               }
               if (cardLeader.hasBeenUsed)
               {
                    //mcStatus.visible = false;
                    if (cardSlot != null)
                    {
                         //cardSlot.darkenIcon(0.3);
                    }
               }
               else
               {
                    if (cardSlot != null)
                    {
                         //cardSlot.filters = [];
                    }
                    if (mcStatus != null)
                    {
                         if (isLeader)
                         {
                              //mcStatus.visible = true;
                              if (cardLeader.canBeUsed)
                              {
                                   //mcStatus.gotoAndStop(1);
                              }
                              else
                              {
                                   //mcStatus.gotoAndStop(2);
                              }
                         }
                         else
                         {
                              //mcStatus.visible = false;
                         }
                    }
               }
          }

          public void cardAdded(CardSlot argumentCardSlot)
          {
               CardSlot localCardSlot = null;
               int index = 0;
               if (selectedCardIdx != -1 && selectedCardIdx < cardSlotsList.Count)
               {
                    localCardSlot = cardSlotsList[selectedCardIdx];
               }
               cardSlotsList.Add(argumentCardSlot);
               cardSlotsList.Sort(cardSorter);
               if (localCardSlot != null)
               {
                    index = cardSlotsList.IndexOf(localCardSlot);
                    if (index != selectedCardIdx)
                    {
                         selectedCardIdx = index;
                    }
               }
               repositionAllCards();//display?
               argumentCardSlot.activeSelectionEnabled = selected && _cardSelectionEnabled;
               if (argumentCardSlot.selected)
               {
                    argumentCardSlot.selected = false;
               }
               updateWeatherEffects();
               //registerCard(argumentCardSlot);not needed?
          }

          public void cardRemoved(CardSlot cardSlot)
          {
               //unregisterCard(cardSlot);//not needed?
               int index = cardSlotsList.IndexOf(cardSlot);
               if (index != -1)
               {
                    cardSlotsList.RemoveRange(index, 1);
                    findCardSelection(index >= _selectedCardIdx);
               }
               repositionAllCards();
               updateWeatherEffects();
          }

          protected void findCardSelection(bool isSelected)//arguments not needed?
          {
               selectedCardIdx = Math.Max(0, Math.Min((cardSlotsList.Count - 1), _selectedCardIdx));
          }
          
          public int selectedCardIdx
          {
               get
               {
                    return _selectedCardIdx;
               }

               set
               {
                    if (value == -1 && _lastSelectedCard == null)
                    {
                         return;
                    }
                    if (!(_lastSelectedCard == null) && cardSlotsList.IndexOf(_lastSelectedCard) != -1)
                    {
                         if (cardSlotsList[value] == _lastSelectedCard)
                         {
                              if (!_lastSelectedCard.selected)
                              {
                                   _lastSelectedCard.selected = true;
                              }
                              return;
                         }
                         _lastSelectedCard.selected = false;
                    }
                    if (value < 0 || value >= cardSlotsList.Count)
                    {
                         value = -1;
                    }
                    /*else
                    {
                         value = value;//redundant
                    }*/
                    if (value != -1)
                    {
                         _selectedCardIdx = value;
                         _lastSelectedCard = cardSlotsList[_selectedCardIdx];
                         _lastSelectedCard.selected = true;
                         //event?
                         //dispatchEvent(new red.game.witcher3.events.GwintCardEvent(red.game.witcher3.events.GwintCardEvent.CARD_SELECTED, true, false, _lastSelectedCard, this));
                    }
                    updateDrawOrder();
               }
          }

          private void updateDrawOrder()
          {
               throw new NotImplementedException();
               //overlapping?
          }

          protected int cardSorter(CardSlot slot1, CardSlot slot2)
          {
               CardInstance card1 = slot1.cardInstance;
               CardInstance card2 = slot2.cardInstance;
               if (card1.templateId == card2.templateId)
               {
                    return 0;
               }
               int type1 = card1.templateRef.getCreatureType();
               int type2 = card2.templateRef.getCreatureType();
               if (type1 == CardTemplate.CardType_None && type2 == CardTemplate.CardType_None)
               {
                    return card1.templateId - card2.templateId;
               }
               if (type1 == CardTemplate.CardType_None)
               {
                    return -1;
               }
               if (type2 == CardTemplate.CardType_None)
               {
                    return 1;
               }
               if (card1.templateRef.power != card2.templateRef.power)
               {
                    return card1.templateRef.power - card2.templateRef.power;
               }
               return card1.templateId - card2.templateId;
          }
          
          public bool selected
          {
               get
               {
                    return _selected;
               }

               set
               {
                    _selected = value;
               }
          }

          public bool selectable
          {
               get { return _selectable; }
               set { _selectable = value; }
          }

          public bool alwaysHighlight
          {
               get { return _alwaysHighlight; }
               set { _alwaysHighlight = value; }
          }

          protected void updateWeatherEffects()
          {
               bool melee = false;
               bool ranged = false;
               bool siege = false;
               int counter = 0;
               CardSlot cardSlot = null;
               CardFXManager cardFXManager = null;
               if (boardRendererRef != null && cardHolderID == CardManager.CARD_LIST_LOC_WEATHERSLOT)
               {
                    melee = false;
                    ranged = false;
                    siege = false;
                    counter = 0;
                    while (counter < cardSlotsList.Count)
                    {
                         cardSlot = cardSlotsList[counter];
                         int effectID = cardSlot.cardInstance.templateRef.getFirstEffect();
                         switch (effectID)
                         {
                              case CardTemplate.CardEffect_Melee:
                                   {
                                        melee = true;
                                        break;
                                   }
                              case CardTemplate.CardEffect_Ranged:
                                   {
                                        ranged = true;
                                        break;
                                   }
                              case CardTemplate.CardEffect_Siege:
                                   {
                                        siege = true;
                                        break;
                                   }
                         }
                         ++counter;
                    }
                    cardFXManager = CardFXManager.getInstance();
                    cardFXManager.ShowWeatherOngoing(CardManager.CARD_LIST_LOC_MELEE, melee);
                    cardFXManager.ShowWeatherOngoing(CardManager.CARD_LIST_LOC_RANGED, ranged);
                    cardFXManager.ShowWeatherOngoing(CardManager.CARD_LIST_LOC_SEIGE, siege);
               }
          }

          public void repositionAllCards()
          {
               if (cardHolderID == CardManager.CARD_LIST_LOC_MELEE || cardHolderID == CardManager.CARD_LIST_LOC_SEIGE || cardHolderID == CardManager.CARD_LIST_LOC_RANGED || cardHolderID == CardManager.CARD_LIST_LOC_HAND)
               {
                    Console.WriteLine("reposition standard true");
                    //repositionAllCards_Standard(true);
               }
               else
               {
                    Console.WriteLine("reposition standard false");
                    //repositionAllCards_Standard(false);
               }
               if (cardHolderID == CardManager.CARD_LIST_LOC_GRAVEYARD)
               {
                    Console.WriteLine("reposition graveyard");
                    //repositionAllCards_Graveyard();
               }
          }

          public void spawnCard(CardSlot cardSlot)
          {
               //throw new NotImplementedException();
               Console.WriteLine("Position the spawned card!");
          }

          public void selectCardInstance(CardInstance cardInstance)
          {
               int counter = 0;
               while (counter < cardSlotsList.Count)
               {
                    if (cardSlotsList[counter].cardInstance == cardInstance)
                    {
                         selectedCardIdx = counter;
                    }
                    ++counter;
               }
               throw new ArgumentException("GFX [ERROR] - tried to select card in slot: (" + cardHolderID + ", " + playerID + "), but could could not find reference to: " + cardInstance);
          }

          public CardSlot getSelectedCardSlot()
          {
               if (_selectedCardIdx >= 0 && _selectedCardIdx < cardSlotsList.Count)
               {
                    return cardSlotsList[_selectedCardIdx];
               }
               return null;
          }

          public bool cardSelectionEnabled
          {
               get
               {
                    return _cardSelectionEnabled;
               }

               set
               {
                    _cardSelectionEnabled = value;
                    updateCardSelectionAvailable();
               }
          }

          protected void updateCardSelectionAvailable()
          {
               int counter = 0;
               CardSlot cardSlot = null;
               while (counter < cardSlotsList.Count)
               {
                    cardSlot = cardSlotsList[counter];
                    if (cardSlot != null)
                    {
                         cardSlot.activeSelectionEnabled = _cardSelectionEnabled && selected;
                    }
                    ++counter;
               }
               updateDrawOrder();
          }

          public void clearAllCards()
          {
               cardSlotsList = new List<CardSlot>();
          }
     }
}
