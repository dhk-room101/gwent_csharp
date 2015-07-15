/* COMPLETED */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class GwintDeck
     {
          public string deckName;
          public List<int> cardIndices = new List<int>();//debug?
          public int selectedKingIndex;
          public int specialCard;
          public bool isUnlocked = false;
          public List<int> dynamicCardRequirements;
          public List<int> dynamicCards;
          public Action refreshCallback;
          public Action onCardChangedCallback;
          private GwintDeckRenderer _deckRenderer;
          public List<int> cardIndicesInDeck;
          private SafeRandom random = new SafeRandom();

          public GwintDeck()
          {
               cardIndicesInDeck = new List<int>();
          }

          public int drawCard()
          {
               if (cardIndicesInDeck.Count > 0)
               {
                    if (_deckRenderer != null)
                    {
                         _deckRenderer.cardCount = (cardIndicesInDeck.Count - 1);
                    }
                    //the next two lines are the equivalent of AS3 pop()
                    var i = cardIndicesInDeck[cardIndicesInDeck.Count - 1];
                    cardIndicesInDeck.RemoveAt(cardIndicesInDeck.Count - 1);
                    return i;
               }
               return CardInstance.INVALID_INSTANCE_ID;
          }

          public void getCardsInDeck(int type, int effect, List<int> cardsID)
          {
               CardTemplate cardTemplate = null;
               CardManager cardManager = CardManager.getInstance();
               int counter = 0;
               while (counter < cardIndicesInDeck.Count)
               {
                    cardTemplate = cardManager.getCardTemplate(cardIndicesInDeck[counter]);
                    if (cardTemplate != null)
                    {
                         if ((cardTemplate.isType(type) || type == CardTemplate.CardType_None) && 
                              (cardTemplate.hasEffect(effect) || effect == CardTemplate.CardEffect_None))
                         {
                              cardsID.Add(cardIndicesInDeck[counter]);
                         }
                    }
                    else
                    {
                         throw new ArgumentException("GFX [ERROR] - failed to fetch template reference for card ID: " + cardIndicesInDeck[counter]);
                    }
                    ++counter;
               }
          }//should return cardsID?

          public bool tryDrawSpecificCard(int index)
          {
               int counter = 0;
               while (counter < cardIndicesInDeck.Count)
               {
                    if (cardIndicesInDeck[counter] == index)
                    {
                         cardIndicesInDeck.RemoveRange(counter,1);
                         return true;
                    }
                    ++counter;
               }
               return false;
          }

          public int numCopiesLeft(int cardID)
          {
               int counter = 0;
               int copies = 0;
               while (counter < cardIndicesInDeck.Count)
               {
                    if (cardID == cardIndicesInDeck[counter])
                    {
                         ++copies;
                    }
                    ++counter;
               }
               return copies;
          }

          public int getDeckFaction()
          {
               CardTemplate kingTemplate = getDeckKingTemplate();
               if (kingTemplate != null)
               {
                    return kingTemplate.factionIdx;
               }
               return CardTemplate.FactionId_Error;
          }

          public string getFactionNameString()
          {
               int deckFaction = getDeckFaction();
               switch (deckFaction)
               {
                    case CardTemplate.FactionId_Nilfgaard:
                         {
                              return "[[gwint_faction_name_nilfgaard]]";
                         }
                    case CardTemplate.FactionId_No_Mans_Land:
                         {
                              return "[[gwint_faction_name_no_mans_land]]";
                         }
                    case CardTemplate.FactionId_Northern_Kingdom:
                         {
                              return "[[gwint_faction_name_northern_kingdom]]";
                         }
                    case CardTemplate.FactionId_Scoiatael:
                         {
                              return "[[gwint_faction_name_scoiatael]]";
                         }
               }
               return "Invalid Faction for Deck";
          }

          public string getFactionPerkString()
          {
               int deckFaction = getDeckFaction();
               switch (deckFaction)
               {
                    case CardTemplate.FactionId_Nilfgaard:
                         {
                              return "[[gwint_faction_ability_nilf]]";
                         }
                    case CardTemplate.FactionId_No_Mans_Land:
                         {
                              return "[[gwint_faction_ability_nml]]";
                         }
                    case CardTemplate.FactionId_Northern_Kingdom:
                         {
                              return "[[gwint_faction_ability_nr]]";
                         }
                    case CardTemplate.FactionId_Scoiatael:
                         {
                              return "[[gwint_faction_ability_scoia]]";
                         }
               }
               return "Invalid Faction, no perk";
          }

          public CardTemplate getDeckKingTemplate()
          {
               return CardManager.getInstance().getCardTemplate(selectedKingIndex);
          }

          public string toString()
          {
               int counter = 0;
               string indices = "";
               while (counter < cardIndices.Count)
               {
                    indices = indices + (cardIndices[counter].ToString() + " - ");
                    ++counter;
               }
               return "[GwintDeck] Name:" + deckName + ", selectedKing:" + selectedKingIndex.ToString() + ", indices:" + indices;
          }

          public int originalStength()
          {
               int counter = 0;
               CardTemplate cardTemplate = null;
               int strength = 0;
               CardManager cardManager = CardManager.getInstance();
               while (counter < cardIndices.Count)
               {
                    cardTemplate = cardManager.getCardTemplate(cardIndices[counter]);
                    if (cardTemplate.isType(CardTemplate.CardType_Creature))
                    {
                         strength = strength + cardTemplate.power;
                    }
                    int effectID = cardTemplate.getFirstEffect();
                    switch (effectID)
                    {
                         case CardTemplate.CardEffect_Melee:
                         case CardTemplate.CardEffect_Ranged:
                         case CardTemplate.CardEffect_Siege:
                         case CardTemplate.CardEffect_ClearSky:
                              {
                                   strength = strength + 2;
                                   break;
                              }
                         case CardTemplate.CardEffect_UnsummonDummy:
                              {
                                   strength = strength + 4;
                                   break;
                              }
                         case CardTemplate.CardEffect_Horn:
                              {
                                   strength = strength + 5;
                                   break;
                              }
                         case CardTemplate.CardEffect_Scorch:
                         case CardTemplate.CardEffect_MeleeScorch:
                              {
                                   strength = strength + 6;
                                   break;
                              }
                         case CardTemplate.CardEffect_SummonClones:
                              {
                                   strength = strength + 3;
                                   break;
                              }
                         case CardTemplate.CardEffect_ImproveNeighbours:
                              {
                                   strength = strength + 4;
                                   break;
                              }
                         case CardTemplate.CardEffect_Nurse:
                              {
                                   strength = strength + 4;
                                   break;
                              }
                         case CardTemplate.CardEffect_Draw2:
                              {
                                   strength = strength + 6;
                                   break;
                              }
                         case CardTemplate.CardEffect_SameTypeMorale:
                              {
                                   strength = strength + 4;
                                   break;
                              }
                    }
                    ++counter;
               }
               Console.WriteLine("GFX -#AI#----- > {0}", strength);
               return strength;
          }

          public void shuffleDeck(int difficulty)
          {
               int counter = 0;
               int randomIndex = 0;
               List<int> indicesList = new List<int>();
               while (counter < cardIndices.Count)
               {
                    indicesList.Add(cardIndices[counter]);
                    ++counter;
               }
               adjustDeckToDifficulty(difficulty, indicesList);
               cardIndicesInDeck = new List<int>();
               while (indicesList.Count > 0)
               {
                    randomIndex = (int)Math.Min(Math.Floor(random.NextDouble() * indicesList.Count), (indicesList.Count - 1));
                    cardIndicesInDeck.Add(indicesList[randomIndex]);
                    indicesList.RemoveRange(randomIndex, 1);
               }
               if (specialCard != -1)
               {
                    cardIndicesInDeck.Add(specialCard);
               }
               if (_deckRenderer != null)
               {
                    _deckRenderer.cardCount = cardIndicesInDeck.Count;
               }
          }

          private void adjustDeckToDifficulty(int requirement, List<int> deckIndices)
          {
               int counter = 0;
               if (dynamicCardRequirements.Count > 0 && dynamicCardRequirements.Count == dynamicCards.Count)
               {
                    Console.WriteLine("GFX -#AI#------------------- Deck balance --------------------");
                    counter = 0;
                    while (counter < dynamicCardRequirements.Count)
                    {
                         if (requirement >= dynamicCardRequirements[counter])
                         {
                              Console.WriteLine("GFX -#AI# Requirement [ " + dynamicCardRequirements[counter] + " ] - Adding card with id [ " + dynamicCards[counter] + "]");
                              deckIndices.Add(dynamicCards[counter]);
                         }
                         ++counter;
                    }
                    Console.WriteLine("GFX -#AI#-----------------------------------------------------");
               }
          }

          public void readdCard(int cardIndex)
          {
               cardIndicesInDeck.Insert(0,cardIndex);
          }

          public void triggerRefresh()
          {
               if (refreshCallback != null)
               {
                    refreshCallback();
               }
          }

          public GwintDeckRenderer DeckRenderer
          {
               set
               {
                    _deckRenderer = value;
                    _deckRenderer.factionString = getDeckKingTemplate().getFactionString();
                    _deckRenderer.cardCount = cardIndices.Count;
               }
          }
     }
}
