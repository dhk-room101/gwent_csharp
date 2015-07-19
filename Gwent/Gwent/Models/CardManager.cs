/* movieclips/sounds/text */
//TO DO onFinishedMovingIntoHolder
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class CardManager
     {
          public const int PLAYER_INVALID = -1;
          public const int PLAYER_1 = 0;
          public const int PLAYER_2 = 1;
          public const int PLAYER_BOTH = 2;
          
          public const int CARD_LIST_LOC_INVALID = -1;
          public const int CARD_LIST_LOC_DECK = 0;
          public const int CARD_LIST_LOC_HAND = 1;
          public const int CARD_LIST_LOC_GRAVEYARD = 2;
          public const int CARD_LIST_LOC_SEIGE = 3;
          public const int CARD_LIST_LOC_RANGED = 4;
          public const int CARD_LIST_LOC_MELEE = 5;
          public const int CARD_LIST_LOC_SEIGEMODIFIERS = 6;
          public const int CARD_LIST_LOC_RANGEDMODIFIERS = 7;
          public const int CARD_LIST_LOC_MELEEMODIFIERS = 8;
          public const int CARD_LIST_LOC_WEATHERSLOT = 9;
          public const int CARD_LIST_LOC_LEADER = 10;
          public const String cardTemplatesLoaded = "CardManager.templates.received"; 
          
          public List<GwintPlayerRenderer> playerRenderers;
          public CardEffectManager cardEffectManager;
          public Dictionary<int, CardTemplate> _cardTemplates = null;
          private Dictionary<int, CardInstance> _cardInstances;
          protected static CardManager _instance;
          private SafeRandom random = new SafeRandom();
          private int lastInstanceID = 0;
          private int _heroDrawSoundsAllowed = -1;
          private int _normalDrawSoundsAllowed = -1;
          public bool cardTemplatesReceived = false;
          public GwintBoardRenderer boardRenderer;
          public GwintCardValues cardValues;
          public List<GwintDeck> playerDeckDefinitions;
          public List<int> currentPlayerScores;
          public List<GwintRoundResult> roundResults;

          private List<CardInstance> cardListWeather;
          private List<CardInstance>[] cardListLeader;
          private List<CardInstance>[] cardListGraveyard;
          private List<CardInstance>[] cardListHand;
          private List<CardInstance>[] cardListSeige;
          private List<CardInstance>[] cardListRanged;
          private List<CardInstance>[] cardListMelee;
          private List<CardInstance>[] cardListSeigeModifier;
          private List<CardInstance>[] cardListRangedModifier;
          private List<CardInstance>[] cardListMeleeModifier;

          public List<GwintDeck>[] completeDecks;

          public CardManager()
          {
               playerRenderers = new List<GwintPlayerRenderer>();
               cardEffectManager = new CardEffectManager();
               initializeLists();
               _cardTemplates = new Dictionary<int, CardTemplate>();
               _cardInstances = new Dictionary<int, CardInstance>();
               _instance = this;
          }

          private void initializeLists()
          {
               playerDeckDefinitions = new List<GwintDeck>();
               playerDeckDefinitions.Add(new GwintDeck());
               playerDeckDefinitions.Add(new GwintDeck());

               currentPlayerScores = new List<int>();
               currentPlayerScores.Add(0);
               currentPlayerScores.Add(0);

               cardListHand = new List<CardInstance>[2];
               cardListHand[0] = new List<CardInstance>();
               cardListHand[1] = new List<CardInstance>();

               cardListGraveyard = new List<CardInstance>[2];
               cardListGraveyard[0] = new List<CardInstance>();
               cardListGraveyard[1] = new List<CardInstance>();

               cardListSeige = new List<CardInstance>[2];
               cardListSeige[0] = new List<CardInstance>();
               cardListSeige[1] = new List<CardInstance>();

               cardListRanged = new List<CardInstance>[2];
               cardListRanged[0] = new List<CardInstance>();
               cardListRanged[1] = new List<CardInstance>();

               cardListMelee = new List<CardInstance>[2];
               cardListMelee[0] = new List<CardInstance>();
               cardListMelee[1] = new List<CardInstance>();

               cardListSeigeModifier = new List<CardInstance>[2];
               cardListSeigeModifier[0] = new List<CardInstance>();
               cardListSeigeModifier[1] = new List<CardInstance>();

               cardListRangedModifier = new List<CardInstance>[2];
               cardListRangedModifier[0] = new List<CardInstance>();
               cardListRangedModifier[1] = new List<CardInstance>();

               cardListMeleeModifier = new List<CardInstance>[2];
               cardListMeleeModifier[0] = new List<CardInstance>();
               cardListMeleeModifier[1] = new List<CardInstance>();

               cardListLeader = new List<CardInstance>[2];
               cardListLeader[0] = new List<CardInstance>();
               cardListLeader[1] = new List<CardInstance>();

               cardListWeather = new List<CardInstance>();

               roundResults = new List<GwintRoundResult>();
               roundResults.Add(new GwintRoundResult());
               roundResults.Add(new GwintRoundResult());
               roundResults.Add(new GwintRoundResult());
          }

          public static CardManager getInstance()
          {
               if (_instance == null)
               {
                    _instance = new CardManager();
               }
               return _instance;
          }

          public List<CardInstance> getCardInstanceList(int listID, int player)
          {
               switch (listID)
               {
                    case CARD_LIST_LOC_DECK:
                         {
                              return null;
                         }
                    case CARD_LIST_LOC_HAND:
                         {
                              if (player != PLAYER_INVALID)
                              {
                                   return cardListHand[player];
                              }
                              break;
                         }
                    case CARD_LIST_LOC_GRAVEYARD:
                         {
                              if (player != PLAYER_INVALID)
                              {
                                   return cardListGraveyard[player];
                              }
                              break;
                         }
                    case CARD_LIST_LOC_SEIGE:
                         {
                              if (player != PLAYER_INVALID)
                              {
                                   return cardListSeige[player];
                              }
                              break;
                         }
                    case CARD_LIST_LOC_RANGED:
                         {
                              if (player != PLAYER_INVALID)
                              {
                                   return cardListRanged[player];
                              }
                              break;
                         }
                    case CARD_LIST_LOC_MELEE:
                         {
                              if (player != PLAYER_INVALID)
                              {
                                   return cardListMelee[player];
                              }
                              break;
                         }
                    case CARD_LIST_LOC_SEIGEMODIFIERS:
                         {
                              if (player != PLAYER_INVALID)
                              {
                                   return cardListSeigeModifier[player];
                              }
                              break;
                         }
                    case CARD_LIST_LOC_RANGEDMODIFIERS:
                         {
                              if (player != PLAYER_INVALID)
                              {
                                   return cardListRangedModifier[player];
                              }
                              break;
                         }
                    case CARD_LIST_LOC_MELEEMODIFIERS:
                         {
                              if (player != PLAYER_INVALID)
                              {
                                   return cardListMeleeModifier[player];
                              }
                              break;
                         }
                    case CARD_LIST_LOC_WEATHERSLOT:
                         {
                              return cardListWeather;
                         }
                    case CARD_LIST_LOC_LEADER:
                         {
                              if (player != PLAYER_INVALID)
                              {
                                   return cardListLeader[player];
                              }
                              break;
                         }
               }
               Console.WriteLine("GFX [WARNING] - CardManager: failed to get card list with player: {0} and listID: {1}", player, listID);
               return null;
          }

          public CardLeaderInstance getCardLeader(int player)
          {
               List<CardInstance> leader = CardManager.getInstance().getCardInstanceList(CardManager.CARD_LIST_LOC_LEADER, player);
               if (leader.Count < 1)
               {
                    return null;
               }
               Console.WriteLine("player {2}: converting to leader {0}, {1}", leader[0].templateId, leader[0].templateRef.title, player);
               return convertToLeader(leader[0]);
          }

          public CardLeaderInstance convertToLeader(CardInstance instance)
          {
               CardLeaderInstance result = new CardLeaderInstance();
               result.effectedByCardsRefList = instance.effectedByCardsRefList;
               result.effectingCardsRefList = instance.effectingCardsRefList;
               result.inList = instance.inList;
               result.instanceId = instance.instanceId;
               result.lastListApplied = instance.lastListApplied;
               result.lastListPlayerApplied = instance.lastListPlayerApplied;
               result.listsPlayer = instance.listsPlayer;
               result.owningPlayer = instance.owningPlayer;
               result.playSummonedFX = instance.playSummonedFX;
               result.powerChangeCallback = instance.powerChangeCallback;
               result.templateId = instance.templateId;
               result.templateRef = instance.templateRef;
               return result;
          }

          public CardTemplate getCardTemplate(int id)
          {
               return _cardTemplates[id];
          }

          public List<CardInstance> getScorchTargets(int type = 7, int playerID = 2)
          {
               int counter = 0;
               List<CardInstance> cardInstanceList = null;
               CardInstance cardInstance = null;
               List<CardInstance> result = new List<CardInstance>();
               int power = 0;
               int id = PLAYER_1;
               while (id < PLAYER_2 + 1)
               {
                    if (id == playerID || playerID == PLAYER_BOTH)
                    {
                         if ((type & CardTemplate.CardType_Melee) != CardTemplate.CardType_None)
                         {
                              cardInstanceList = getCardInstanceList(CARD_LIST_LOC_MELEE, id);
                              counter = 0;
                              while (counter < cardInstanceList.Count)
                              {
                                   cardInstance = cardInstanceList[counter];
                                   if (cardInstance.getTotalPower() >= power && !((cardInstance.templateRef.typeArray & type) == CardTemplate.CardType_None) && !cardInstance.templateRef.isType(CardTemplate.CardType_Hero))
                                   {
                                        if (cardInstance.getTotalPower() > power)
                                        {
                                             power = cardInstance.getTotalPower();
                                             result = new List<CardInstance>();//result.Count = 0;
                                             result.Add(cardInstance);
                                        }
                                        else
                                        {
                                             result.Add(cardInstance);
                                        }
                                   }
                                   ++counter;
                              }
                         }
                         if ((type & CardTemplate.CardType_Ranged) != CardTemplate.CardType_None)
                         {
                              cardInstanceList = getCardInstanceList(CARD_LIST_LOC_RANGED, id);
                              counter = 0;
                              while (counter < cardInstanceList.Count)
                              {
                                   cardInstance = cardInstanceList[counter];
                                   if (cardInstance.getTotalPower() >= power && !((cardInstance.templateRef.typeArray & type) == CardTemplate.CardType_None) && !cardInstance.templateRef.isType(CardTemplate.CardType_Hero))
                                   {
                                        if (cardInstance.getTotalPower() > power)
                                        {
                                             power = cardInstance.getTotalPower();
                                             result = new List<CardInstance>();//result.Count = 0;
                                             result.Add(cardInstance);
                                        }
                                        else
                                        {
                                             result.Add(cardInstance);
                                        }
                                   }
                                   ++counter;
                              }
                         }
                         if ((type & CardTemplate.CardType_Siege) != CardTemplate.CardType_None)
                         {
                              cardInstanceList = getCardInstanceList(CARD_LIST_LOC_SEIGE, id);
                              counter = 0;
                              while (counter < cardInstanceList.Count)
                              {
                                   cardInstance = cardInstanceList[counter];
                                   if (cardInstance.getTotalPower() >= power && !((cardInstance.templateRef.typeArray & type) == CardTemplate.CardType_None) && !cardInstance.templateRef.isType(CardTemplate.CardType_Hero))
                                   {
                                        if (cardInstance.getTotalPower() > power)
                                        {
                                             power = cardInstance.getTotalPower();
                                             result = new List<CardInstance>();//result.Count = 0;
                                             result.Add(cardInstance);
                                        }
                                        else
                                        {
                                             result.Add(cardInstance);
                                        }
                                   }
                                   ++counter;
                              }
                         }
                    }
                    ++id;
               }
               return result;
          }

          public int calculatePlayerScore(int type, int player)
          {
               int counter = 0;
               List<CardInstance> instance = getCardInstanceList(type, player);
               int score = 0;
               while (counter < instance.Count)
               {
                    score = score + instance[counter].getTotalPower();
                    ++counter;
               }
               return score;
          }

          public void GetRessurectionTargets(int playerID, List<CardInstance> cardInstanceList, bool recalculate)
          {
               CardInstance cardInstance = null;
               List<CardInstance> graveyardList = getCardInstanceList(CardManager.CARD_LIST_LOC_GRAVEYARD, playerID);
               int counter = 0;
               while (counter < graveyardList.Count)
               {
                    cardInstance = graveyardList[counter];
                    if (cardInstance.templateRef.isType(CardTemplate.CardType_Creature) && !cardInstance.templateRef.isType(CardTemplate.CardType_Hero))
                    {
                         if (recalculate)
                         {
                              cardInstance.recalculatePowerPotential(this);
                         }
                         cardInstanceList.Add(cardInstance);//needs return?
                    }
                    ++counter;
               }
          }

          public List<CardInstance> getCardsInHandWithEffect(int effectID, int playerID)
          {
               int counter = 0;
               CardInstance cardInstance = null;
               List<CardInstance> resultList = new List<CardInstance>();
               List<CardInstance> handList = getCardInstanceList(CARD_LIST_LOC_HAND, playerID);
               while (counter < handList.Count)
               {
                    cardInstance = handList[counter];
                    if (cardInstance.templateRef.hasEffect(effectID))
                    {
                         resultList.Add(cardInstance);
                    }
                    ++counter;
               }
               return resultList;
          }

          public int getWinningPlayer()
          {
               if (currentPlayerScores[PLAYER_1] > currentPlayerScores[PLAYER_2])
               {
                    return PLAYER_1;
               }
               if (currentPlayerScores[PLAYER_1] < currentPlayerScores[PLAYER_2])
               {
                    return PLAYER_2;
               }
               return PLAYER_INVALID;
          }

          public void recalculateScores()
          {
               int winner = getWinningPlayer();
               int siegeP2 = calculatePlayerScore(CARD_LIST_LOC_SEIGE, PLAYER_2);
               int rangeP2 = calculatePlayerScore(CARD_LIST_LOC_RANGED, PLAYER_2);
               int meleeP2 = calculatePlayerScore(CARD_LIST_LOC_MELEE, PLAYER_2);
               int meleeP1 = calculatePlayerScore(CARD_LIST_LOC_MELEE, PLAYER_1);
               int rangeP1 = calculatePlayerScore(CARD_LIST_LOC_RANGED, PLAYER_1);
               int siegeP1 = calculatePlayerScore(CARD_LIST_LOC_SEIGE, PLAYER_1);
               currentPlayerScores[PLAYER_1] = meleeP1 + rangeP1 + siegeP1;
               playerRenderers[PLAYER_1].score = currentPlayerScores[PLAYER_1];
               currentPlayerScores[PLAYER_2] = meleeP2 + rangeP2 + siegeP2;
               playerRenderers[PLAYER_2].score = currentPlayerScores[PLAYER_2];
               playerRenderers[PLAYER_1].setIsWinning(currentPlayerScores[PLAYER_1] > currentPlayerScores[PLAYER_2]);
               playerRenderers[PLAYER_2].setIsWinning(currentPlayerScores[PLAYER_2] > currentPlayerScores[PLAYER_1]);
               boardRenderer.updateRowScores(siegeP1, rangeP1, meleeP1, meleeP2, rangeP2, siegeP2);
               /*if (winner != getWinningPlayer())
               {
                    GwintGameMenu.mSingleton.playSound("gui_gwint_whose_winning_changed");
               }*/
          }

          public void getAllCreaturesNonHero(int listID, int playerID, List<CardInstance> creaturesList)
          {
               int counter = 0;
               CardInstance cardInstance = null;
               List<CardInstance> cardInstanceList = getCardInstanceList(listID, playerID);
               if (cardInstanceList == null)
               {
                    throw new ArgumentException("GFX [ERROR] - Failed to get card instance list for listID: " + listID + ", and playerIndex: " + playerID);
               }
               while (counter < cardInstanceList.Count)
               {
                    cardInstance = cardInstanceList[counter];
                    if (cardInstance.templateRef.isType(CardTemplate.CardType_Creature) && !cardInstance.templateRef.isType(CardTemplate.CardType_Hero))
                    {
                         creaturesList.Add(cardInstance);
                    }
                    ++counter;
               }
          }

          public bool checkIfHigherOrLower(CardInstance card1, CardInstance card2, bool isBetter)
          {
               if (isBetter)
               {
                    if (card1.getTotalPower() > card2.getTotalPower())
                    {
                         return true;
                    }
                    return false;
               }
               if (card1.getTotalPower() < card2.getTotalPower())
               {
                    return true;
               }
               return false;
          }

          public bool isBetterMatch(CardInstance card1, CardInstance card2, int playerID, bool boolParameter1, bool boolParameter2, bool boolParameter3)
          {
               bool isBetter = false;//needs tweaking?
               bool spy1 = card1.templateRef.isType(CardTemplate.CardType_Spy);
               bool spy2 = card2.templateRef.isType(CardTemplate.CardType_Spy);
               bool scorch1 = card1.templateRef.hasEffect(CardTemplate.CardEffect_MeleeScorch);
               bool scorch2 = card2.templateRef.hasEffect(CardTemplate.CardEffect_MeleeScorch);
               bool nurse1 = card1.templateRef.hasEffect(CardTemplate.CardEffect_Nurse);
               bool nurse2 = card2.templateRef.hasEffect(CardTemplate.CardEffect_Nurse);
               int owningPlayer = playerID != CardManager.PLAYER_1 ? CardManager.PLAYER_1 : CardManager.PLAYER_2;
               int score = calculatePlayerScore(CARD_LIST_LOC_MELEE, owningPlayer);
               if (boolParameter2 || boolParameter3)
               {
                    isBetter = boolParameter1 != true ? true : false;
               }
               if (spy1 || spy2)
               {
                    if (!spy2)
                    {
                         return true;
                    }
                    if (boolParameter2 && spy1 && checkIfHigherOrLower(card1, card2, isBetter))
                    {
                         return true;
                    }
                    if (spy1 && checkIfHigherOrLower(card1, card2, boolParameter1))
                    {
                         return true;
                    }
                    return false;
               }
               if (scorch1 || scorch2)
               {
                    if (scorch2)
                    {
                         return false;
                    }
                    if (score >= 10)
                    {
                         return true;
                    }
                    return false;
               }
               if (nurse1 || nurse2)
               {
                    if (!nurse2)
                    {
                         return true;
                    }
                    if (boolParameter3 && nurse1 && checkIfHigherOrLower(card1, card2, isBetter))
                    {
                         return true;
                    }
                    if (nurse1 && checkIfHigherOrLower(card1, card2, true))
                    {
                         return true;
                    }
                    return false;
               }
               if (checkIfHigherOrLower(card1, card2, boolParameter1))
               {
                    return true;
               }
               return false;
          }

          public bool isBetterMatchForGrave(CardInstance card1, CardInstance card2, int playerID, bool boolParameter1, bool boolParameter2, bool boolParameter3)
          {
               bool isBetter = false;
               bool spy1 = card1.templateRef.isType(CardTemplate.CardType_Spy);
               bool spy2 = card2.templateRef.isType(CardTemplate.CardType_Spy);
               bool meleeScorch1 = card1.templateRef.hasEffect(CardTemplate.CardEffect_MeleeScorch);
               bool meleeScorch2 = card2.templateRef.hasEffect(CardTemplate.CardEffect_MeleeScorch);
               bool nurse1 = card1.templateRef.hasEffect(CardTemplate.CardEffect_Nurse);
               bool nurse2 = card2.templateRef.hasEffect(CardTemplate.CardEffect_Nurse);
               int _player = playerID != CardManager.PLAYER_1 ? CardManager.PLAYER_1 : CardManager.PLAYER_2;
               int score = calculatePlayerScore(CARD_LIST_LOC_MELEE, _player);
               if (boolParameter2 || boolParameter3)
               {
                    isBetter = boolParameter1 != true ? true : false;
               }
               if (spy1 || spy2)
               {
                    if (!spy2)
                    {
                         return true;
                    }
                    if (boolParameter2 && spy1 && checkIfHigherOrLower(card1, card2, isBetter))
                    {
                         return true;
                    }
                    if (spy1 && checkIfHigherOrLower(card1, card2, boolParameter1))
                    {
                         return true;
                    }
                    return false;
               }
               if (meleeScorch1 || meleeScorch2)
               {
                    if (meleeScorch2)
                    {
                         return false;
                    }
                    if (score >= 10)
                    {
                         return true;
                    }
                    return false;
               }
               if (nurse1 || nurse2)
               {
                    if (!nurse2)
                    {
                         return true;
                    }
                    if (boolParameter3 && nurse1 && checkIfHigherOrLower(card1, card2, isBetter))
                    {
                         return true;
                    }
                    if (nurse1 && checkIfHigherOrLower(card1, card2, true))
                    {
                         return true;
                    }
                    return false;
               }
               if (checkIfHigherOrLower(card1, card2, boolParameter1))
               {
                    return true;
               }
               return false;
          }

          public CardInstance getHigherOrLowerValueTargetCardOnBoard(CardInstance currentCard, int playerID, bool boolParameter1 = true, bool boolParameter2 = false, bool boolParameter3 = false)
          {
               int counter = 0;
               CardInstance result = null;
               CardInstance targetCard = null;
               List<CardInstance> creaturesList = new List<CardInstance>();
               getAllCreaturesNonHero(CARD_LIST_LOC_MELEE, playerID, creaturesList);
               getAllCreaturesNonHero(CARD_LIST_LOC_RANGED, playerID, creaturesList);
               getAllCreaturesNonHero(CARD_LIST_LOC_SEIGE, playerID, creaturesList);
               while (counter < creaturesList.Count)
               {
                    targetCard = creaturesList[counter];
                    if (currentCard.canBeCastOn(targetCard))
                    {
                         if (result != null)
                         {
                              if (isBetterMatch(targetCard, result, playerID, boolParameter1, boolParameter2, boolParameter3))
                              {
                                   result = targetCard;
                              }
                         }
                         else
                         {
                              result = targetCard;
                         }
                    }
                    ++counter;
               }
               return result;
          }

          public CardInstance getCardInstance(int cardInstance)
          {
               return _cardInstances[cardInstance];
          }

          public CardInstance getFirstCardInHandWithEffect(int effectID, int playerID)
          {
               int counter = 0;
               CardInstance cardInstance = null;
               List<CardInstance> cardInstanceList = getCardInstanceList(CARD_LIST_LOC_HAND, playerID);
               while (counter < cardInstanceList.Count)
               {
                    cardInstance = cardInstanceList[counter];
                    if (cardInstance.templateRef.hasEffect(effectID))
                    {
                         return cardInstance;
                    }
                    ++counter;
               }
               return null;
          }

          public List<CardInstance> getCardsInSlotIdWithEffect(int effectID, int playerID, int listID = -1)
          {
               CardInstance cardInstance = null;
               int counter = 0;
               List<CardInstance> definedList = null;
               List<CardInstance> dynamicList = null;
               List<CardInstance> resultList = new List<CardInstance>();
               if (listID != -1)
               {
                    dynamicList = getCardInstanceList(listID, playerID);
                    counter = 0;
                    while (counter < dynamicList.Count)
                    {
                         cardInstance = dynamicList[counter];
                         if (cardInstance.templateRef.hasEffect(effectID))
                         {
                              resultList.Add(cardInstance);
                         }
                         ++counter;
                    }
               }
               else
               {
                    definedList = getCardInstanceList(CARD_LIST_LOC_MELEE, playerID);
                    counter = 0;
                    while (counter < definedList.Count)
                    {
                         cardInstance = definedList[counter];
                         if (cardInstance.templateRef.hasEffect(effectID))
                         {
                              resultList.Add(cardInstance);
                         }
                         ++counter;
                    }
                    definedList = getCardInstanceList(CARD_LIST_LOC_RANGED, playerID);
                    counter = 0;
                    while (counter < definedList.Count)
                    {
                         cardInstance = definedList[counter];
                         if (cardInstance.templateRef.hasEffect(effectID))
                         {
                              resultList.Add(cardInstance);
                         }
                         ++counter;
                    }
                    definedList = getCardInstanceList(CARD_LIST_LOC_SEIGE, playerID);
                    counter = 0;
                    while (counter < definedList.Count)
                    {
                         cardInstance = definedList[counter];
                         if (cardInstance.templateRef.hasEffect(effectID))
                         {
                              resultList.Add(cardInstance);
                         }
                         ++counter;
                    }
               }
               return resultList;
          }

          public void CalculateCardPowerPotentials()
          {
               int counter = 0;
               List<CardInstance> cardInstanceList = null;

               cardInstanceList = getCardInstanceList(CARD_LIST_LOC_HAND, PLAYER_1);
               while (counter < cardInstanceList.Count)
               {
                    cardInstanceList[counter].recalculatePowerPotential(this);
                    ++counter;
               }
               cardInstanceList = getCardInstanceList(CARD_LIST_LOC_HAND, PLAYER_2);
               counter = 0;
               while (counter < cardInstanceList.Count)
               {
                    cardInstanceList[counter].recalculatePowerPotential(this);
                    ++counter;
               }
          }

          public List<CardInstance> getAllCreaturesInHand(int playerID)
          {
               int counter = 0;
               CardInstance cardInstance = null;
               List<CardInstance> resultList = new List<CardInstance>();
               List<CardInstance> cardInstanceList = getCardInstanceList(CARD_LIST_LOC_HAND, playerID);
               while (counter < cardInstanceList.Count)
               {
                    cardInstance = cardInstanceList[counter];
                    if (cardInstance.templateRef.isType(CardTemplate.CardType_Creature))
                    {
                         resultList.Add(cardInstance);
                    }
                    ++counter;
               }
               return resultList;
          }

          public List<CardInstance> getAllCreatures(int playerID)
          {
               int counter = 0;
               List<CardInstance> list = null;
               List<CardInstance> resultList = new List<CardInstance>();
               list = getCardInstanceList(CARD_LIST_LOC_MELEE, playerID);
               counter = 0;
               while (counter < list.Count)
               {
                    resultList.Add(list[counter]);
                    ++counter;
               }
               list = getCardInstanceList(CARD_LIST_LOC_RANGED, playerID);
               counter = 0;
               while (counter < list.Count)
               {
                    resultList.Add(list[counter]);
                    ++counter;
               }
               list = getCardInstanceList(CARD_LIST_LOC_SEIGE, playerID);
               counter = 0;
               while (counter < list.Count)
               {
                    resultList.Add(list[counter]);
                    ++counter;
               }
               return resultList;
          }

          //needs tweaking?
          public CardAndComboPoints getHigherOrLowerValueCardFromTargetGraveyard(int playerID, bool boolParameter1 = true, bool boolParameter2 = false, bool boolParameter3 = false, bool boolParameter4 = false)
          {
               int counter1 = 0;
               CardInstance playerCandidateGraveyard1 = null;
               CardInstance playerCardGraveyard = null;
               CardInstance playerCandidateGraveyard2 = null;
               CardInstance playerNurseGraveyard = null;
               CardInstance playerSpyGraveyard = null;
               CardInstance playerMeleeScorchGraveyard = null;
               int playerPower = 0;
               int opponentPower = 0;
               int opponentTotalPower = 0;
               List<CardInstance> opponentCreaturesGraveyardList = null;
               int _opponent = 0;
               CardInstance opponentCandidateGraveyard1 = null;
               CardInstance opponentCardGraveyard = null;
               CardInstance opponentCandidateGraveyard2 = null;
               CardInstance opponentSpyGraveyard = null;
               List<CardInstance> opponentUpdatedGraveyardList = null;
               CardInstance opponentMeleeScorchGraveyard = null;
               int counter2 = 0;
               List<CardInstance> playerCreaturesGraveyardList = new List<CardInstance>();
               getAllCreaturesNonHero(CARD_LIST_LOC_GRAVEYARD, playerID, playerCreaturesGraveyardList);
               List<CardInstance> playerUpdatedGraveyardList = new List<CardInstance>();
               int playerTotalPower = 0;
               CardAndComboPoints cardAndComboPoints = new CardAndComboPoints();
               counter1 = 0;
               while (counter1 < playerCreaturesGraveyardList.Count)
               {
                    playerCardGraveyard = playerCreaturesGraveyardList[counter1];
                    if (playerCandidateGraveyard1 == null)
                    {
                         playerCandidateGraveyard1 = playerCardGraveyard;
                    }
                    if (playerCardGraveyard.templateRef.isType(CardTemplate.CardType_Spy))
                    {
                         if (playerSpyGraveyard != null)
                         {
                              if (playerSpyGraveyard != null && isBetterMatchForGrave(playerCardGraveyard, playerSpyGraveyard, playerID, boolParameter1, boolParameter2, boolParameter3))
                              {
                                   playerSpyGraveyard = playerCardGraveyard;
                              }
                         }
                         else
                         {
                              playerSpyGraveyard = playerCardGraveyard;
                         }
                    }
                    else if (playerCardGraveyard.templateRef.hasEffect(CardTemplate.CardEffect_MeleeScorch))
                    {
                         playerMeleeScorchGraveyard = playerCardGraveyard;
                    }
                    else if (playerCardGraveyard.templateRef.hasEffect(CardTemplate.CardEffect_Nurse))
                    {
                         if (playerNurseGraveyard != null)
                         {
                              if (playerNurseGraveyard != null && isBetterMatchForGrave(playerCardGraveyard, playerNurseGraveyard, playerID, boolParameter1, boolParameter2, boolParameter3))
                              {
                                   playerNurseGraveyard = playerCardGraveyard;
                              }
                         }
                         else
                         {
                              playerNurseGraveyard = playerCardGraveyard;
                         }
                         playerUpdatedGraveyardList.Add(playerCardGraveyard);
                    }
                    else if (playerCandidateGraveyard2 != null)
                    {
                         if (playerCandidateGraveyard2 != null && isBetterMatchForGrave(playerCardGraveyard, playerCandidateGraveyard2, playerID, boolParameter1, boolParameter2, boolParameter3))
                         {
                              playerCandidateGraveyard2 = playerCardGraveyard;
                         }
                    }
                    else
                    {
                         playerCandidateGraveyard2 = playerCardGraveyard;
                    }
                    ++counter1;
               }

               //Opponent
               if (boolParameter4 && playerUpdatedGraveyardList.Count > 0)
               {
                    opponentCreaturesGraveyardList = new List<CardInstance>();
                    _opponent = playerID != CardManager.PLAYER_1 ? CardManager.PLAYER_1 : CardManager.PLAYER_2;
                    getAllCreaturesNonHero(CARD_LIST_LOC_GRAVEYARD, _opponent, opponentCreaturesGraveyardList);
                    opponentUpdatedGraveyardList = new List<CardInstance>();
                    counter1 = 0;
                    while (counter1 < opponentCreaturesGraveyardList.Count)
                    {
                         opponentCardGraveyard = opponentCreaturesGraveyardList[counter1];
                         if (opponentCandidateGraveyard1 == null)
                         {
                              opponentCandidateGraveyard1 = opponentCardGraveyard;
                         }
                         if (opponentCardGraveyard.templateRef.isType(CardTemplate.CardType_Spy))
                         {
                              if (opponentSpyGraveyard == null)
                              {
                                   opponentSpyGraveyard = opponentCardGraveyard;
                              }
                              if (opponentSpyGraveyard != null && isBetterMatchForGrave(opponentCardGraveyard, opponentSpyGraveyard, _opponent, boolParameter1, boolParameter2, boolParameter3))
                              {
                                   opponentSpyGraveyard = opponentCardGraveyard;
                              }
                         }
                         else if (opponentCardGraveyard.templateRef.hasEffect(CardTemplate.CardEffect_MeleeScorch))
                         {
                              opponentMeleeScorchGraveyard = opponentCardGraveyard;
                         }
                         else if (opponentCardGraveyard.templateRef.hasEffect(CardTemplate.CardEffect_Nurse))
                         {
                              opponentUpdatedGraveyardList.Add(opponentCardGraveyard);
                         }
                         else if (opponentCandidateGraveyard2 != null)
                         {
                              if (opponentCandidateGraveyard2 != null && isBetterMatchForGrave(opponentCardGraveyard, opponentCandidateGraveyard2, _opponent, boolParameter1, boolParameter2, boolParameter3))
                              {
                                   opponentCandidateGraveyard2 = opponentCardGraveyard;
                              }
                         }
                         else
                         {
                              opponentCandidateGraveyard2 = opponentCardGraveyard;
                         }
                         ++counter1;
                    }
                    if (opponentSpyGraveyard != null)
                    {
                         opponentTotalPower = Math.Max(0, 10 - opponentSpyGraveyard.getTotalPower());
                         opponentPower = opponentTotalPower;
                    }
                    else if (opponentMeleeScorchGraveyard != null)
                    {
                         opponentTotalPower = opponentMeleeScorchGraveyard.getTotalPower();
                    }
                    else if (opponentCandidateGraveyard2 != null)
                    {
                         opponentTotalPower = opponentCandidateGraveyard2.getTotalPower();
                    }
                    if (opponentUpdatedGraveyardList != null)
                    {
                         counter2 = 0;
                         while (counter2 < opponentUpdatedGraveyardList.Count)
                         {
                              opponentTotalPower = opponentTotalPower + opponentUpdatedGraveyardList[counter2].getTotalPower();
                              ++counter2;
                         }
                    }
                    if (playerNurseGraveyard != null)
                    {
                         opponentTotalPower = opponentTotalPower + playerNurseGraveyard.getTotalPower();
                    }
               }
               if (playerSpyGraveyard != null)
               {
                    playerTotalPower = Math.Max(0, 10 - playerSpyGraveyard.getTotalPower());
                    playerPower = playerTotalPower;
                    playerCandidateGraveyard1 = playerSpyGraveyard;
               }
               else if (playerMeleeScorchGraveyard != null)
               {
                    playerTotalPower = playerMeleeScorchGraveyard.getTotalPower();
                    playerCandidateGraveyard1 = playerMeleeScorchGraveyard;
               }
               else if (playerCandidateGraveyard2 != null)
               {
                    playerTotalPower = playerCandidateGraveyard2.getTotalPower();
                    playerCandidateGraveyard1 = playerCandidateGraveyard2;
               }
               if (!boolParameter4 && playerUpdatedGraveyardList != null)
               {
                    counter2 = 0;
                    while (counter2 < playerUpdatedGraveyardList.Count)
                    {
                         playerTotalPower = playerTotalPower + playerUpdatedGraveyardList[counter2].getTotalPower();
                         ++counter2;
                    }
               }
               else if (playerSpyGraveyard == null && playerMeleeScorchGraveyard == null && playerCandidateGraveyard2 == null && playerNurseGraveyard != null)
               {
                    playerTotalPower = playerNurseGraveyard.getTotalPower();
                    playerCandidateGraveyard1 = playerNurseGraveyard;
               }
               if (boolParameter4 && playerNurseGraveyard != null)
               {
                    if (opponentPower != 0 && playerPower != 0 && opponentTotalPower > playerTotalPower)
                    {
                         cardAndComboPoints.cardInstance = playerNurseGraveyard;
                         cardAndComboPoints.comboPoints = opponentTotalPower;
                    }
                    else if (playerPower != 0 && opponentPower != 0 || opponentPower > playerPower)
                    {
                         cardAndComboPoints.cardInstance = playerNurseGraveyard;
                         cardAndComboPoints.comboPoints = opponentTotalPower;
                    }
                    else
                    {
                         cardAndComboPoints.cardInstance = playerCandidateGraveyard1;
                         cardAndComboPoints.comboPoints = playerTotalPower;
                    }
               }
               else
               {
                    cardAndComboPoints.cardInstance = playerCandidateGraveyard1;
                    cardAndComboPoints.comboPoints = playerTotalPower;
               }
               return cardAndComboPoints;
          }

          public bool tryDrawSpecificCard(int playerID, int cardID)
          {
               CardInstance card = null;
               GwintDeck deck = playerDeckDefinitions[playerID];
               if (deck.tryDrawSpecificCard(cardID))
               {
                    card = spawnCardInstance(cardID, playerID);
                    addCardInstanceToList(card, CARD_LIST_LOC_HAND, playerID);
                    Console.WriteLine("GFX - Player {0} drew the following Card: {1}", playerID, card);
                    return true;
               }
               return false;
          }

          public bool tryDrawAndPlaySpecificCard_Weather(int playerID, int cardID)
          {
               CardInstance card = null;
               GwintDeck deck = playerDeckDefinitions[playerID];
               if (deck.tryDrawSpecificCard(cardID))
               {
                    card = spawnCardInstance(cardID, playerID);
                    addCardInstanceToList(card, CARD_LIST_LOC_WEATHERSLOT, CardManager.PLAYER_INVALID);
                    Console.WriteLine("GFX - Player ", playerID, " drew the following Card:", card);
                    return true;
               }
               return false;
          }
          
          public CardInstance spawnCardInstance(int templateID, int playerID, int listID = -1)
          {
               CardInstance spawn = null;
               lastInstanceID = lastInstanceID + 1;
               if (templateID >= 1000)
               {
                    spawn = new CardLeaderInstance();
               }
               else
               {
                    spawn = new CardInstance();
               }
               int _listID = listID;
               if (_listID == CARD_LIST_LOC_INVALID)
               {
                    _listID = CARD_LIST_LOC_DECK;
               }
               spawn.templateId = templateID;
               spawn.templateRef = getCardTemplate(templateID);
               spawn.owningPlayer = playerID;
               spawn.instanceId = lastInstanceID;
               _cardInstances[spawn.instanceId] = spawn;
               spawn.finalizeSetup();
               if (boardRenderer != null)
               {
                    boardRenderer.spawnCardInstance(spawn, _listID, playerID);
               }
               if (listID == CARD_LIST_LOC_INVALID)
               {
                    addCardInstanceToList(spawn, CARD_LIST_LOC_HAND, playerID);
               }
               return spawn;
          }

          public void addCardInstanceToList(CardInstance cardInstance, int listID, int listsPlayer)
          {
               removeCardInstanceFromItsList(cardInstance);
               cardInstance.inList = listID;
               cardInstance.listsPlayer = listsPlayer;
               List<CardInstance> cardInstanceList = getCardInstanceList(listID, listsPlayer);
               Console.WriteLine("GFX ====== Adding card with instance ID: " + cardInstance.instanceId + ", to List ID: " + listIDToString(listID) + ", for player: " + listsPlayer);
               cardInstanceList.Add(cardInstance);
               if (boardRenderer != null)
               {
                    boardRenderer.wasAddedToList(cardInstance, listID, listsPlayer);
               }
               recalculateScores();
               if (listID == CARD_LIST_LOC_HAND)
               {
                    playerRenderers[listsPlayer].numCardsInHand = cardInstanceList.Count;
               }
          }

          public void addCardInstanceIDToList(int cardID, int listID, int playerID)
          {
               CardInstance card = getCardInstance(cardID);
               if (card != null)
               {
                    addCardInstanceToList(card, listID, playerID);
               }
          }

          public void removeCardInstanceFromItsList(CardInstance card)
          {
               removeCardInstanceFromList(card, card.inList, card.listsPlayer);
          }

          public void removeCardInstanceFromList(CardInstance card, int listID, int playerID)
          {
               List<CardInstance> cardInstanceList = null;
               int index = 0;
               if (card.inList != CARD_LIST_LOC_INVALID)
               {
                    card.inList = CARD_LIST_LOC_INVALID;
                    card.listsPlayer = PLAYER_INVALID;
                    cardInstanceList = getCardInstanceList(listID, playerID);
                    if (cardInstanceList == null)
                    {
                         throw new ArgumentException("GFX - Tried to remove from unknown listID:" + listID + ", and player:" + playerID + ", the following card: " + card);
                    }
                    index = cardInstanceList.IndexOf(card);
                    if (index < 0 || index >= cardInstanceList.Count)
                    {
                         throw new ArgumentException("GFX - tried to remove card instance from a list that does not contain it: " + listID + ", " + playerID + ", " + card);
                    }
                    cardInstanceList.RemoveRange(index, 1);
                    if (boardRenderer != null)
                    {
                         boardRenderer.wasRemovedFromList(card, listID, playerID);
                    }
                    recalculateScores();
                    if (listID == CARD_LIST_LOC_HAND)
                    {
                         playerRenderers[playerID].numCardsInHand = cardInstanceList.Count;
                    }
               }
          }

          public string listIDToString(int listID)
          {
               switch (listID)
               {
                    case CARD_LIST_LOC_DECK:
                         {
                              return "DECK";
                         }
                    case CARD_LIST_LOC_HAND:
                         {
                              return "HAND";
                         }
                    case CARD_LIST_LOC_GRAVEYARD:
                         {
                              return "GRAVEYARD";
                         }
                    case CARD_LIST_LOC_SEIGE:
                         {
                              return "SEIGE";
                         }
                    case CARD_LIST_LOC_RANGED:
                         {
                              return "RANGED";
                         }
                    case CARD_LIST_LOC_MELEE:
                         {
                              return "MELEE";
                         }
                    case CARD_LIST_LOC_SEIGEMODIFIERS:
                         {
                              return "SEIGEMODIFIERS";
                         }
                    case CARD_LIST_LOC_RANGEDMODIFIERS:
                         {
                              return "RANGEDMODIFIERS";
                         }
                    case CARD_LIST_LOC_MELEEMODIFIERS:
                         {
                              return "MELEEMODIFIERS";
                         }
                    case CARD_LIST_LOC_WEATHERSLOT:
                         {
                              return "WEATHER";
                         }
                    case CARD_LIST_LOC_LEADER:
                         {
                              return "LEADER";
                         }
                    case CARD_LIST_LOC_INVALID:
                    default:
                         {
                              return "INVALID";
                         }
               }
          }

          public void sendToGraveyardID(int cardID)
          {
               sendToGraveyard(getCardInstance(cardID));
          }

          public void sendToGraveyard(CardInstance card)
          {
               if (card != null)
               {
                    if (card.templateRef.isType(CardTemplate.CardType_Weather))
                    {
                         addCardInstanceToList(card, CARD_LIST_LOC_GRAVEYARD, card.owningPlayer);
                    }
                    else
                    {
                         addCardInstanceToList(card, CARD_LIST_LOC_GRAVEYARD, card.listsPlayer);
                    }
               }
          }

          public void replaceCardInstanceIDs(int sourceID, int destinationID)
          {
               replaceCardInstance(getCardInstance(sourceID), getCardInstance(destinationID));
          }

          public void replaceCardInstance(CardInstance source, CardInstance destination)
          {
               if (destination == null || source == null)
               {
                    return;
               }
               //GwintGameMenu.mSingleton.playSound("gui_gwint_dummy");
               int listID = destination.inList;
               int playerID = destination.listsPlayer;
               addCardInstanceToList(destination, CARD_LIST_LOC_HAND, destination.listsPlayer);
               addCardInstanceToList(source, listID, playerID);
          }

          public void applyCardEffectsID(int card)
          {
               applyCardEffects(getCardInstance(card));
          }

          public void applyCardEffects(CardInstance card)
          {
               if (card != null)
               {
                    card.updateEffectsApplied();
               }
          }

          public bool drawCards(int playerID, int quantity)
          {
               int counter = 0;
               _heroDrawSoundsAllowed = 1;
               _normalDrawSoundsAllowed = 1;
               while (counter < quantity)
               {
                    if (!drawCard(playerID))
                    {
                         return false;
                    }
                    ++counter;
               }
               _heroDrawSoundsAllowed = -1;
               _normalDrawSoundsAllowed = -1;
               return true;
          }

          public bool drawCard(int playerID)
          {
               int cardID = 0;
               CardInstance card = null;
               GwintDeck deck = playerDeckDefinitions[playerID];
               if (deck.cardIndicesInDeck.Count > 0)
               {
                    cardID = playerDeckDefinitions[playerID].drawCard();
                    card = spawnCardInstance(cardID, playerID);
                    addCardInstanceToList(card, CARD_LIST_LOC_HAND, playerID);
                    if (card.templateRef.isType(CardTemplate.CardType_Hero))
                    {
                         if (_heroDrawSoundsAllowed > 0)
                         {
                              --_heroDrawSoundsAllowed;
                              //GwintGameMenu.mSingleton.playSound("gui_gwint_hero_card_drawn");
                         }
                         else if (_heroDrawSoundsAllowed == -1)
                         {
                              //GwintGameMenu.mSingleton.playSound("gui_gwint_hero_card_drawn");
                         }
                    }
                    else if (_normalDrawSoundsAllowed > 0)
                    {
                         --_normalDrawSoundsAllowed;
                         //GwintGameMenu.mSingleton.playSound("gui_gwint_draw_card");
                    }
                    else if (_normalDrawSoundsAllowed == -1)
                    {
                         //GwintGameMenu.mSingleton.playSound("gui_gwint_draw_card");
                    }
                    Console.WriteLine("GFX - Player {0}, drew the following Card: {1}", playerID, card);
                    return true;
               }
               Console.WriteLine("GFX - Player {0}, has no more cards to draw!", playerID);
               return false;
          }

          public void spawnLeaders()
          {
               //should be CardLeaderInstance?
               int kingIndex = 0;
               CardInstance leader = null;
               CardInstance hand = null;
               List<int> leadersList;
               List<int> handList;

               //P1
               kingIndex = playerDeckDefinitions[PLAYER_1].selectedKingIndex;
               leadersList = new List<int>();
               handList = new List<int>();
               foreach (int cardID in playerDeckDefinitions[PLAYER_1].cardIndices)
               {
                    if (cardID >= kingIndex * 100 && cardID < (kingIndex * 100 + 5))
                    {
                         leadersList.Add(cardID);
                    }
                    else 
                    {
                         handList.Add(cardID);
                    }
               }
               if (leadersList.Count > 0)
               {
                    foreach (int leaderID in leadersList)
                    {
                         leader = spawnCardInstance(leaderID, PLAYER_1);
                         Console.WriteLine("leader found {0}, {1}", leader.templateId, leader.templateRef.title);
                         addCardInstanceToList(leader, CARD_LIST_LOC_LEADER, PLAYER_1);
                    }
               }
               if (handList.Count > 0)
               {
                    foreach (int handID in handList)
                    {
                         hand = spawnCardInstance(handID, PLAYER_1);
                         Console.WriteLine("hand card found {0}, {1}", hand.templateId, hand.templateRef.title);
                         addCardInstanceToList(hand, CARD_LIST_LOC_HAND, PLAYER_1);
                    }
               }

               //P2
               kingIndex = playerDeckDefinitions[PLAYER_2].selectedKingIndex;
               leadersList = new List<int>();
               handList = new List<int>();
               foreach (int cardID in playerDeckDefinitions[PLAYER_2].cardIndices)
               {
                    if (cardID >= kingIndex * 100 && cardID < (kingIndex * 100 + 5))
                    {
                         leadersList.Add(cardID);
                    }
                    else
                    {
                         handList.Add(cardID);
                    }
               }
               if (leadersList.Count > 0)
               {
                    foreach (int leaderID in leadersList)
                    {
                         leader = spawnCardInstance(leaderID, PLAYER_2);
                         Console.WriteLine("leader found {0}, {1}", leader.templateId, leader.templateRef.title);
                         addCardInstanceToList(leader, CARD_LIST_LOC_LEADER, PLAYER_2);
                    }
               }
               if (handList.Count > 0)
               {
                    foreach (int handID in handList)
                    {
                         hand = spawnCardInstance(handID, PLAYER_2);
                         Console.WriteLine("hand card found {0}, {1}", hand.templateId, hand.templateRef.title);
                         addCardInstanceToList(hand, CARD_LIST_LOC_HAND, PLAYER_2);
                    }
               }
          }

          public void traceRoundResults()
          {
               int counter = 0;
               Console.WriteLine("GFX -------------------------------- START TRACE ROUND RESULTS ----------------------------------");
               Console.WriteLine("GFX =============================================================================================");
               if (roundResults != null)
               {
                    counter = 0;
                    while (counter < roundResults.Count)
                    {
                         Console.WriteLine("GFX - " + roundResults[counter]);
                         ++counter;
                    }
               }
               else
               {
                    Console.WriteLine("GFX -------------- Round Results is empty!!! -------------");
               }
               Console.WriteLine("GFX =============================================================================================");
               Console.WriteLine("GFX ---------------------------------- END TRACE ROUND RESULTS ----------------------------------");
          }

          public void shuffleAndDrawCards()
          {
               int index = 0;
               GwintDeck deckP1 = playerDeckDefinitions[PLAYER_1];
               GwintDeck deckP2 = playerDeckDefinitions[PLAYER_2];
               CardLeaderInstance leaderP1 = getCardLeader(PLAYER_1);
               CardLeaderInstance leaderP2 = getCardLeader(PLAYER_2);
               if (deckP1.getDeckKingTemplate() == null || deckP2.getDeckKingTemplate() == null)
               {
                    throw new ArgumentException("GFX - Trying to shuffle and draw cards when one of the following decks is null:" + deckP1.getDeckKingTemplate() + ", " + deckP2.getDeckKingTemplate());
               }
               Console.WriteLine("GFX -#AI#------------------- DECK STRENGTH --------------------");
               Console.WriteLine("GFX -#AI#--- PLAYER 1:");
               deckP1.shuffleDeck(deckP2.originalStength());
               Console.WriteLine("GFX -#AI#--- PLAYER 2:");
               deckP2.shuffleDeck(deckP1.originalStength());
               Console.WriteLine("GFX -#AI#------------------------------------------------------");
               if (leaderP1.canBeUsed && leaderP1.templateRef.getFirstEffect() == CardTemplate.CardEffect_11th_card)
               {
                    leaderP1.canBeUsed = false;
                    index = 11;
               }
               else
               {
                    index = 10;
               }
               Console.WriteLine("card manager:shuffle and draw tutorial not implemented");
               /*if (GwintGameMenu.mSingleton.tutorialsOn)
               {
                    if (tryDrawSpecificCard(PLAYER_1, 3))
                    {
                         --index;
                    }
                    if (tryDrawSpecificCard(PLAYER_1, 5))
                    {
                         --index;
                    }
                    if (tryDrawSpecificCard(PLAYER_1, 150))
                    {
                         --index;
                    }
                    if (tryDrawSpecificCard(PLAYER_1, 115))
                    {
                         --index;
                    }
                    if (tryDrawSpecificCard(PLAYER_1, 135))
                    {
                         --index;
                    }
                    if (tryDrawSpecificCard(PLAYER_1, 111))
                    {
                         --index;
                    }
                    if (tryDrawSpecificCard(PLAYER_1, 145))
                    {
                         --index;
                    }
                    if (tryDrawSpecificCard(PLAYER_1, 113))
                    {
                         --index;
                    }
                    if (tryDrawSpecificCard(PLAYER_1, 114))
                    {
                         --index;
                    }
                    if (tryDrawSpecificCard(PLAYER_1, 107))
                    {
                         --index;
                    }
                    GwintGameMenu.mSingleton.playSound("gui_gwint_draw_card");
               }*/
               drawCards(PLAYER_1, index);
               List<CardInstance> playerCardsList = getCardInstanceList(CARD_LIST_LOC_HAND, PLAYER_1);
               playerCardsList.Sort(cardSorter);
               if (leaderP2.canBeUsed && leaderP2.templateRef.getFirstEffect() == CardTemplate.CardEffect_11th_card)
               {
                    leaderP2.canBeUsed = false;
                    index = 11;
               }
               else
               {
                    index = 10;
               }
               drawCards(PLAYER_2, index);
          }

          protected int cardSorter(CardInstance card1, CardInstance card2)
          {
               if (card1.templateId == card2.templateId)
               {
                    return 0;
               }
               CardTemplate templateC1 = card1.templateRef;
               CardTemplate templateC2 = card2.templateRef;
               int creatureC1 = templateC1.getCreatureType();
               int creatureC2 = templateC2.getCreatureType();
               if (creatureC1 == CardTemplate.CardType_None && creatureC2 == CardTemplate.CardType_None)
               {
                    return card1.templateId - card2.templateId;
               }
               if (creatureC1 == CardTemplate.CardType_None)
               {
                    return -1;
               }
               if (creatureC2 == CardTemplate.CardType_None)
               {
                    return 1;
               }
               if (templateC1.power != templateC2.power)
               {
                    return templateC1.power - templateC2.power;
               }
               return card1.templateId - card2.templateId;
          }

          public CardInstance mulliganCard(CardInstance card)
          {
               int cardID = 0;
               CardInstance spawnedCard = null;
               List<CardInstance> playerHandList = null;
               GwintDeck deck = null;
               if (card.owningPlayer >= 0 && card.owningPlayer < playerDeckDefinitions.Count)
               {
                    deck = playerDeckDefinitions[card.owningPlayer];
               }
               if (deck != null)
               {
                    deck.readdCard(card.templateId);
                    cardID = deck.drawCard();
                    if (cardID != CardInstance.INVALID_INSTANCE_ID)
                    {
                         spawnedCard = spawnCardInstance(cardID, card.owningPlayer);
                         if (spawnedCard != null)
                         {
                              addCardInstanceToList(spawnedCard, CARD_LIST_LOC_HAND, card.owningPlayer);
                              unspawnCardInstance(card);
                              if (spawnedCard.templateRef.isType(CardTemplate.CardType_Hero))
                              {
                                   //GwintGameMenu.mSingleton.playSound("gui_gwint_hero_card_drawn");
                              }
                              playerHandList = getCardInstanceList(CARD_LIST_LOC_HAND, PLAYER_1);
                              playerHandList.Sort(cardSorter);
                              return spawnedCard;
                         }
                    }
               }
               return null;
          }

          public void unspawnCardInstance(CardInstance card)
          {
               removeCardInstanceFromItsList(card);
               if (boardRenderer != null)
               {
                    boardRenderer.returnToDeck(card);
               }
               _cardInstances.Remove(card.instanceId);
          }

          public void updatePlayerLives()
          {
               int counter = 0;
               List<int> livesList = new List<int>();
               livesList.Add(2);
               livesList.Add(2);
               while (counter < roundResults.Count)
               {
                    if (roundResults[counter].played)
                    {
                         if (roundResults[counter].winningPlayer == PLAYER_1 || roundResults[counter].winningPlayer == PLAYER_INVALID)
                         {
                              livesList[PLAYER_2] = Math.Max(0, (livesList[PLAYER_2] - 1));
                         }
                         if (roundResults[counter].winningPlayer == PLAYER_2 || roundResults[counter].winningPlayer == PLAYER_INVALID)
                         {
                              livesList[PLAYER_1] = Math.Max(0, (livesList[PLAYER_1] - 1));
                         }
                    }
                    else
                    {
                         break;
                    }
                    ++counter;
               }
               playerRenderers[PLAYER_1].setPlayerLives(livesList[PLAYER_1]);
               playerRenderers[PLAYER_2].setPlayerLives(livesList[PLAYER_2]);
          }

          public void clearBoard(bool clear)
          {
               int playerID = 0;
               CardInstance creatureCard = null;
               CardInstance weatherCard = null;
               while (cardListWeather.Count > 0)
               {
                    weatherCard = cardListWeather[0];
                    addCardInstanceToList(weatherCard, CARD_LIST_LOC_GRAVEYARD, weatherCard.owningPlayer);
               }
               playerID = PLAYER_1;
               while (playerID <= PLAYER_2)
               {
                    if (clear)
                    {
                         creatureCard = chooseCreatureToExclude(playerID);
                    }
                    sendListToGraveyard(CARD_LIST_LOC_MELEE, playerID, creatureCard);
                    sendListToGraveyard(CARD_LIST_LOC_RANGED, playerID, creatureCard);
                    sendListToGraveyard(CARD_LIST_LOC_SEIGE, playerID, creatureCard);
                    sendListToGraveyard(CARD_LIST_LOC_MELEEMODIFIERS, playerID, creatureCard);
                    sendListToGraveyard(CARD_LIST_LOC_RANGEDMODIFIERS, playerID, creatureCard);
                    sendListToGraveyard(CARD_LIST_LOC_SEIGEMODIFIERS, playerID, creatureCard);
                    ++playerID;
               }
          }

          private void sendListToGraveyard(int listID, int playerID, CardInstance card)
          {
               CardInstance _cardInstance = null;
               List<CardInstance> cardsList = getCardInstanceList(listID, playerID);
               int counter = 0;
               while (cardsList.Count > counter)
               {
                    _cardInstance = cardsList[counter];
                    if (_cardInstance == card)
                    {
                         ++counter;
                         continue;
                    }
                    if (playerID == -1)
                    {
                         addCardInstanceToList(_cardInstance, CARD_LIST_LOC_GRAVEYARD, _cardInstance.owningPlayer);
                         continue;
                    }
                    addCardInstanceToList(_cardInstance, CARD_LIST_LOC_GRAVEYARD, _cardInstance.listsPlayer);
               }
          }

          public CardInstance chooseCreatureToExclude(int playerID)
          {
               List<CardInstance> creaturesList = null;
               int index = 0;
               if (playerDeckDefinitions[playerID].getDeckFaction() == CardTemplate.FactionId_No_Mans_Land)
               {
                    creaturesList = new List<CardInstance>();
                    getAllCreaturesNonHero(CARD_LIST_LOC_MELEE, playerID, creaturesList);
                    getAllCreaturesNonHero(CARD_LIST_LOC_RANGED, playerID, creaturesList);
                    getAllCreaturesNonHero(CARD_LIST_LOC_SEIGE, playerID, creaturesList);
                    if (creaturesList.Count > 0)
                    {
                         index = (int)Math.Min(Math.Floor(random.NextDouble() * creaturesList.Count), (creaturesList.Count - 1));
                         return creaturesList[index];
                    }
               }
               return null;
          }

          public void reset()
          {
               boardRenderer.clearAllCards();

               _cardInstances = new Dictionary<int, CardInstance>();

               cardListHand = new List<CardInstance>[2];
               cardListHand[0] = new List<CardInstance>();
               cardListHand[1] = new List<CardInstance>();

               cardListGraveyard = new List<CardInstance>[2];
               cardListGraveyard[0] = new List<CardInstance>();
               cardListGraveyard[1] = new List<CardInstance>();

               cardListSeige = new List<CardInstance>[2];
               cardListSeige[0] = new List<CardInstance>();
               cardListSeige[1] = new List<CardInstance>();

               cardListRanged = new List<CardInstance>[2];
               cardListRanged[0] = new List<CardInstance>();
               cardListRanged[1] = new List<CardInstance>();

               cardListMelee = new List<CardInstance>[2];
               cardListMelee[0] = new List<CardInstance>();
               cardListMelee[1] = new List<CardInstance>();

               cardListSeigeModifier = new List<CardInstance>[2];
               cardListSeigeModifier[0] = new List<CardInstance>();
               cardListSeigeModifier[1] = new List<CardInstance>();

               cardListRangedModifier = new List<CardInstance>[2];
               cardListRangedModifier[0] = new List<CardInstance>();
               cardListRangedModifier[1] = new List<CardInstance>();

               cardListMeleeModifier = new List<CardInstance>[2];
               cardListMeleeModifier[0] = new List<CardInstance>();
               cardListMeleeModifier[1] = new List<CardInstance>();

               cardListLeader = new List<CardInstance>[2];
               cardListLeader[0] = new List<CardInstance>();
               cardListLeader[1] = new List<CardInstance>();

               cardListWeather = new List<CardInstance>();

               roundResults[0].reset();
               roundResults[1].reset();
               roundResults[2].reset();
               
               playerRenderers[0].reset();
               playerRenderers[1].reset();
               
               cardEffectManager.flushAllEffects();
               
               recalculateScores();
          }
     }
}
