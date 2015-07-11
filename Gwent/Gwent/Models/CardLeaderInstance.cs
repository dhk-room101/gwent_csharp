using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class CardLeaderInstance: CardInstance
     {
          public int leaderEffect;
          protected int numToBin;
          protected int numBinnedSoFar;
          protected int numToPick;
          protected int numPickedSoFar;
          private bool _canBeUsed = true;
          private Random random = new Random();

          private Action applyClearWeatherAction;
          
          public CardLeaderInstance()
          {
               //initialize actions
               applyClearWeatherAction = () => applyClearWeather();
          }

          public bool canBeUsed
          {
               get
               {
                    if (!_canBeUsed)
                    {
                         return false;
                    }
                    return canAbilityBeApplied();
               }

               set
               {
                    _canBeUsed = value;
               }
          }

          protected bool canAbilityBeApplied()
          {
               var cardManager = CardManager.getInstance();
               var playerDeckDefinitions = cardManager.playerDeckDefinitions[owningPlayer];
               var cardsList = new List<int>();
               var cardInstanceList = new List<CardInstance>();
               var firstEffect = templateRef.getFirstEffect();
               switch (firstEffect)
               {
                    case CardTemplate.CardEffect_Clear_Weather:
                         {
                              if (cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_WEATHERSLOT, CardManager.PLAYER_INVALID).Count == 0)
                              {
                                   return false;
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Pick_Fog:
                         {
                              playerDeckDefinitions.getCardsInDeck(CardTemplate.CardType_Weather, CardTemplate.CardEffect_Ranged, cardsList);
                              return cardsList.Count > 0;
                         }
                    case CardTemplate.CardEffect_Siege_Horn:
                         {
                              if (cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_SEIGEMODIFIERS, owningPlayer).Count > 0)
                              {
                                   return false;
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Siege_Scorch:
                         {
                              if (cardManager.getScorchTargets(CardTemplate.CardType_Siege, notOwningPlayer) == null || cardManager.calculatePlayerScore(CardManager.CARD_LIST_LOC_SEIGE, notOwningPlayer) < 10)
                              {
                                   return false;
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Pick_Frost:
                         {
                              playerDeckDefinitions.getCardsInDeck(CardTemplate.CardType_Weather, CardTemplate.CardEffect_Melee, cardsList);
                              return cardsList.Count > 0;
                         }
                    case CardTemplate.CardEffect_Range_Horn:
                         {
                              if (cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_RANGEDMODIFIERS, owningPlayer).Count > 0)
                              {
                                   return false;
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_11th_card:
                         {
                              return true;
                         }
                    case CardTemplate.CardEffect_MeleeScorch:
                         {
                              if (cardManager.getScorchTargets(CardTemplate.CardType_Melee, notOwningPlayer) == null || cardManager.calculatePlayerScore(CardManager.CARD_LIST_LOC_MELEE, notOwningPlayer) < 10)
                              {
                                   return false;
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Pick_Rain:
                         {
                              playerDeckDefinitions.getCardsInDeck(CardTemplate.CardType_Weather, CardTemplate.CardEffect_Siege, cardsList);
                              return cardsList.Count > 0;
                         }
                    case CardTemplate.CardEffect_View_3_Enemy:
                         {
                              if (cardManager !=null && cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, notOwningPlayer) != null)
                              {
                                   return cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, notOwningPlayer).Count > 0;
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Resurect_Enemy:
                         {
                              cardManager.GetRessurectionTargets(notOwningPlayer, cardInstanceList, false);
                              if (cardInstanceList == null)
                              {
                                   return false;
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Counter_King:
                         {
                              return false;
                         }
                    case CardTemplate.CardEffect_Bin2_Pick1:
                         {
                              cardInstanceList = cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, owningPlayer);
                              return cardInstanceList != null && cardInstanceList.Count > 1 && playerDeckDefinitions.cardIndicesInDeck.Count > 0;
                         }
                    case CardTemplate.CardEffect_Pick_Weather:
                         {
                              playerDeckDefinitions.getCardsInDeck(CardTemplate.CardType_Weather, CardTemplate.CardEffect_None, cardsList);
                              return cardsList.Count > 0;
                         }
                    case CardTemplate.CardEffect_Resurect:
                         {
                              cardManager.GetRessurectionTargets(owningPlayer, cardInstanceList, false);
                              if (cardInstanceList == null)
                              {
                                   return false;
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Melee_Horn:
                         {
                              if (cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_MELEEMODIFIERS, owningPlayer).Count > 0)
                              {
                                   return false;
                              }
                              break;
                         }
               }
               return true;
          }//needs tweaking due to card instance list/array mishap?

          public bool hasBeenUsed
          {
               get { return !_canBeUsed; }
          }

          public override void finalizeSetup()
          {
               if (templateRef == null || templateRef.getFirstEffect() == CardTemplate.CardEffect_None)
               {
                    throw new ArgumentException("GFX [ERROR] tried to finalize card leader with invalid template info - " + templateId);
               }
               leaderEffect = templateRef.getFirstEffect();
               if (leaderEffect == CardTemplate.CardEffect_Counter_King)
               {
                    _canBeUsed = false;
               }
          }

          protected void clearWeather()
          {
               CardFXManager.getInstance().spawnFX(null, applyClearWeatherAction, CardFXManager.getInstance()._clearWeatherFXClassRef);
          }

          protected void applyClearWeather()
          {
               CardManager cardManager = CardManager.getInstance();
               List<CardInstance> weatherList = cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_WEATHERSLOT, CardManager.PLAYER_INVALID);
               while (weatherList.Count > 0)
               {
                    cardManager.sendToGraveyard(weatherList[0]);
               }
          }

          protected void applyHorn(int listID, int playerID)
          {
               CardManager cardManager = CardManager.getInstance();
               CardInstance cardInstance = cardManager.spawnCardInstance(1, playerID, CardManager.CARD_LIST_LOC_LEADER);
               cardManager.addCardInstanceToList(cardInstance, listID, playerID);
          }

          protected void scorch(int type)
          {
               int counter = 0;
               CardManager cardManager = CardManager.getInstance();
               CardFXManager cardFXManager = CardFXManager.getInstance();
               List<CardInstance> scorchList = cardManager.getScorchTargets(type, notOwningPlayer);
               //GwintGameMenu.mSingleton.playSound("gui_gwint_scorch");
               while (counter < scorchList.Count)
               {
                    Console.WriteLine("card leader instance scorchFX not implemented yet!");
                    //cardFXManager.playScorchEffectFX(scorchList[counter], onScorchFXEnd);
                    ++counter;
               }
          }

          protected void ShowEnemyHand(int numberOfCards)
          {
               List<CardInstance> opponentHandList = null;
               int counter = 0;
               int randomPick = 0;
               CardManager cardManager = CardManager.getInstance();
               List<CardInstance> opponentHandListClone = new List<CardInstance>();
               List<CardInstance> opponentHandListRandom = new List<CardInstance>();
               opponentHandList = cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, notOwningPlayer);
               while (counter < opponentHandList.Count)
               {
                    opponentHandListClone.Add(opponentHandList[counter]);
                    ++counter;
               }
               while (numberOfCards > 0 && opponentHandListClone.Count > 0)
               {
                    randomPick = (int)Math.Min(Math.Floor(random.NextDouble() * opponentHandListClone.Count), (opponentHandListClone.Count - 1));
                    opponentHandListRandom.Add(opponentHandListClone[randomPick]);
                    opponentHandListClone.RemoveRange(randomPick, 1);
                    --numberOfCards;
               }
               if (opponentHandListRandom.Count > 0)
               {
                    //display?
                    //GwintGameFlowController.getInstance().mcChoiceDialog.showDialogCardInstances(opponentHandListRandom, null, handleHandShowClose, "[[gwint_showing_enemy_hand]]");
               }
               else
               {
                    throw new ArgumentException("GFX [ERROR] - Tried to ShowEnemyHand with no cards chosen?! - " + opponentHandList.Count);
               }
          }

          protected void handleHandShowClose(int notneeded = -1)
          {
               //display?
               //GwintGameFlowController.getInstance().mcChoiceDialog.hideDialog();
          }

          protected void resurrectGraveyard(int playerID)
          {
               CardManager cardManager = CardManager.getInstance();
               List<CardInstance> graveyardList = new List<CardInstance>();
               cardManager.GetRessurectionTargets(playerID, graveyardList, true);
               if (graveyardList.Count > 0)
               {
                    if (graveyardList.Count != 1)
                    {
                         Console.WriteLine("graveyard resurrection dialogue not implemented yet!");
                         //display?
                         //GwintGameFlowController.getInstance().mcChoiceDialog.showDialogCardInstances(graveyardList, handleResurrectChoice, null, "[[gwint_choose_card_to_ressurect]]");
                    }
                    else
                    {
                         cardManager.addCardInstanceToList(graveyardList[0], CardManager.CARD_LIST_LOC_HAND, owningPlayer);
                    }
               }
               else
               {
                    throw new ArgumentException("GFX [ERROR] - tried to ressurect from player: " + playerID + "\'s graveyard but found no cards");
               }
          }

          protected void handleResurrectChoice(int cardID = -1)
          {
               //display?
               //GwintGameFlowController.getInstance().mcChoiceDialog.hideDialog();
               if (cardID == -1)
               {
                    throw new ArgumentException("GFX [ERROR] - tried to ressurect card with no valid id");
               }
               CardManager.getInstance().addCardInstanceIDToList(cardID, CardManager.CARD_LIST_LOC_HAND, owningPlayer);
          }

          protected void pickWeather(bool apply)
          {
               CardManager cardManager = CardManager.getInstance();
               GwintDeck owningPlayerDeck = cardManager.playerDeckDefinitions[owningPlayer];
               List<int> cardsInDeck = new List<int>();
               owningPlayerDeck.getCardsInDeck(CardTemplate.CardType_Weather, CardTemplate.CardEffect_None, cardsInDeck);
               if (cardsInDeck.Count == 1 || apply && cardsInDeck.Count > 0)
               {
                    cardManager.tryDrawAndPlaySpecificCard_Weather(owningPlayer, cardsInDeck[0]);
               }
               else if (cardsInDeck.Count > 0)
               {
                    //display?
                    //GwintGameFlowController.getInstance().mcChoiceDialog.showDialogCardTemplates(cardsInDeck, handleCardDrawChoice_Weather, null, "[[gwint_pick_card_to_draw]]");
               }
               else
               {
                    throw new ArgumentException("GFX [ERROR] - tried to pick weather card when there was none");
               }
          }

          protected void handleCardDrawChoice_Weather(int cardID = -1)
          {
               //display?
               //GwintGameFlowController.getInstance().mcChoiceDialog.hideDialog();
               if (cardID == -1)
               {
                    throw new ArgumentException("GFX [ERROR] - tried to draw card with invalid ID");
               }
               CardManager.getInstance().tryDrawAndPlaySpecificCard_Weather(owningPlayer, cardID);
          }

          protected void binPick(int bin, int pick)
          {
               numToBin = bin;
               numBinnedSoFar = 0;
               numToPick = pick;
               numPickedSoFar = 0;
               if (numToBin > numBinnedSoFar)
               {
                    askBin();
               }
               else if (numToPick > numPickedSoFar)
               {
                    askPick();
               }
               else
               {
                    throw new ArgumentException("GFX [ERROR] - called binPick with invalid values");
               }
          }

          protected void askBin()
          {
               List<CardInstance> handList = CardManager.getInstance().getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, owningPlayer);
               if (handList.Count == 0)
               {
                    throw new ArgumentException("GFX [ERROR] - Tried to bin a card when there are none left in the hand");
               }
               //display?
               //GwintGameFlowController.getInstance().mcChoiceDialog.showDialogCardInstances(handList, handleBinChoice, null, "[[gwint_choose_card_to_dump]]");
          }

          protected void handleBinChoice(int cardID = -1)
          {
               CardManager cardManager = CardManager.getInstance();
               CardLeaderInstance cardLeaderInstance = this;
               int binned = numBinnedSoFar + 1;
               cardLeaderInstance.numBinnedSoFar = binned;
               cardManager.addCardInstanceIDToList(cardID, CardManager.CARD_LIST_LOC_GRAVEYARD, owningPlayer);
               //GwintGameMenu.mSingleton.playSound("gui_gwint_discard_card");
               if (numToBin > numBinnedSoFar)
               {
                    askBin();
               }
               else if (numToPick > numPickedSoFar)
               {
                    askPick();
               }
               else
               {
                    //display?
                    //GwintGameFlowController.getInstance().mcChoiceDialog.hideDialog();
               }
          }

          protected void askPick()
          {
               CardManager cardManager = CardManager.getInstance();
               GwintDeck owningPlayerDeck = cardManager.playerDeckDefinitions[owningPlayer];
               List<int> cardsInDeck = new List<int>();
               owningPlayerDeck.getCardsInDeck(CardTemplate.CardType_None, CardTemplate.CardEffect_None, cardsInDeck);
               if (cardsInDeck.Count == 0)
               {
                    throw new ArgumentException("GFX [ERROR] - Tried to pick a card when there are none left in the deck");
               }
               //display?
               //GwintGameFlowController.getInstance().mcChoiceDialog.showDialogCardTemplates(cardsInDeck, handlePickChoice, null, "[[gwint_pick_card_to_draw]]");
          }

          protected void handlePickChoice(int cardID = -1)
          {
               //CardLeaderInstance cardLeaderInstance;
               //int picked = numPickedSoFar + 1;
               //cardLeaderInstance.numPickedSoFar = picked;
               ++numPickedSoFar;//increment?
               CardManager.getInstance().tryDrawSpecificCard(owningPlayer, cardID);
               if (numToPick > numPickedSoFar)
               {
                    askPick();
               }
               else
               {
                    //display?
                    //GwintGameFlowController.getInstance().mcChoiceDialog.hideDialog();
               }
          }

          public string toString()//override?
          {
               return "super | canBeUsed: " + canBeUsed.ToString() + ", canBeApplied: " + canAbilityBeApplied().ToString();
          }
          
          public override void recalculatePowerPotential(CardManager argumentCardManager)
          {
               CardInstance currentCard = null;
               CardAndComboPoints cardAndComboPoints = null;
               int points = 0;
               int deltaPower = 0;
               int totalPower = 0;
               int counter = 0;
               _lastCalculatedPowerPotential.powerChangeResult = 0;
               _lastCalculatedPowerPotential.strategicValue = -1;
               _lastCalculatedPowerPotential.sourceCardInstanceRef = this;
               CardManager instanceCardManager = CardManager.getInstance();
               List<CardInstance> creaturesList = new List<CardInstance>();
               int _player = owningPlayer == CardManager.PLAYER_1 ? (CardManager.PLAYER_2) : (CardManager.PLAYER_1);
               List<CardInstance> handCardsList = instanceCardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, owningPlayer);
               switch (templateRef.getFirstEffect())
               {
                    case CardTemplate.CardEffect_11th_card:
                    case CardTemplate.CardEffect_Counter_King:
                         {
                              break;
                         }
                    case CardTemplate.CardEffect_Pick_Fog:
                    case CardTemplate.CardEffect_Pick_Frost:
                    case CardTemplate.CardEffect_Pick_Weather:
                    case CardTemplate.CardEffect_Pick_Rain:
                         {
                              _lastCalculatedPowerPotential.strategicValue = instanceCardManager.cardValues.weatherCardValue;
                              break;
                         }
                    case CardTemplate.CardEffect_View_3_Enemy:
                         {
                              _lastCalculatedPowerPotential.strategicValue = 0;
                              break;
                         }
                    case CardTemplate.CardEffect_Siege_Horn:
                    case CardTemplate.CardEffect_Range_Horn:
                    case CardTemplate.CardEffect_Melee_Horn:
                         {
                              if (templateRef.getFirstEffect() == CardTemplate.CardEffect_Siege_Horn)
                              {
                                   instanceCardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, owningPlayer, creaturesList);
                              }
                              else if (templateRef.getFirstEffect() == CardTemplate.CardEffect_Range_Horn)
                              {
                                   instanceCardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, owningPlayer, creaturesList);
                              }
                              else if (templateRef.getFirstEffect() == CardTemplate.CardEffect_Melee_Horn)
                              {
                                   instanceCardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, owningPlayer, creaturesList);
                              }
                              deltaPower = 0;
                              counter = 0;
                              while (counter < creaturesList.Count)
                              {

                                   currentCard = creaturesList[counter];
                                   totalPower = currentCard.getTotalPower();
                                   //currentCard.effectedByCardsRefList.Count(this);//?
                                   deltaPower = currentCard.getTotalPower() - totalPower;
                                   currentCard.effectedByCardsRefList.RemoveAt(currentCard.effectedByCardsRefList.Count - 1);
                                   ++counter;
                              }
                              _lastCalculatedPowerPotential.powerChangeResult = deltaPower;
                              _lastCalculatedPowerPotential.strategicValue = instanceCardManager.cardValues.hornCardValue;
                              break;
                         }
                    case CardTemplate.CardEffect_Siege_Scorch:
                         {
                              creaturesList = instanceCardManager.getScorchTargets(CardTemplate.CardType_Siege, _player);
                              counter = 0;
                              while (counter < creaturesList.Count)
                              {

                                   currentCard = creaturesList[counter];
                                   if (currentCard.listsPlayer == owningPlayer)
                                   {
                                        _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult - currentCard.getTotalPower();
                                   }
                                   else
                                   {
                                        _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult + currentCard.getTotalPower();
                                   }
                                   ++counter;
                              }
                              _lastCalculatedPowerPotential.strategicValue = instanceCardManager.cardValues.scorchCardValue;
                              break;
                         }
                    case CardTemplate.CardEffect_MeleeScorch:
                         {
                              creaturesList = instanceCardManager.getScorchTargets(CardTemplate.CardType_Melee, _player);
                              counter = 0;
                              while (counter < creaturesList.Count)
                              {

                                   currentCard = creaturesList[counter];
                                   if (currentCard.listsPlayer == owningPlayer)
                                   {
                                        _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult - currentCard.getTotalPower();
                                   }
                                   else
                                   {
                                        _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult + currentCard.getTotalPower();
                                   }
                                   ++counter;
                              }
                              _lastCalculatedPowerPotential.strategicValue = instanceCardManager.cardValues.scorchCardValue;
                              break;
                         }
                    case CardTemplate.CardEffect_Clear_Weather:
                         {
                              creaturesList = new List<CardInstance>();
                              argumentCardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, owningPlayer, creaturesList);
                              argumentCardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, owningPlayer, creaturesList);
                              argumentCardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, owningPlayer, creaturesList);
                              counter = 0;
                              while (counter < creaturesList.Count)
                              {

                                   _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult + (currentCard.getTotalPower(true) - currentCard.getTotalPower());
                                   ++counter;
                              }
                              creaturesList = new List<CardInstance>();
                              argumentCardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, _player, creaturesList);
                              argumentCardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, _player, creaturesList);
                              argumentCardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, _player, creaturesList);
                              counter = 0;
                              while (counter < creaturesList.Count)
                              {

                                   _lastCalculatedPowerPotential.powerChangeResult = _lastCalculatedPowerPotential.powerChangeResult - (currentCard.getTotalPower(true) - currentCard.getTotalPower());
                                   ++counter;
                              }
                              _lastCalculatedPowerPotential.strategicValue = instanceCardManager.cardValues.weatherCardValue;
                              break;
                         }
                    case CardTemplate.CardEffect_Resurect_Enemy:
                         {
                              cardAndComboPoints = instanceCardManager.getHigherOrLowerValueCardFromTargetGraveyard(_player, true, true, false, true);
                              if (cardAndComboPoints != null)
                              {
                                   points = cardAndComboPoints.comboPoints;
                                   if (handCardsList.Count < 8 || random.NextDouble() <= 1 / handCardsList.Count * 0.5)
                                   {
                                        _lastCalculatedPowerPotential.strategicValue = Math.Max(0, 10 - points);
                                        _lastCalculatedPowerPotential.powerChangeResult = cardAndComboPoints.cardInstance.getTotalPower();
                                   }
                                   else
                                   {
                                        _lastCalculatedPowerPotential.strategicValue = 1000;
                                   }
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Resurect:
                         {
                              cardAndComboPoints = instanceCardManager.getHigherOrLowerValueCardFromTargetGraveyard(owningPlayer, true, true, false);
                              if (cardAndComboPoints != null)
                              {
                                   points = cardAndComboPoints.comboPoints;
                                   if (handCardsList.Count < 8 || random.NextDouble() <= 1 / handCardsList.Count * 0.5)
                                   {
                                        _lastCalculatedPowerPotential.strategicValue = Math.Max(0, 10 - points);
                                        _lastCalculatedPowerPotential.powerChangeResult = cardAndComboPoints.cardInstance.getTotalPower();
                                   }
                                   else
                                   {
                                        _lastCalculatedPowerPotential.strategicValue = 1000;
                                   }
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Bin2_Pick1:
                         {
                              break;
                         }
                    default:
                         {
                              break;
                         }
               }
          }

          public void ApplyLeaderAbility(bool apply)
          {
               CardInstance cardFromGraveyard = null;
               CardAndComboPoints cardAndComboPoints = null;
               if (!_canBeUsed)
               {
                    throw new ArgumentException("GFX [ERROR] - Tried to apply a card ability when its disabled!");
               }
               CardManager cardManager = CardManager.getInstance();
               GwintDeck playerDeck = cardManager.playerDeckDefinitions[owningPlayer];
               List<int> cardsInDeck = new List<int>();
               _canBeUsed = false;
               //GwintGameMenu.mSingleton.playSound("gui_gwint_using_ability");
               int firstEffect = templateRef.getFirstEffect();
               switch (firstEffect)
               {
                    case CardTemplate.CardEffect_Clear_Weather:
                         {
                              clearWeather();
                              break;
                         }
                    case CardTemplate.CardEffect_Pick_Fog:
                         {
                              playerDeck.getCardsInDeck(CardTemplate.CardType_Weather, CardTemplate.CardEffect_Ranged, cardsInDeck);
                              if (cardsInDeck.Count > 0)
                              {
                                   cardManager.tryDrawAndPlaySpecificCard_Weather(owningPlayer, cardsInDeck[0]);
                              }
                              else
                              {
                                   throw new ArgumentException("GFX [ERROR] - tried to pick fog but did not have any");
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Siege_Horn:
                         {
                              applyHorn(CardManager.CARD_LIST_LOC_SEIGEMODIFIERS, owningPlayer);
                              break;
                         }
                    case CardTemplate.CardEffect_Siege_Scorch:
                         {
                              scorch(CardTemplate.CardType_Siege);
                              break;
                         }
                    case CardTemplate.CardEffect_Pick_Frost:
                         {
                              playerDeck.getCardsInDeck(CardTemplate.CardType_Weather, CardTemplate.CardEffect_Melee, cardsInDeck);
                              if (cardsInDeck.Count > 0)
                              {
                                   cardManager.tryDrawAndPlaySpecificCard_Weather(owningPlayer, cardsInDeck[0]);
                              }
                              else
                              {
                                   throw new ArgumentException("GFX [ERROR] - tried to pick frost but did not have any");
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Range_Horn:
                         {
                              applyHorn(CardManager.CARD_LIST_LOC_RANGEDMODIFIERS, owningPlayer);
                              break;
                         }
                    case CardTemplate.CardEffect_11th_card:
                         {
                              throw new ArgumentException("GFX [ERROR] - tried to apply 11th card ability which should not occur through here");
                         }
                    case CardTemplate.CardEffect_MeleeScorch:
                         {
                              scorch(CardTemplate.CardType_Melee);
                              break;
                         }
                    case CardTemplate.CardEffect_Pick_Rain:
                         {
                              playerDeck.getCardsInDeck(CardTemplate.CardType_Weather, CardTemplate.CardEffect_Siege, cardsInDeck);
                              if (cardsInDeck.Count > 0)
                              {
                                   cardManager.tryDrawAndPlaySpecificCard_Weather(owningPlayer, cardsInDeck[0]);
                              }
                              else
                              {
                                   throw new ArgumentException("GFX [ERROR] - tried to pick Rain but did not have any");
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_View_3_Enemy:
                         {
                              if (!apply)
                              {
                                   ShowEnemyHand(3);
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Resurect_Enemy:
                         {
                              if (apply)
                              {
                                   cardAndComboPoints = cardManager.getHigherOrLowerValueCardFromTargetGraveyard(notOwningPlayer, true, true, false, true);
                                   if (cardAndComboPoints != null)
                                   {
                                        cardFromGraveyard = cardAndComboPoints.cardInstance;
                                        handleResurrectChoice(cardFromGraveyard.instanceId);
                                   }
                                   else
                                   {
                                        throw new ArgumentException("GFX [ERROR] - AI tried to ressurect enemy card when there wasn\'t a valid target!");
                                   }
                              }
                              else
                              {
                                   resurrectGraveyard(notOwningPlayer);
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Counter_King:
                         {
                              throw new ArgumentException("GFX [ERROR] - tried to apply couner king ability which should not occur through here");
                         }
                    case CardTemplate.CardEffect_Bin2_Pick1:
                         {
                              if (apply)
                              {
                                   Console.WriteLine("GFX [WARNING] - AI tried to bin2, pick 1 but it was never properly implemented");
                              }
                              else
                              {
                                   binPick(2, 1);
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Pick_Weather:
                         {
                              pickWeather(apply);
                              break;
                         }
                    case CardTemplate.CardEffect_Resurect:
                         {
                              if (apply)
                              {
                                   cardAndComboPoints = cardManager.getHigherOrLowerValueCardFromTargetGraveyard(owningPlayer, true, true, false);
                                   if (cardAndComboPoints != null)
                                   {
                                        cardFromGraveyard = cardAndComboPoints.cardInstance;
                                        handleResurrectChoice(cardFromGraveyard.instanceId);
                                   }
                                   else
                                   {
                                        throw new ArgumentException("GFX [ERROR] - AI tried to ressurect enemy card when there wasn\'t a valid target!");
                                   }
                              }
                              else
                              {
                                   resurrectGraveyard(owningPlayer);
                              }
                              break;
                         }
                    case CardTemplate.CardEffect_Melee_Horn:
                         {
                              applyHorn(CardManager.CARD_LIST_LOC_MELEEMODIFIERS, owningPlayer);
                              break;
                         }
               }
          }
     }
}
