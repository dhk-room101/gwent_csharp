using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class CardInstance
     {
          public const int INVALID_INSTANCE_ID = -1;
          public int templateId;
          public int instanceId = -1;
          public int owningPlayer;
          public int inList;
          public int listsPlayer;
          public int lastListApplied;
          public int lastListPlayerApplied;
          public Action powerChangeCallback;
          public List<CardInstance> effectingCardsRefList;
          public List<CardInstance> effectedByCardsRefList;
          public bool playSummonedFX = false;
          public CardTemplate templateRef;
          protected CardTransaction _lastCalculatedPowerPotential;
          private Random random = new Random();
          
          public CardInstance()
          {
               owningPlayer = CardManager.PLAYER_INVALID;
               inList = CardManager.CARD_LIST_LOC_INVALID;
               listsPlayer = CardManager.PLAYER_INVALID;
               effectingCardsRefList = new List<CardInstance>();
               effectedByCardsRefList = new List<CardInstance>();
               lastListApplied = CardManager.CARD_LIST_LOC_INVALID;
               lastListPlayerApplied = CardManager.PLAYER_INVALID;
               _lastCalculatedPowerPotential = new CardTransaction();
          }

          public int notOwningPlayer
          {
               get { return owningPlayer != CardManager.PLAYER_1 ? CardManager.PLAYER_1 : CardManager.PLAYER_2; }
          }

          public int notListPlayer
          {
               get { return listsPlayer != CardManager.PLAYER_1 ? CardManager.PLAYER_1 : CardManager.PLAYER_2; }
          }

          public int getTotalPower(bool arg1 = false)
          {
               int counter = 0;
               CardInstance effectedBy = null;
               bool cardWeather = false;
               int hasHorn = 0;
               int improveNeighbors = 0;
               int sameTypeMorale = 0;
               if (!templateRef.isType(CardTemplate.CardType_Hero))
               {
                    counter = 0;
                    while (counter < effectedByCardsRefList.Count)
                    {
                         effectedBy = effectedByCardsRefList[counter];
                         if (effectedBy.templateRef.isType(CardTemplate.CardType_Weather))
                         {
                              cardWeather = true;
                         }
                         if (effectedBy.templateRef.hasEffect(CardTemplate.CardEffect_Horn) || effectedBy.templateRef.hasEffect(CardTemplate.CardEffect_Siege_Horn) || effectedBy.templateRef.hasEffect(CardTemplate.CardEffect_Range_Horn) || effectedBy.templateRef.hasEffect(CardTemplate.CardEffect_Melee_Horn))
                         {
                              ++hasHorn;
                         }
                         if (effectedBy.templateRef.hasEffect(CardTemplate.CardEffect_ImproveNeighbours))
                         {
                              ++improveNeighbors;
                         }
                         if (effectedBy.templateRef.hasEffect(CardTemplate.CardEffect_SameTypeMorale))
                         {
                              ++sameTypeMorale;
                         }
                         ++counter;
                    }
               }
               int boost = !arg1 && cardWeather ? Math.Min(1, CardManager.getInstance().getCardTemplate(templateId).power) : CardManager.getInstance().getCardTemplate(templateId).power;
               int power = 0;
               power = power + boost * sameTypeMorale + improveNeighbors;
               if (hasHorn > 0)
               {
                    power = power * 2 + boost;
               }
               return boost + power;//needs tweaking?
          }

          public bool canBeCastOn(CardInstance card)
          {
               if (templateRef.isType(CardTemplate.CardType_Hero) || 
                    card.templateRef.isType(CardTemplate.CardType_Hero))
               {
                    return false;
               }
               if (templateRef.hasEffect(CardTemplate.CardEffect_UnsummonDummy) && 
                    card.templateRef.isType(CardTemplate.CardType_Creature) && 
                    card.listsPlayer == listsPlayer && 
                    card.inList != CardManager.CARD_LIST_LOC_HAND && 
                    card.inList != CardManager.CARD_LIST_LOC_GRAVEYARD && 
                    card.inList == CardManager.CARD_LIST_LOC_LEADER)
               {
                    return true;
               }
               return false;
          }

          protected int powerChangeSorter(CardInstance card1, CardInstance card2)
          {
               if (card1.getOptimalTransaction().powerChangeResult == card2.getOptimalTransaction().powerChangeResult)
               {
                    return card1.getOptimalTransaction().strategicValue - card2.getOptimalTransaction().strategicValue;
               }
               return card1.getOptimalTransaction().powerChangeResult - card2.getOptimalTransaction().powerChangeResult;
          }

          public CardTransaction getOptimalTransaction()
          {
               return _lastCalculatedPowerPotential;
          }

          public virtual void recalculatePowerPotential(CardManager cardManager)
          {
               CardInstance cardInstance1 = null;
               List<CardInstance> cardInstanceList1 = null;
               List<CardInstance> cardInstanceList2 = null;
               CardInstance lastResurrectedCard = null;
               int powerChange = 0;
               int totalPower1 = 0;
               int totalPower2 = 0;
               int comparePower1 = 0;
               int comparePower2 = 0;
               int cardPower1 = 0;
               int clones1 = 0;
               int calculatedPower1 = 0;
               int totalPower3 = 0;
               int modifierMelee = 0;
               int modifierRange = 0;
               int modifierSiege = 0;
               int totalPower4 = 0;
               List<CardInstance> meleeScorchList1 = null;
               int totalPower5 = 0;
               int meleeScorchTotalPower1 = 0;
               List<CardInstance> resurrectionList1;
               bool canResurrect = false;
               int updatedPower1 = 0;
               int counter1 = 0;
               _lastCalculatedPowerPotential.powerChangeResult = 0;
               _lastCalculatedPowerPotential.strategicValue = 0;
               _lastCalculatedPowerPotential.sourceCardInstanceRef = this;
               List<CardInstance> weatherList = cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_WEATHERSLOT, CardManager.PLAYER_INVALID);
               CardInstance cardWeather = weatherList.Count > 0 ? weatherList[0] : null;
               int player = listsPlayer != CardManager.PLAYER_1 ? CardManager.PLAYER_1 : CardManager.PLAYER_2;
               bool handEffect = cardManager.getCardsInHandWithEffect(CardTemplate.CardEffect_Scorch, listsPlayer).Count > 0;
               List<CardInstance> handList = cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, listsPlayer);
               if (templateRef.isType(CardTemplate.CardType_Creature))
               {
                    _lastCalculatedPowerPotential.targetPlayerID = templateRef.isType(CardTemplate.CardType_Spy) ? player : listsPlayer;
                    if (templateRef.isType(CardTemplate.CardType_Melee))
                    {
                         _lastCalculatedPowerPotential.targetSlotID = CardManager.CARD_LIST_LOC_MELEE;
                    }
                    else if (templateRef.isType(CardTemplate.CardType_Ranged))
                    {
                         _lastCalculatedPowerPotential.targetSlotID = CardManager.CARD_LIST_LOC_RANGED;
                    }
                    else
                    {
                         _lastCalculatedPowerPotential.targetSlotID = CardManager.CARD_LIST_LOC_SEIGE;
                    }
                    cardInstanceList1 = cardManager.cardEffectManager.getEffectsForList(_lastCalculatedPowerPotential.targetSlotID, listsPlayer);
                    counter1 = 0;
                    while (counter1 < cardInstanceList1.Count)
                    {
                         cardInstance1 = cardInstanceList1[counter1];
                         if (cardInstance1 != this)
                         {
                              effectedByCardsRefList.Add(cardInstance1);
                         }
                         ++counter1;
                    }
                    totalPower1 = getTotalPower();
                    effectedByCardsRefList = new List<CardInstance>();
                    if (templateRef.isType(CardTemplate.CardType_RangedMelee))
                    {
                         cardInstanceList1 = cardManager.cardEffectManager.getEffectsForList(CardManager.CARD_LIST_LOC_RANGED, listsPlayer);
                         counter1 = 0;
                         while (counter1 < cardInstanceList1.Count)
                         {
                              cardInstance1 = cardInstanceList1[counter1];
                              if (cardInstance1 != this)
                              {
                                   effectedByCardsRefList.Add(cardInstance1);
                              }
                              ++counter1;
                         }
                         totalPower2 = getTotalPower();
                         effectedByCardsRefList = new List<CardInstance>();//reset?
                         if (templateRef.hasEffect(CardTemplate.CardEffect_ImproveNeighbours))
                         {
                              cardInstanceList2 = new List<CardInstance>();
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, CardManager.PLAYER_1, cardInstanceList2);
                              comparePower1 = totalPower1 + cardInstanceList2.Count;
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, CardManager.PLAYER_1, cardInstanceList2);
                              comparePower2 = totalPower2 + cardInstanceList2.Count;
                              if (comparePower2 > comparePower1 || comparePower2 == comparePower1 && random.NextDouble() < 0.5)
                              {
                                   totalPower1 = totalPower2;
                                   _lastCalculatedPowerPotential.targetSlotID = CardManager.CARD_LIST_LOC_RANGED;
                              }
                         }
                         else if (totalPower2 > totalPower1 || totalPower2 == totalPower1 && random.NextDouble() < 0.5)
                         {
                              totalPower1 = totalPower2;
                              _lastCalculatedPowerPotential.targetSlotID = CardManager.CARD_LIST_LOC_RANGED;
                         }
                    }
                    if (templateRef.hasEffect(CardTemplate.CardEffect_SameTypeMorale) || templateRef.hasEffect(CardTemplate.CardEffect_ImproveNeighbours))
                    {
                         cardInstanceList1 = new List<CardInstance>();
                         if (_lastCalculatedPowerPotential.targetSlotID == CardTemplate.CardType_Melee)
                         {
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, listsPlayer, cardInstanceList1);
                         }
                         if (_lastCalculatedPowerPotential.targetSlotID == CardTemplate.CardType_Ranged)
                         {
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, listsPlayer, cardInstanceList1);
                         }
                         if (_lastCalculatedPowerPotential.targetSlotID == CardTemplate.CardType_Siege)
                         {
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, listsPlayer, cardInstanceList1);
                         }
                         if (templateRef.hasEffect(CardTemplate.CardEffect_ImproveNeighbours))
                         {
                              _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult + cardInstanceList1.Count;
                         }
                         else
                         {
                              counter1 = 0;
                              while (counter1 < cardInstanceList1.Count)
                              {
                                   cardInstance1 = cardInstanceList1[counter1];
                                   if (cardInstance1.templateId == templateId)
                                   {
                                        cardPower1 = cardInstance1.getTotalPower();
                                        cardInstance1.effectedByCardsRefList.Add(this);
                                        _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult + (cardInstance1.getTotalPower() - cardPower1);
                                        cardInstance1.effectedByCardsRefList.RemoveAt(cardInstance1.effectedByCardsRefList.Count - 1);
                                   }
                                   ++counter1;
                              }
                         }
                    }
                    if (templateRef.hasEffect(CardTemplate.CardEffect_SummonClones))
                    {
                         clones1 = 0;
                         cardInstanceList1 = cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, listsPlayer);
                         counter1 = 0;
                         while (counter1 < cardInstanceList1.Count)
                         {
                              if (templateRef.summonFlags.IndexOf(cardInstanceList1[counter1].templateId) != -1)
                              {
                                   ++clones1;
                              }
                              ++counter1;
                         }
                         counter1 = 0;
                         while (counter1 < templateRef.summonFlags.Count)
                         {
                              clones1 = clones1 + cardManager.playerDeckDefinitions[listsPlayer].numCopiesLeft(templateRef.summonFlags[counter1]);
                              ++counter1;
                         }
                         _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult + clones1 * totalPower1;
                    }
                    if (templateRef.isType(CardTemplate.CardType_Spy))
                    {
                         _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult - totalPower1;
                    }
                    else
                    {
                         _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult + totalPower1;
                    }
               }
               if (templateRef.isType(CardTemplate.CardType_Weather))
               {
                    calculatedPower1 = 0;
                    totalPower3 = 0;
                    cardInstanceList2 = new List<CardInstance>();
                    if (templateRef.hasEffect(CardTemplate.CardEffect_ClearSky))
                    {
                         cardInstanceList2 = new List<CardInstance>();
                         cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, listsPlayer, cardInstanceList2);
                         cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, listsPlayer, cardInstanceList2);
                         cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, listsPlayer, cardInstanceList2);
                         counter1 = 0;
                         while (counter1 < cardInstanceList2.Count)
                         {
                              calculatedPower1 = calculatedPower1 + (cardInstanceList2[counter1].getTotalPower(true) - cardInstanceList2[counter1].getTotalPower());
                              ++counter1;
                         }
                         cardInstanceList2 = new List<CardInstance>();
                         cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, player, cardInstanceList2);
                         cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, player, cardInstanceList2);
                         cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, player, cardInstanceList2);
                         counter1 = 0;
                         while (counter1 < cardInstanceList2.Count)
                         {
                              calculatedPower1 = calculatedPower1 - (cardInstanceList2[counter1].getTotalPower(true) - cardInstanceList2[counter1].getTotalPower());
                              ++counter1;
                         }
                    }
                    else
                    {
                         if (templateRef.hasEffect(CardTemplate.CardEffect_Melee))
                         {
                              cardInstanceList2 = new List<CardInstance>();
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, listsPlayer, cardInstanceList2);
                              counter1 = 0;
                              while (counter1 < cardInstanceList2.Count)
                              {
                                   cardInstance1 = cardInstanceList2[counter1];
                                   totalPower3 = cardInstance1.getTotalPower();
                                   cardInstance1.effectedByCardsRefList.Add(this);
                                   calculatedPower1 = calculatedPower1 + (cardInstance1.getTotalPower() - totalPower3);
                                   cardInstance1.effectedByCardsRefList.RemoveAt(cardInstance1.effectedByCardsRefList.Count - 1);
                                   ++counter1;
                              }
                              cardInstanceList2 = new List<CardInstance>();
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, player, cardInstanceList2);
                              counter1 = 0;
                              while (counter1 < cardInstanceList2.Count)
                              {
                                   cardInstance1 = cardInstanceList2[counter1];
                                   totalPower3 = cardInstance1.getTotalPower();
                                   cardInstance1.effectedByCardsRefList.Add(this);
                                   calculatedPower1 = calculatedPower1 - (cardInstance1.getTotalPower() - totalPower3);
                                   cardInstance1.effectedByCardsRefList.RemoveAt(cardInstance1.effectedByCardsRefList.Count - 1);
                                   ++counter1;
                              }
                         }
                         if (templateRef.hasEffect(CardTemplate.CardEffect_Ranged))
                         {
                              cardInstanceList2 = new List<CardInstance>();
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, listsPlayer, cardInstanceList2);
                              counter1 = 0;
                              while (counter1 < cardInstanceList2.Count)
                              {
                                   cardInstance1 = cardInstanceList2[counter1];
                                   totalPower3 = cardInstance1.getTotalPower();
                                   cardInstance1.effectedByCardsRefList.Add(this);
                                   calculatedPower1 = calculatedPower1 + (cardInstance1.getTotalPower() - totalPower3);
                                   cardInstance1.effectedByCardsRefList.RemoveAt(cardInstance1.effectedByCardsRefList.Count - 1);
                                   ++counter1;
                              }
                              cardInstanceList2 = new List<CardInstance>();
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, player, cardInstanceList2);
                              counter1 = 0;
                              while (counter1 < cardInstanceList2.Count)
                              {
                                   cardInstance1 = cardInstanceList2[counter1];
                                   totalPower3 = cardInstance1.getTotalPower();
                                   cardInstance1.effectedByCardsRefList.Add(this);
                                   calculatedPower1 = calculatedPower1 - (cardInstance1.getTotalPower() - totalPower3);
                                   cardInstance1.effectedByCardsRefList.RemoveAt(cardInstance1.effectedByCardsRefList.Count - 1);
                                   ++counter1;
                              }
                         }
                         if (templateRef.hasEffect(CardTemplate.CardEffect_Siege))
                         {
                              cardInstanceList2 = new List<CardInstance>();
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, listsPlayer, cardInstanceList2);
                              counter1 = 0;
                              while (counter1 < cardInstanceList2.Count)
                              {
                                   cardInstance1 = cardInstanceList2[counter1];
                                   totalPower3 = cardInstance1.getTotalPower();
                                   cardInstance1.effectedByCardsRefList.Add(this);
                                   calculatedPower1 = calculatedPower1 + (cardInstance1.getTotalPower() - totalPower3);
                                   cardInstance1.effectedByCardsRefList.RemoveAt(cardInstance1.effectedByCardsRefList.Count - 1);
                                   ++counter1;
                              }
                              cardInstanceList2 = new List<CardInstance>();
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, player, cardInstanceList2);
                              counter1 = 0;
                              while (counter1 < cardInstanceList2.Count)
                              {
                                   cardInstance1 = cardInstanceList2[counter1];
                                   totalPower3 = cardInstance1.getTotalPower();
                                   cardInstance1.effectedByCardsRefList.Add(this);
                                   calculatedPower1 = calculatedPower1 - (cardInstance1.getTotalPower() - totalPower3);
                                   cardInstance1.effectedByCardsRefList.RemoveAt(cardInstance1.effectedByCardsRefList.Count - 1);
                                   ++counter1;
                              }
                         }
                    }
                    _lastCalculatedPowerPotential.powerChangeResult = calculatedPower1;
                    _lastCalculatedPowerPotential.strategicValue = Math.Max(0, cardManager.cardValues.weatherCardValue - calculatedPower1);
                    if (templateRef.hasEffect(CardTemplate.CardEffect_ClearSky))
                    {
                         _lastCalculatedPowerPotential.strategicValue = Math.Max(_lastCalculatedPowerPotential.strategicValue, 8);
                    }
                    _lastCalculatedPowerPotential.targetSlotID = CardManager.CARD_LIST_LOC_WEATHERSLOT;
                    _lastCalculatedPowerPotential.targetPlayerID = CardManager.PLAYER_INVALID;
               }
               List<CardInstance> scorchTargetsList1 = null;
               if (templateRef.hasEffect(CardTemplate.CardEffect_Scorch))
               {
                    scorchTargetsList1 = cardManager.getScorchTargets();
               }
               if (scorchTargetsList1 != null)
               {
                    counter1 = 0;
                    while (counter1 < scorchTargetsList1.Count)
                    {
                         cardInstance1 = scorchTargetsList1[counter1];
                         if (cardInstance1.listsPlayer != listsPlayer)
                         {
                              _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult + cardInstance1.getTotalPower();
                         }
                         else
                         {
                              _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult - cardInstance1.getTotalPower();
                         }
                         ++counter1;
                    }
                    if (_lastCalculatedPowerPotential.powerChangeResult < 0)
                    {
                         _lastCalculatedPowerPotential.strategicValue = -1;
                    }
                    else
                    {
                         _lastCalculatedPowerPotential.strategicValue = Math.Max(templateRef.GetBonusValue(), _lastCalculatedPowerPotential.powerChangeResult);
                    }
                    return;
               }
               if (templateRef.hasEffect(CardTemplate.CardEffect_UnsummonDummy))
               {
                    _lastCalculatedPowerPotential.targetCardInstanceRef = cardManager.getHigherOrLowerValueTargetCardOnBoard(this, listsPlayer, false, false, true);
                    if (_lastCalculatedPowerPotential.targetCardInstanceRef != null)
                    {
                         if (_lastCalculatedPowerPotential.targetCardInstanceRef.templateRef.isType(CardTemplate.CardType_Spy))
                         {
                              _lastCalculatedPowerPotential.powerChangeResult = 0;
                         }
                         else
                         {
                              _lastCalculatedPowerPotential.powerChangeResult = -_lastCalculatedPowerPotential.targetCardInstanceRef.getTotalPower();
                         }
                         if (cardManager.cardValues.unsummonCardValue + _lastCalculatedPowerPotential.powerChangeResult >= 0)
                         {
                              _lastCalculatedPowerPotential.strategicValue = Math.Abs(_lastCalculatedPowerPotential.powerChangeResult);
                         }
                         else
                         {
                              _lastCalculatedPowerPotential.strategicValue = cardManager.cardValues.unsummonCardValue + Math.Abs(_lastCalculatedPowerPotential.powerChangeResult);
                         }
                    }
                    else
                    {
                         _lastCalculatedPowerPotential.powerChangeResult = -1000;
                         _lastCalculatedPowerPotential.strategicValue = -1;
                    }
               }
               if (templateRef.isType(CardTemplate.CardType_Row_Modifier) && templateRef.hasEffect(CardTemplate.CardEffect_Horn))
               {
                    modifierMelee = -1;
                    modifierRange = -1;
                    modifierSiege = -1;
                    totalPower4 = 0;
                    cardInstanceList2 = new List<CardInstance>();
                    if (cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_MELEEMODIFIERS, listsPlayer).Count == 0)
                    {
                         cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, listsPlayer, cardInstanceList2);
                         modifierMelee = 0;
                         counter1 = 0;
                         while (counter1 < cardInstanceList2.Count)
                         {
                              cardInstance1 = cardInstanceList2[counter1];
                              totalPower4 = cardInstance1.getTotalPower();
                              cardInstance1.effectedByCardsRefList.Add(this);
                              modifierMelee = cardInstance1.getTotalPower() - totalPower4;
                              cardInstance1.effectedByCardsRefList.RemoveAt(cardInstance1.effectedByCardsRefList.Count - 1);
                              ++counter1;
                         }
                    }
                    cardInstanceList2 = new List<CardInstance>();
                    if (cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_RANGEDMODIFIERS, listsPlayer).Count == 0)
                    {
                         cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, listsPlayer, cardInstanceList2);
                         modifierRange = 0;
                         counter1 = 0;
                         while (counter1 < cardInstanceList2.Count)
                         {
                              cardInstance1 = cardInstanceList2[counter1];
                              totalPower4 = cardInstance1.getTotalPower();
                              cardInstance1.effectedByCardsRefList.Add(this);
                              modifierRange = cardInstance1.getTotalPower() - totalPower4;
                              cardInstance1.effectedByCardsRefList.RemoveAt(cardInstance1.effectedByCardsRefList.Count - 1);
                              ++counter1;
                         }
                    }
                    cardInstanceList2 = new List<CardInstance>();
                    if (cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_SEIGEMODIFIERS, listsPlayer).Count == 0)
                    {
                         cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, listsPlayer, cardInstanceList2);
                         modifierSiege = 0;
                         counter1 = 0;
                         while (counter1 < cardInstanceList2.Count)
                         {
                              cardInstance1 = cardInstanceList2[counter1];
                              cardInstance1.effectedByCardsRefList.Add(this);
                              modifierSiege = cardInstance1.getTotalPower() - totalPower4;
                              cardInstance1.effectedByCardsRefList.RemoveAt(cardInstance1.effectedByCardsRefList.Count - 1);
                              ++counter1;
                         }
                    }
                    if (modifierSiege == -1 && modifierMelee == -1 && modifierRange == -1)
                    {
                         _lastCalculatedPowerPotential.powerChangeResult = -1;
                         _lastCalculatedPowerPotential.strategicValue = -1;
                         return;
                    }
                    if (modifierMelee > modifierSiege && modifierMelee > modifierRange)
                    {
                         _lastCalculatedPowerPotential.powerChangeResult = modifierMelee;
                         _lastCalculatedPowerPotential.targetSlotID = CardManager.CARD_LIST_LOC_MELEEMODIFIERS;
                         _lastCalculatedPowerPotential.targetPlayerID = listsPlayer;
                    }
                    else if (modifierRange > modifierSiege)
                    {
                         _lastCalculatedPowerPotential.powerChangeResult = modifierRange;
                         _lastCalculatedPowerPotential.targetSlotID = CardManager.CARD_LIST_LOC_RANGEDMODIFIERS;
                         _lastCalculatedPowerPotential.targetPlayerID = listsPlayer;
                    }
                    else
                    {
                         _lastCalculatedPowerPotential.powerChangeResult = modifierSiege;
                         _lastCalculatedPowerPotential.targetSlotID = CardManager.CARD_LIST_LOC_SEIGEMODIFIERS;
                         _lastCalculatedPowerPotential.targetPlayerID = listsPlayer;
                    }
                    if (_lastCalculatedPowerPotential.powerChangeResult > cardManager.cardValues.hornCardValue)
                    {
                         _lastCalculatedPowerPotential.strategicValue = Math.Max(0, cardManager.cardValues.hornCardValue * 2 - _lastCalculatedPowerPotential.powerChangeResult);
                    }
                    else
                    {
                         _lastCalculatedPowerPotential.strategicValue = cardManager.cardValues.hornCardValue;
                    }
               }
               if (templateRef.hasEffect(CardTemplate.CardEffect_MeleeScorch))
               {
                    meleeScorchList1 = null;
                    meleeScorchList1 = cardManager.getScorchTargets(CardTemplate.CardType_Melee, notListPlayer);
                    if (!(meleeScorchList1.Count == 0) && cardManager.calculatePlayerScore(CardManager.CARD_LIST_LOC_MELEE, notListPlayer) >= 10)
                    {
                         counter1 = 0;
                         totalPower5 = 0;
                         meleeScorchTotalPower1 = 0;
                         counter1 = 0;
                         while (counter1 < meleeScorchList1.Count)
                         {
                              meleeScorchTotalPower1 = meleeScorchList1[counter1].getTotalPower();
                              _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult + meleeScorchTotalPower1;
                              totalPower5 = totalPower5 + meleeScorchTotalPower1;
                              ++counter1;
                         }
                         if (random.NextDouble() >= 2 / meleeScorchList1.Count || random.NextDouble() >= 4 / totalPower5)
                         {
                              _lastCalculatedPowerPotential.strategicValue = 1;
                         }
                         else
                         {
                              _lastCalculatedPowerPotential.strategicValue = _lastCalculatedPowerPotential.powerChangeResult;
                         }
                    }
                    else
                    {
                         _lastCalculatedPowerPotential.strategicValue = _lastCalculatedPowerPotential.powerChangeResult + cardManager.cardValues.scorchCardValue;
                    }
               }
               if (templateRef.isType(CardTemplate.CardType_Creature))
               {
                    if (templateRef.hasEffect(CardTemplate.CardEffect_Nurse))
                    {
                         resurrectionList1 = new List<CardInstance>();
                         canResurrect = true;
                         counter1 = 0;
                         while (counter1 < handList.Count)
                         {
                              if (!handList[counter1].templateRef.hasEffect(CardTemplate.CardEffect_Nurse))
                              {
                                   canResurrect = false;
                                   break;
                              }
                              ++counter1;
                         }
                         cardManager.GetRessurectionTargets(listsPlayer, resurrectionList1, false);
                         if (resurrectionList1.Count != 0)
                         {
                              counter1 = 0;
                              while (counter1 < resurrectionList1.Count)
                              {
                                   if (!resurrectionList1[counter1].templateRef.hasEffect(CardTemplate.CardEffect_Nurse))
                                   {
                                        resurrectionList1[counter1].recalculatePowerPotential(cardManager);
                                   }
                                   ++counter1;
                              }
                              resurrectionList1.Sort(powerChangeSorter);
                              lastResurrectedCard = resurrectionList1[(resurrectionList1.Count - 1)];
                              powerChange = lastResurrectedCard.getOptimalTransaction().powerChangeResult;
                              _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult + powerChange;
                              if (random.NextDouble() <= 1 / handList.Count || random.NextDouble() >= 8 / powerChange)
                              {
                                   _lastCalculatedPowerPotential.strategicValue = 0;
                              }
                              else
                              {
                                   updatedPower1 = cardManager.cardValues.nurseCardValue + powerChange;
                                   _lastCalculatedPowerPotential.strategicValue = Math.Max(updatedPower1, templateRef.power);
                              }
                         }
                         else if (!canResurrect)
                         {
                              _lastCalculatedPowerPotential.powerChangeResult = -1000;
                              _lastCalculatedPowerPotential.strategicValue = -1;
                         }
                    }
                    else if (_lastCalculatedPowerPotential.strategicValue == 0)
                    {
                         _lastCalculatedPowerPotential.strategicValue = _lastCalculatedPowerPotential.strategicValue + templateRef.power;
                    }
               }
          }

          public int potentialWeatherHarm()
          {
               CardManager cardManager = null;
               List<CardInstance> handCardsList = null;
               int weatherHarm = 0;
               CardInstance currentCard = null;
               int totalPower = 0;
               List<CardInstance> effectsList = null;
               int listID = 0;
               int effectsCounter = 0;
               int creaturesCounter = 0;
               if (templateRef.isType(CardTemplate.CardType_Weather))
               {
                    cardManager = CardManager.getInstance();
                    handCardsList = cardManager.getAllCreaturesInHand(listsPlayer);
                    weatherHarm = 0;
                    totalPower = 0;
                    if (templateRef.hasEffect(CardTemplate.CardEffect_Melee))
                    {
                         listID = CardManager.CARD_LIST_LOC_MELEE;
                    }
                    else if (templateRef.hasEffect(CardTemplate.CardEffect_Ranged))
                    {
                         listID = CardManager.CARD_LIST_LOC_RANGED;
                    }
                    else if (templateRef.hasEffect(CardTemplate.CardEffect_Siege))
                    {
                         listID = CardManager.CARD_LIST_LOC_SEIGE;
                    }
                    effectsList = cardManager.cardEffectManager.getEffectsForList(listID, listsPlayer);
                    creaturesCounter = 0;
                    while (creaturesCounter < handCardsList.Count)
                    {
                         currentCard = handCardsList[creaturesCounter];
                         if (currentCard.templateRef.isType(CardTemplate.CardType_Creature) && !currentCard.templateRef.isType(CardTemplate.CardType_RangedMelee) && currentCard.templateRef.isType(listID))
                         {
                              effectsCounter = 0;
                              while (effectsCounter < effectsList.Count)
                              {
                                   currentCard.effectedByCardsRefList.Add(effectsList[effectsCounter]);
                                   ++effectsCounter;
                              }
                              totalPower = currentCard.getTotalPower();
                              currentCard.effectedByCardsRefList.Add(this);
                              weatherHarm = weatherHarm + Math.Max(0, totalPower - currentCard.getTotalPower());
                              currentCard.effectedByCardsRefList = new List<CardInstance>();//?
                         }
                         ++creaturesCounter;
                    }
                    return weatherHarm;
               }
               return 0;
          }

          public virtual void finalizeSetup()
          {

          }

          public void updateEffectsApplied(CardInstance argumentCard = null)//arguments not needed?
          {
               int effectsCounter = 0;
               List<CardInstance> effectsList = null;
               List<CardInstance> nurseList = null;
               CardInstance effectsCardInstance = null;
               List<CardInstance> scorchTargetsList = null;
               int scorchTargetsCounter = 0;
               List<CardInstance> meleeScorchTargetsList = null;
               int listID = 0;
               CardAndComboPoints cardAndComboPoints = null;
               bool hasMorale = false;
               GwintDeck deck = null;
               int clonesEffectsCounter = 0;
               bool hasClones = false;
               CardFXManager cardFXManager = CardFXManager.getInstance();
               CardManager cardManager = CardManager.getInstance();
               GwintGameFlowController gameFlowController = GwintGameFlowController.getInstance();
               Console.WriteLine("GFX - updateEffectsApplied Called ----------");
               if (templateRef.isType(CardTemplate.CardType_Creature) && !templateRef.isType(CardTemplate.CardType_Hero))
               {
                    effectsList = cardManager.cardEffectManager.getEffectsForList(inList, listsPlayer);
                    Console.WriteLine("GFX - fetched: ", effectsList.Count, ", effects for list:", inList, ", and Player:", listsPlayer);
                    effectsCounter = 0;
                    while (effectsCounter < effectsList.Count)
                    {
                         effectsCardInstance = effectsList[effectsCounter];
                         if (effectsCardInstance != this)
                         {
                              effectsCardInstance.addToEffectingList(this);
                         }
                         ++effectsCounter;
                    }
               }
               if (templateRef.isType(CardTemplate.CardType_Weather))
               {
                    if (!templateRef.hasEffect(CardTemplate.CardEffect_ClearSky))
                    {
                         effectsList = new List<CardInstance>();
                         if (templateRef.hasEffect(CardTemplate.CardEffect_Melee))
                         {
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, CardManager.PLAYER_1, effectsList);
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, CardManager.PLAYER_2, effectsList);
                              cardManager.cardEffectManager.registerActiveEffectCardInstance(this, CardManager.CARD_LIST_LOC_MELEE, CardManager.PLAYER_1);
                              cardManager.cardEffectManager.registerActiveEffectCardInstance(this, CardManager.CARD_LIST_LOC_MELEE, CardManager.PLAYER_2);
                              Console.WriteLine("GFX - Applying Melee Weather Effect");
                         }
                         if (templateRef.hasEffect(CardTemplate.CardEffect_Ranged))
                         {
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, CardManager.PLAYER_1, effectsList);
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, CardManager.PLAYER_2, effectsList);
                              cardManager.cardEffectManager.registerActiveEffectCardInstance(this, CardManager.CARD_LIST_LOC_RANGED, CardManager.PLAYER_1);
                              cardManager.cardEffectManager.registerActiveEffectCardInstance(this, CardManager.CARD_LIST_LOC_RANGED, CardManager.PLAYER_2);
                              Console.WriteLine("GFX - Applying Ranged Weather Effect");
                         }
                         if (templateRef.hasEffect(CardTemplate.CardEffect_Siege))
                         {
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, CardManager.PLAYER_1, effectsList);
                              cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, CardManager.PLAYER_2, effectsList);
                              cardManager.cardEffectManager.registerActiveEffectCardInstance(this, CardManager.CARD_LIST_LOC_SEIGE, CardManager.PLAYER_1);
                              cardManager.cardEffectManager.registerActiveEffectCardInstance(this, CardManager.CARD_LIST_LOC_SEIGE, CardManager.PLAYER_2);
                              Console.WriteLine("GFX - Applying SIEGE Weather Effect");
                         }
                         effectsCounter = 0;
                         while (effectsCounter < effectsList.Count)
                         {
                              effectsCardInstance = effectsList[effectsCounter];
                              addToEffectingList(effectsCardInstance);
                              ++effectsCounter;
                         }
                    }
               }
               if (templateRef.hasEffect(CardTemplate.CardEffect_Scorch))
               {
                    scorchTargetsList = cardManager.getScorchTargets();
                    Console.WriteLine("GFX - Applying Scorch Effect, number of targets: " + scorchTargetsList.Count);
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_scorch");
                    scorchTargetsCounter = 0;
                    while (scorchTargetsCounter < scorchTargetsList.Count)
                    {
                         Console.WriteLine("card instance scorchFX not implemented yet!");
                         //cardFXManager.playScorchEffectFX(scorchTargetsList[scorchTargetsCounter], onScorchFXEnd);
                         ++scorchTargetsCounter;
                    }
               }
               if (templateRef.hasEffect(CardTemplate.CardEffect_MeleeScorch))
               {
                    if (cardManager.calculatePlayerScore(CardManager.CARD_LIST_LOC_MELEE, notListPlayer) >= 10)
                    {
                         meleeScorchTargetsList = cardManager.getScorchTargets(CardTemplate.CardType_Melee, notListPlayer);
                         Console.WriteLine("GFX - Applying scorchMeleeList, number of targets: " + meleeScorchTargetsList.Count);
                         //GwintGameMenu.mSingleton.playSound("gui_gwint_scorch");
                         effectsCounter = 0;
                         while (effectsCounter < meleeScorchTargetsList.Count)
                         {
                              Console.WriteLine("card instance scorchFX not implemented yet!");
                              //cardFXManager.playScorchEffectFX(meleeScorchTargetsList[effectsCounter], onScorchFXEnd);
                              ++effectsCounter;
                         }
                    }
               }
               if (templateRef.hasEffect(CardTemplate.CardEffect_Horn))
               {
                    Console.WriteLine("GFX - Applying Horn Effect ----------");
                    listID = CardManager.CARD_LIST_LOC_INVALID;
                    if (inList == CardManager.CARD_LIST_LOC_MELEEMODIFIERS || inList == CardManager.CARD_LIST_LOC_MELEE)
                    {
                         listID = CardManager.CARD_LIST_LOC_MELEE;
                    }
                    else if (inList == CardManager.CARD_LIST_LOC_RANGEDMODIFIERS || inList == CardManager.CARD_LIST_LOC_RANGED)
                    {
                         listID = CardManager.CARD_LIST_LOC_RANGED;
                    }
                    else if (inList == CardManager.CARD_LIST_LOC_SEIGEMODIFIERS || inList == CardManager.CARD_LIST_LOC_SEIGE)
                    {
                         listID = CardManager.CARD_LIST_LOC_SEIGE;
                    }
                    if (listID != CardManager.PLAYER_INVALID)
                    {
                         effectsList = cardManager.getCardInstanceList(listID, listsPlayer);
                         if (effectsList != null)
                         {
                              effectsCounter = 0;
                              while (effectsCounter < effectsList.Count)
                              {
                                   effectsCardInstance = effectsList[effectsCounter];
                                   if (!effectsCardInstance.templateRef.isType(CardTemplate.CardType_Hero) && !(effectsCardInstance == this))
                                   {
                                        addToEffectingList(effectsCardInstance);
                                   }
                                   ++effectsCounter;
                              }
                         }
                         Console.WriteLine("card instance player card and row effect not implemented yet!");
                         //cardFXManager.playerCardEffectFX(this, null);
                         //cardFXManager.playRowEffect(listID, listsPlayer);
                         cardManager.cardEffectManager.registerActiveEffectCardInstance(this, listID, listsPlayer);
                    }
               }
               if (templateRef.hasEffect(CardTemplate.CardEffect_Nurse))
               {
                    nurseList = new List<CardInstance>();
                    cardManager.GetRessurectionTargets(listsPlayer, nurseList, true);
                    Console.WriteLine("GFX - Applying Nurse Effect");
                    if (nurseList.Count > 0)
                    {
                         if (gameFlowController.playerControllers[listsPlayer] is AIPlayerController)
                         {
                              cardAndComboPoints = cardManager.getHigherOrLowerValueCardFromTargetGraveyard(listsPlayer, true, true, false);
                              effectsCardInstance = cardAndComboPoints.cardInstance;
                              handleNurseChoice(effectsCardInstance.instanceId);
                         }
                         else
                         {
                              Console.WriteLine("choice dialogue not implemented yet!");
                              //gameFlowController.mcChoiceDialog.showDialogCardInstances(nurseList, handleNurseChoice, noNurseChoice, "[[gwint_choose_card_to_ressurect]]");
                         }
                    }
               }
               if (templateRef.hasEffect(CardTemplate.CardEffect_ImproveNeighbours))
               {
                    effectsList = cardManager.getCardInstanceList(inList, listsPlayer);
                    Console.WriteLine("GFX - Applying Improve Neightbours effect");
                    effectsCounter = 0;
                    while (effectsCounter < effectsList.Count)
                    {
                         effectsCardInstance = effectsList[effectsCounter];
                         if (!effectsCardInstance.templateRef.isType(CardTemplate.CardType_Hero) && !(effectsCardInstance == this))
                         {
                              addToEffectingList(effectsCardInstance);
                         }
                         ++effectsCounter;
                    }
                    Console.WriteLine(" card instance player card effect not implemented yet!");
                    //cardFXManager.playerCardEffectFX(this, null);
                    cardManager.cardEffectManager.registerActiveEffectCardInstance(this, inList, listsPlayer);
               }
               if (templateRef.hasEffect(CardTemplate.CardEffect_SameTypeMorale))
               {
                    effectsList = new List<CardInstance>();
                    cardManager.getAllCreaturesNonHero(inList, listsPlayer, effectsList);
                    Console.WriteLine("GFX - Applying Right Bonds effect");
                    hasMorale = false;
                    effectsCounter = 0;
                    while (effectsCounter < effectsList.Count)
                    {
                         effectsCardInstance = effectsList[effectsCounter];
                         if (!(effectsCardInstance == this) && !(templateRef.summonFlags.IndexOf(effectsCardInstance.templateId) == -1))
                         {
                              effectsCardInstance.addToEffectingList(this);
                              addToEffectingList(effectsCardInstance);
                              Console.WriteLine(" card instance morale boost effect not implemented yet");
                              //GwintGameMenu.mSingleton.playSound("gui_gwint_morale_boost");
                              //cardFXManager.playTightBondsFX(effectsCardInstance, null);
                              hasMorale = true;
                         }
                         ++effectsCounter;
                    }
                    if (hasMorale)
                    {
                         Console.WriteLine(" card instance morale boost effect not implemented yet");
                         //cardFXManager.playTightBondsFX(this, null);
                    }
               }
               if (templateRef.hasEffect(CardTemplate.CardEffect_SummonClones))
               {
                    deck = cardManager.playerDeckDefinitions[listsPlayer];
                    effectsList = cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, listsPlayer);
                    hasClones = false;
                    effectsCounter = 0;
                    while (effectsCounter < templateRef.summonFlags.Count && !hasClones)
                    {
                         if (deck.numCopiesLeft(templateRef.summonFlags[effectsCounter]) > 0)
                         {
                              hasClones = true;
                         }
                         clonesEffectsCounter = 0;
                         while (clonesEffectsCounter < effectsList.Count)
                         {
                              if (effectsList[clonesEffectsCounter].templateId == templateRef.summonFlags[effectsCounter])
                              {
                                   hasClones = true;
                                   break;
                              }
                              ++clonesEffectsCounter;
                         }
                         ++effectsCounter;
                    }
                    Console.WriteLine("GFX - Applying Summon Clones Effect, found summons: " + hasClones);
                    if (hasClones)
                    {
                         Console.WriteLine("clones card instance player card effect not implemented yet!");
                         //cardFXManager.playerCardEffectFX(this, summonFXEnded);
                    }
               }
               if (templateRef.hasEffect(CardTemplate.CardEffect_Draw2))
               {
                    Console.WriteLine("GFX - applying draw 2 effect");
                    cardManager.drawCards(listsPlayer != CardManager.PLAYER_1 ? CardManager.PLAYER_1 : CardManager.PLAYER_2, 2);
               }
               cardManager.recalculateScores();
          }

          protected void addToEffectingList(CardInstance card)
          {
               effectingCardsRefList.Add(card);
               card.addEffect(this);
          }

          protected void addEffect(CardInstance card)
          {
               effectedByCardsRefList.Add(card);
               powerChangeCallback();
          }//action callback

          protected void handleNurseChoice(int cardID)
          {
               CardSlot cardSlot = null;
               CardManager cardManager = CardManager.getInstance();
               CardInstance card = cardManager.getCardInstance(cardID);
               CardFXManager cardFXManager = CardFXManager.getInstance();
               GwintBoardRenderer boardRenderer = cardManager.boardRenderer;
               if (boardRenderer != null)
               {
                    cardSlot = boardRenderer.getCardSlotById(cardID);
                    if (cardSlot != null)
                    {
                         Console.WriteLine("handles nurse adding child not implemented yet!");
                         //cardSlot.parent.addChild(cardSlot);
                    }
               }
               Console.WriteLine(" nurse resurrect effect not implemented yet!");
               /*GwintGameMenu.mSingleton.playSound("gui_gwint_resurrect");
               cardFXManager.playRessurectEffectFX(card, onNurseEffectEnded);
               if (GwintGameFlowController.getInstance().mcChoiceDialog.visible) 
               {
                   GwintGameFlowController.getInstance().mcChoiceDialog.hideDialog();
               }*/
          }

          public bool canBePlacedInSlot(int listID, int playerID)
          {
               CardManager cardManager = CardManager.getInstance();
               if (listID == CardManager.CARD_LIST_LOC_DECK || listID == CardManager.CARD_LIST_LOC_GRAVEYARD)
               {
                    return false;
               }
               if (playerID == CardManager.PLAYER_INVALID && listID == CardManager.CARD_LIST_LOC_WEATHERSLOT && templateRef.isType(CardTemplate.CardType_Weather))
               {
                    return true;
               }
               if (playerID == listsPlayer && templateRef.isType(CardTemplate.CardType_Spy))
               {
                    return false;
               }
               if (!templateRef.isType(CardTemplate.CardType_Spy) && !(playerID == listsPlayer) && (templateRef.isType(CardTemplate.CardType_Creature) || templateRef.isType(CardTemplate.CardType_Row_Modifier)))
               {
                    return false;
               }
               if (templateRef.isType(CardTemplate.CardType_Creature))
               {
                    if (listID == CardManager.CARD_LIST_LOC_MELEE && templateRef.isType(CardTemplate.CardType_Melee))
                    {
                         return true;
                    }
                    if (listID == CardManager.CARD_LIST_LOC_RANGED && templateRef.isType(CardTemplate.CardType_Ranged))
                    {
                         return true;
                    }
                    if (listID == CardManager.CARD_LIST_LOC_SEIGE && templateRef.isType(CardTemplate.CardType_Siege))
                    {
                         return true;
                    }
               }
               else if (templateRef.isType(CardTemplate.CardType_Row_Modifier))
               {
                    if (listID == CardManager.CARD_LIST_LOC_MELEEMODIFIERS && templateRef.isType(CardTemplate.CardType_Melee) && cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_MELEEMODIFIERS, listsPlayer).Count == 0)
                    {
                         return true;
                    }
                    if (listID == CardManager.CARD_LIST_LOC_RANGEDMODIFIERS && templateRef.isType(CardTemplate.CardType_Ranged) && cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_RANGEDMODIFIERS, listsPlayer).Count == 0)
                    {
                         return true;
                    }
                    if (listID == CardManager.CARD_LIST_LOC_SEIGEMODIFIERS && templateRef.isType(CardTemplate.CardType_Siege) && cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_SEIGEMODIFIERS, listsPlayer).Count == 0)
                    {
                         return true;
                    }
               }
               return false;
          }
     }
}
