using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Gwent.Models
{
     public class AIPlayerController: BasePlayerController
     {
          protected const int TACTIC_NONE = 0;
          protected const int TACTIC_MINIMIZE_LOSS = 1;
          protected const int TACTIC_MINIMIZE_WIN = 2;
          protected const int TACTIC_MAXIMIZE_WIN = 3;
          protected const int TACTIC_AVERAGE_WIN = 4;
          protected const int TACTIC_MINIMAL_WIN = 5;
          protected const int TACTIC_JUST_WAIT = 6;
          protected const int TACTIC_PASS = 7;
          protected const int TACTIC_WAIT_DUMMY = 8;
          protected const int TACTIC_SPY = 9;
          protected const int TACTIC_SPY_DUMMY_BEST_THEN_PASS = 10;
          
          private const int SortType_None = 0;
          private const int SortType_StrategicValue = 1;
          private const int SortType_PowerChange = 2;
          
          protected int attitude;
          protected int chances;
          protected bool waitingForTimer { get; set; }
          protected Timer waitingTimer { get; set; }//not needed?
          protected bool _currentRoundCritical = false;
          private SafeRandom random = new SafeRandom();
          protected Action actionWaitingTimerEnded;

          public AIPlayerController()
          {
               actionWaitingTimerEnded = () => onWaitingTimerEnded();
               isAI = true;
               _stateMachine.AddState("Idle", state_begin_Idle, null, state_end_Idle);
               _stateMachine.AddState("ChoosingMove", state_begin_ChoseMove, state_update_ChooseMove, null);
               _stateMachine.AddState("SendingCardToTransaction", state_begin_SendingCard, state_update_SendingCard, null);
               _stateMachine.AddState("DelayBetweenActions", state_begin_DelayAction, state_update_DelayAction, null);
               _stateMachine.AddState("ApplyingCard", state_begin_ApplyingCard, state_update_ApplyingCard, null);
          }

          protected void state_begin_Idle()
          {
               if (attitude == TACTIC_PASS)
               {
                    currentRoundStatus = BasePlayerController.ROUND_PLAYER_STATUS_DONE;
               }
               _turnOver = true;
               if (CardManager.getInstance().getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, playerID).Count == 0 && 
                    CardManager.getInstance().getCardLeader(playerID) != null && 
                    CardManager.getInstance().getCardLeader(playerID).canBeUsed == false)
               {
                    currentRoundStatus = BasePlayerController.ROUND_PLAYER_STATUS_DONE;
               }
               if (_boardRenderer != null)
               {
                    _boardRenderer.getCardHolder(CardManager.CARD_LIST_LOC_LEADER, playerID).updateLeaderStatus(false);
               }
          }

          protected void state_end_Idle()
          {
               if (_boardRenderer != null)
               {
                    _boardRenderer.getCardHolder(CardManager.CARD_LIST_LOC_LEADER, playerID).updateLeaderStatus(true);
               }
          }

          protected void state_begin_ChoseMove()
          {
               CardManager.getInstance().CalculateCardPowerPotentials();
               ChooseAttitude();
               string _attitudeChosen = attitudeToString(attitude);
               Console.WriteLine("GFX -#AI# ai has decided to use the following attitude:{0}", _attitudeChosen);
               _decidedCardTransaction = decideWhichCardToPlay();
               if (_decidedCardTransaction == null && attitude != TACTIC_PASS)
               {
                    attitude = TACTIC_PASS;
               }
               else if (_currentRoundCritical &&
                  _decidedCardTransaction != null &&
                  !_decidedCardTransaction.sourceCardInstanceRef.templateRef.hasEffect(CardTemplate.CardEffect_UnsummonDummy) &&
                  _decidedCardTransaction.powerChangeResult < 0 &&
                  CardManager.getInstance().getAllCreaturesInHand(playerID).Count == 0)
               {
                    _decidedCardTransaction = null;
                    attitude = TACTIC_PASS;
               }
               Console.WriteLine("GFX -#AI# the ai decided on the following transaction: {0}", _decidedCardTransaction);
          }

          protected void state_update_ChooseMove()
          {
               if (attitude == TACTIC_PASS || _decidedCardTransaction == null)
               {
                    _stateMachine.ChangeState("Idle");
                    if (attitude != TACTIC_PASS)
                    {
                         Console.WriteLine("GFX -#AI#--------------- WARNING ---------- AI is passing since chosen tactic was unable to find a transaction it liked");
                    }
                    attitude = TACTIC_PASS;
               }
               else
               {
                    _stateMachine.ChangeState("SendingCardToTransaction");
               }
          }

          protected void state_begin_SendingCard()
          {
               Console.WriteLine("GFX -#AI# AI is sending the following card into transaction: {0}", _decidedCardTransaction.sourceCardInstanceRef);
               startCardTransaction(_decidedCardTransaction.sourceCardInstanceRef.instanceId);
          }

          protected void state_update_SendingCard()
          {
               if (!CardTweenManager.getInstance().isAnyCardMoving())
               {
                    _stateMachine.ChangeState("DelayBetweenActions");
               }
          }

          protected void state_begin_DelayAction()
          {
               waitingForTimer = true;
               ExecuteDelay(actionWaitingTimerEnded, 1200);
          }

          protected void state_update_DelayAction()
          {
               if (!waitingForTimer)
               {
                    _stateMachine.ChangeState("ApplyingCard");
               }
          }

          protected async void ExecuteDelay(Action action, int timeoutInMilliseconds)
          {
               await Task.Delay(timeoutInMilliseconds);
               action();
          }

          protected void onWaitingTimerEnded()
          {
               waitingForTimer = false;
               waitingTimer = null;
          }

          protected CardTransaction decideWhichCardToPlay()
          {
               List<CardInstance> cardInHandWithEffect = null;
               int counter = 0;
               CardTransaction optimalTransaction = null;
               CardInstance firstCardInHandWithEffect = null;
               CardInstance highLow = null;
               int betterAverage = 0;
               int minMax = 0;
               CardManager cardManager = CardManager.getInstance();
               int playerScore = cardManager.currentPlayerScores[playerID];
               int opponentScore = cardManager.currentPlayerScores[opponentID];
               int deltaScore = playerScore - opponentScore;
               switch (attitude)
               {
                    case TACTIC_SPY_DUMMY_BEST_THEN_PASS:
                         {
                              firstCardInHandWithEffect = cardManager.getFirstCardInHandWithEffect(CardTemplate.CardEffect_Draw2, playerID);
                              if (firstCardInHandWithEffect != null)
                              {
                                   return firstCardInHandWithEffect.getOptimalTransaction();
                              }
                              firstCardInHandWithEffect = cardManager.getFirstCardInHandWithEffect(CardTemplate.CardEffect_UnsummonDummy, playerID);
                              if (firstCardInHandWithEffect != null)
                              {
                                   highLow = cardManager.getHigherOrLowerValueTargetCardOnBoard(firstCardInHandWithEffect, playerID, true, true);
                                   if (highLow != null)
                                   {
                                        optimalTransaction = firstCardInHandWithEffect.getOptimalTransaction();
                                        optimalTransaction.targetCardInstanceRef = highLow;
                                        return optimalTransaction;
                                   }
                              }
                              attitude = TACTIC_PASS;
                              break;
                         }
                    case TACTIC_MINIMIZE_LOSS:
                         {
                              firstCardInHandWithEffect = cardManager.getFirstCardInHandWithEffect(CardTemplate.CardEffect_UnsummonDummy, playerID);
                              if (firstCardInHandWithEffect != null)
                              {
                                   highLow = getHighestValueCardOnBoard();
                                   if (highLow != null)
                                   {
                                        optimalTransaction = firstCardInHandWithEffect.getOptimalTransaction();
                                        optimalTransaction.targetCardInstanceRef = highLow;
                                        return optimalTransaction;
                                   }
                              }
                              firstCardInHandWithEffect = cardManager.getFirstCardInHandWithEffect(CardTemplate.CardEffect_Draw2, playerID);
                              if (firstCardInHandWithEffect != null)
                              {
                                   return firstCardInHandWithEffect.getOptimalTransaction();
                              }
                              attitude = TACTIC_PASS;
                              break;
                         }
                    case TACTIC_MINIMIZE_WIN:
                         {
                              firstCardInHandWithEffect = cardManager.getFirstCardInHandWithEffect(CardTemplate.CardEffect_UnsummonDummy, playerID);
                              if (firstCardInHandWithEffect != null)
                              {
                                   highLow = getHighestValueCardOnBoardWithEffectLessThan(deltaScore);
                                   if (highLow != null)
                                   {
                                        optimalTransaction = firstCardInHandWithEffect.getOptimalTransaction();
                                        if (optimalTransaction != null)
                                        {
                                             optimalTransaction.targetCardInstanceRef = highLow;
                                             return optimalTransaction;
                                        }
                                   }
                              }
                              cardInHandWithEffect = cardManager.getCardsInHandWithEffect(CardTemplate.CardEffect_Draw2, playerID);
                              counter = 0;
                              while (counter < cardInHandWithEffect.Count)
                              {
                                   firstCardInHandWithEffect = cardInHandWithEffect[counter];
                                   if (firstCardInHandWithEffect != null && Math.Abs(firstCardInHandWithEffect.getOptimalTransaction().powerChangeResult) < Math.Abs(deltaScore))
                                   {
                                        return firstCardInHandWithEffect.getOptimalTransaction();
                                   }
                                   ++counter;
                              }
                              attitude = TACTIC_PASS;
                              break;
                         }
                    case TACTIC_MAXIMIZE_WIN:
                         {
                              cardInHandWithEffect = getCardsBasedOnCriteria(SortType_PowerChange);
                              if (cardInHandWithEffect.Count > 0)
                              {
                                   highLow = cardInHandWithEffect[(cardInHandWithEffect.Count - 1)];
                                   if (highLow != null)
                                   {
                                        return highLow.getOptimalTransaction();
                                   }
                              }
                              break;
                         }
                    case TACTIC_AVERAGE_WIN:
                         {
                              cardInHandWithEffect = getCardsBasedOnCriteria(SortType_PowerChange);
                              betterAverage = -1;
                              while (counter < cardInHandWithEffect.Count && betterAverage == -1)
                              {
                                   firstCardInHandWithEffect = cardInHandWithEffect[counter];
                                   if (firstCardInHandWithEffect.getOptimalTransaction().powerChangeResult > Math.Abs(deltaScore))
                                   {
                                        betterAverage = counter;
                                   }
                                   ++counter;
                              }
                              if (betterAverage == -1)
                              {
                                   if (cardInHandWithEffect.Count > 0)
                                   {
                                        highLow = cardInHandWithEffect[(cardInHandWithEffect.Count - 1)];
                                        if (highLow != null)
                                        {
                                             return highLow.getOptimalTransaction();
                                        }
                                   }
                              }
                              else
                              {
                                   minMax = (int)Math.Min(betterAverage, Math.Max((cardInHandWithEffect.Count - 1), betterAverage + Math.Floor(random.NextDouble() * ((cardInHandWithEffect.Count - 1) - betterAverage))));
                                   highLow = cardInHandWithEffect[minMax];
                                   if (highLow != null)
                                   {
                                        return highLow.getOptimalTransaction();
                                   }
                              }
                              break;
                         }
                    case TACTIC_MINIMAL_WIN:
                         {
                              cardInHandWithEffect = getCardsBasedOnCriteria(SortType_PowerChange);
                              counter = 0;
                              while (counter < cardInHandWithEffect.Count)
                              {
                                   firstCardInHandWithEffect = cardInHandWithEffect[counter];
                                   if (firstCardInHandWithEffect.getOptimalTransaction().powerChangeResult > Math.Abs(deltaScore))
                                   {
                                        highLow = firstCardInHandWithEffect;
                                        break;
                                   }
                                   ++counter;
                              }
                              if (highLow == null && cardInHandWithEffect.Count > 0)
                              {
                                   highLow = cardInHandWithEffect[(cardInHandWithEffect.Count - 1)];
                              }
                              if (highLow != null)
                              {
                                   return highLow.getOptimalTransaction();
                              }
                              break;
                         }
                    case TACTIC_JUST_WAIT:
                         {
                              cardInHandWithEffect = getCardsBasedOnCriteria(SortType_StrategicValue);
                              if (cardInHandWithEffect.Count == 0)
                              {
                                   return null;
                              }
                              counter = 0;
                              while (counter < cardInHandWithEffect.Count)
                              {
                                   optimalTransaction = cardInHandWithEffect[counter].getOptimalTransaction();
                                   if (optimalTransaction != null)
                                   {
                                        if (_currentRoundCritical)
                                        {
                                             if (optimalTransaction != null && optimalTransaction.sourceCardInstanceRef.templateRef.isType(CardTemplate.CardType_Weather) && (optimalTransaction.powerChangeResult < 0 || optimalTransaction.powerChangeResult < optimalTransaction.sourceCardInstanceRef.potentialWeatherHarm()))
                                             {
                                                  optimalTransaction = null;
                                             }
                                             else
                                             {
                                                  break;
                                             }
                                        }
                                        else
                                        {
                                             break;
                                        }
                                   }
                                   ++counter;
                              }
                              return optimalTransaction;
                         }
                    case TACTIC_WAIT_DUMMY:
                         {
                              firstCardInHandWithEffect = cardManager.getFirstCardInHandWithEffect(CardTemplate.CardEffect_UnsummonDummy, playerID);
                              if (firstCardInHandWithEffect != null)
                              {
                                   optimalTransaction = firstCardInHandWithEffect.getOptimalTransaction();
                                   if (optimalTransaction.targetCardInstanceRef == null)
                                   {
                                        optimalTransaction.targetCardInstanceRef = cardManager.getHigherOrLowerValueTargetCardOnBoard(firstCardInHandWithEffect, playerID, false);
                                   }
                                   if (optimalTransaction.targetCardInstanceRef != null)
                                   {
                                        return optimalTransaction;
                                   }
                              }
                              Console.WriteLine("GFX [ WARNING ] -#AI#---- Uh oh, was in TACTIC_WAIT_DUMMY but was unable to get a valid dummy transaction :S");
                              break;
                         }
                    case TACTIC_SPY:
                         {
                              firstCardInHandWithEffect = cardManager.getFirstCardInHandWithEffect(CardTemplate.CardEffect_Draw2, playerID);
                              if (firstCardInHandWithEffect != null)
                              {
                                   return firstCardInHandWithEffect.getOptimalTransaction();
                              }
                              break;
                         }
               }
               if (!(attitude == TACTIC_PASS) && !(attitude == TACTIC_MINIMIZE_WIN))
               {
                    firstCardInHandWithEffect = cardManager.getFirstCardInHandWithEffect(CardTemplate.CardEffect_Draw2, playerID);
                    if (firstCardInHandWithEffect != null)
                    {
                         return firstCardInHandWithEffect.getOptimalTransaction();
                    }
               }
               return null;
          }

          private void ChooseAttitude()
          {
               int counter = 0;
               int roundResultsCounter = 0;
               int id = 0;
               int median = 0;
               CardInstance cardInstance = null;
               CardManager cardManager = CardManager.getInstance();
               List<CardInstance> cardInstanceList = new List<CardInstance>();
               cardInstanceList = cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, playerID);
               if (cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, playerID).Count == 0)
               {
                    attitude = TACTIC_PASS;
                    return;
               }
               bool isPlayer = false;
               bool isOpponent = false;
               int creaturesCount = 0;
               int unsummonDummyCounter = 0;
               int draw2Counter = 0;
               roundResultsCounter = 0;
               while (roundResultsCounter < cardManager.roundResults.Count)
               {
                    if (cardManager.roundResults[roundResultsCounter].played)
                    {
                         id = cardManager.roundResults[roundResultsCounter].winningPlayer;
                         if (id == playerID || id == CardManager.PLAYER_INVALID)
                         {
                              isPlayer = true;
                         }
                         if (id == opponentID || id == CardManager.PLAYER_INVALID)
                         {
                              isOpponent = true;
                         }
                    }
                    ++roundResultsCounter;
               }
               _currentRoundCritical = isOpponent;
               counter = 0;
               while (counter < cardInstanceList.Count)
               {
                    if (cardInstanceList[counter].templateRef.isType(CardTemplate.CardType_Creature))
                    {
                         ++creaturesCount;
                    }
                    ++counter;
               }
               int playerCardsHand = cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, playerID).Count;
               int opponentCardsHand = cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, opponentID).Count;
               int deltaCards = playerCardsHand - opponentCardsHand;
               int deltaScores = cardManager.currentPlayerScores[playerID] - cardManager.currentPlayerScores[opponentID];
               int opponentCurrentRoundStatus = gameFlowControllerRef.playerControllers[opponentID].currentRoundStatus;
               Console.WriteLine("GFX -#AI# ###############################################################################");
               Console.WriteLine("GFX -#AI#---------------------------- AI Deciding his next move --------------------------------");
               Console.WriteLine("GFX -#AI#------ previousTactic: " + attitudeToString(attitude));
               Console.WriteLine("GFX -#AI#------ playerCardsInHand: " + playerCardsHand);
               Console.WriteLine("GFX -#AI#------ opponentCardsInHand: " + opponentCardsHand);
               Console.WriteLine("GFX -#AI#------ cardAdvantage: " + deltaCards);
               Console.WriteLine("GFX -#AI#------ scoreDifference: " + deltaScores + ", his score: " + cardManager.currentPlayerScores[playerID] + ", enemy score: " + cardManager.currentPlayerScores[opponentID]);
               Console.WriteLine("GFX -#AI#------ opponent has won: " + isOpponent);
               Console.WriteLine("GFX -#AI#------ has won: " + isPlayer);
               Console.WriteLine("GFX -#AI#------ Num units in hand: " + creaturesCount);
               if (gameFlowControllerRef.playerControllers[opponentID].currentRoundStatus != ROUND_PLAYER_STATUS_DONE)
               {
                    Console.WriteLine("GFX -#AI#------ has opponent passed: false");
               }
               else
               {
                    Console.WriteLine("GFX -#AI#------ has opponent passed: true");
               }
               Console.WriteLine("GFX =#AI#=======================================================================================");
               Console.WriteLine("GFX -#AI#-----------------------------   AI CARDS AT HAND   ------------------------------------");
               counter = 0;
               while (counter < cardInstanceList.Count)
               {
                    Console.WriteLine("GFX -#AI# Card Points[ ", cardInstanceList[counter].templateRef.power, " ], Card -", cardInstanceList[counter]);
                    ++counter;
               }
               Console.WriteLine("GFX =#AI#=======================================================================================");
               int playerDeckFaction = cardManager.playerDeckDefinitions[playerID].getDeckFaction();
               int opponentDeckFaction = cardManager.playerDeckDefinitions[opponentID].getDeckFaction();
               int slotsEffectsCounter = cardManager.getCardsInSlotIdWithEffect(CardTemplate.CardEffect_Draw2, opponentID).Count;
               if (playerDeckFaction == CardTemplate.FactionId_Nilfgaard && !(opponentDeckFaction == CardTemplate.FactionId_Nilfgaard) && opponentCurrentRoundStatus == ROUND_PLAYER_STATUS_DONE && deltaScores == 0)
               {
                    attitude = TACTIC_PASS;
               }
               else if (!isOpponent && attitude == TACTIC_SPY_DUMMY_BEST_THEN_PASS)
               {
                    if (opponentCurrentRoundStatus != ROUND_PLAYER_STATUS_DONE)
                    {
                         attitude = TACTIC_SPY_DUMMY_BEST_THEN_PASS;
                    }
               }
               else if (!isOpponent && cardManager.getFirstCardInHandWithEffect(CardTemplate.CardEffect_Draw2, playerID) != null && (random.NextDouble() < 0.2 || slotsEffectsCounter > 1) && attitude != TACTIC_SPY_DUMMY_BEST_THEN_PASS)
               {
                    attitude = TACTIC_SPY;
               }
               else if (attitude == TACTIC_SPY && !(cardManager.getFirstCardInHandWithEffect(CardTemplate.CardEffect_Draw2, playerID) == null))
               {
                    attitude = TACTIC_SPY;
               }
               else if (opponentCurrentRoundStatus != ROUND_PLAYER_STATUS_DONE)
               {
                    if (deltaScores > 0)
                    {
                         if (isOpponent)
                         {
                              attitude = TACTIC_JUST_WAIT;
                         }
                         else
                         {
                              median = creaturesCount * creaturesCount / 36;
                              attitude = TACTIC_NONE;
                              if (isPlayer)
                              {
                                   unsummonDummyCounter = cardManager.getCardsInHandWithEffect(CardTemplate.CardEffect_UnsummonDummy, playerID).Count;
                                   draw2Counter = cardManager.getCardsInHandWithEffect(CardTemplate.CardEffect_Draw2, playerID).Count;
                                   if (random.NextDouble() < 0.2 || playerCardsHand == unsummonDummyCounter + draw2Counter)
                                   {
                                        attitude = TACTIC_SPY_DUMMY_BEST_THEN_PASS;
                                   }
                                   else
                                   {
                                        cardInstance = cardManager.getFirstCardInHandWithEffect(CardTemplate.CardEffect_UnsummonDummy, playerID);
                                        if (!(cardInstance == null) && !(cardManager.getHigherOrLowerValueTargetCardOnBoard(cardInstance, playerID, false) == null))
                                        {
                                             attitude = TACTIC_WAIT_DUMMY;
                                        }
                                        else if (random.NextDouble() < deltaScores / 30 && random.NextDouble() < median)
                                        {
                                             attitude = TACTIC_MAXIMIZE_WIN;
                                        }
                                   }
                              }
                              if (attitude == TACTIC_NONE)
                              {
                                   if (random.NextDouble() < playerCardsHand / 10 || playerCardsHand > 8)
                                   {
                                        if (random.NextDouble() < 0.2 || playerCardsHand == unsummonDummyCounter + draw2Counter)
                                        {
                                             attitude = TACTIC_SPY_DUMMY_BEST_THEN_PASS;
                                        }
                                        else
                                        {
                                             attitude = TACTIC_JUST_WAIT;
                                        }
                                   }
                                   else
                                   {
                                        attitude = TACTIC_PASS;
                                   }
                              }
                         }
                    }
                    else if (isPlayer)
                    {
                         unsummonDummyCounter = cardManager.getCardsInHandWithEffect(CardTemplate.CardEffect_UnsummonDummy, playerID).Count;
                         draw2Counter = cardManager.getCardsInHandWithEffect(CardTemplate.CardEffect_Draw2, playerID).Count;
                         if (!isOpponent && (random.NextDouble() < 0.2 || playerCardsHand == unsummonDummyCounter + draw2Counter))
                         {
                              attitude = TACTIC_SPY_DUMMY_BEST_THEN_PASS;
                         }
                         else
                         {
                              attitude = TACTIC_MAXIMIZE_WIN;
                         }
                    }
                    else if (isOpponent)
                    {
                         attitude = TACTIC_MINIMAL_WIN;
                    }
                    else if (!cardManager.roundResults[0].played && deltaScores < -11 && random.NextDouble() < (Math.Abs(deltaScores) - 10) / 20)
                    {
                         if (random.NextDouble() < 0.9)
                         {
                              attitude = TACTIC_SPY_DUMMY_BEST_THEN_PASS;
                         }
                         else
                         {
                              attitude = TACTIC_PASS;
                         }
                    }
                    else if (random.NextDouble() < playerCardsHand / 10)
                    {
                         attitude = TACTIC_MINIMAL_WIN;
                    }
                    else if (random.NextDouble() < playerCardsHand / 10)
                    {
                         attitude = TACTIC_AVERAGE_WIN;
                    }
                    else if (random.NextDouble() < playerCardsHand / 10)
                    {
                         attitude = TACTIC_MAXIMIZE_WIN;
                    }
                    else if (playerCardsHand <= 8 && random.NextDouble() > playerCardsHand / 10)
                    {
                         attitude = TACTIC_PASS;
                    }
                    else
                    {
                         attitude = TACTIC_JUST_WAIT;
                    }
               }
               else if (attitude != TACTIC_MINIMIZE_LOSS)
               {
                    if (!isOpponent && deltaScores <= 0 && random.NextDouble() < deltaScores / 20)
                    {
                         attitude = TACTIC_MINIMIZE_LOSS;
                    }
                    else if (!isPlayer && deltaScores > 0)
                    {
                         attitude = TACTIC_MINIMIZE_WIN;
                    }
                    else if (deltaScores > 0)
                    {
                         attitude = TACTIC_PASS;
                    }
                    else
                    {
                         attitude = TACTIC_MINIMAL_WIN;
                    }
               }
               else
               {
                    attitude = TACTIC_MINIMIZE_LOSS;
               }
          }

          protected string attitudeToString(int attitude)
          {
               switch (attitude)
               {
                    case TACTIC_NONE:
                         {
                              return "NONE - ERROR";
                         }
                    case TACTIC_SPY_DUMMY_BEST_THEN_PASS:
                         {
                              return "DUMMY BETS THEN PASS";
                         }
                    case TACTIC_MINIMIZE_LOSS:
                         {
                              return "MINIMIZE LOSS";
                         }
                    case TACTIC_MINIMIZE_WIN:
                         {
                              return "MINIMIZE WIN";
                         }
                    case TACTIC_MAXIMIZE_WIN:
                         {
                              return "MAXIMIZE WIN";
                         }
                    case TACTIC_AVERAGE_WIN:
                         {
                              return "AVERAGE WIN";
                         }
                    case TACTIC_MINIMAL_WIN:
                         {
                              return "MINIMAL WIN";
                         }
                    case TACTIC_JUST_WAIT:
                         {
                              return "JUST WAIT";
                         }
                    case TACTIC_PASS:
                         {
                              return "PASS";
                         }
                    case TACTIC_WAIT_DUMMY:
                         {
                              return "WAIT DUMMY";
                         }
                    case TACTIC_SPY:
                         {
                              return "SPIES";
                         }
               }
               return attitude.ToString();
          }

          protected CardInstance getHighestValueCardOnBoard()
          {
               int counter = 0;
               CardInstance currentCard = null;
               List<CardInstance> creaturesList = new List<CardInstance>();
               CardManager cardManager = CardManager.getInstance();
               cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, playerID, creaturesList);
               cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, playerID, creaturesList);
               cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, playerID, creaturesList);
               CardInstance resultCard = null;
               while (counter < creaturesList.Count)
               {
                    currentCard = creaturesList[counter];
                    if (resultCard == null || currentCard.templateRef.power + currentCard.templateRef.GetBonusValue() > resultCard.templateRef.power + resultCard.templateRef.GetBonusValue())
                    {
                         resultCard = currentCard;
                    }
                    ++counter;
               }
               return resultCard;
          }

          protected CardInstance getHighestValueCardOnBoardWithEffectLessThan(int comparison)
          {
               int counter = 0;
               CardInstance currentCard = null;
               List<CardInstance> creaturesList = new List<CardInstance>();
               CardManager cardManager = CardManager.getInstance();
               cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_MELEE, playerID, creaturesList);
               cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_RANGED, playerID, creaturesList);
               cardManager.getAllCreaturesNonHero(CardManager.CARD_LIST_LOC_SEIGE, playerID, creaturesList);
               CardInstance resultCard = null;
               while (counter < creaturesList.Count)
               {
                    currentCard = creaturesList[counter];
                    if (!currentCard.templateRef.hasEffect(CardTemplate.CardEffect_SameTypeMorale) && !currentCard.templateRef.hasEffect(CardTemplate.CardEffect_ImproveNeighbours) && currentCard.getTotalPower() < comparison && (resultCard == null || resultCard.templateRef.power + resultCard.templateRef.GetBonusValue() < currentCard.templateRef.power + currentCard.templateRef.GetBonusValue()))
                    {
                         resultCard = currentCard;
                    }
                    ++counter;
               }
               return resultCard;
          }

          protected List<CardInstance> getCardsBasedOnCriteria(int criteria)
          {
               int counter = 0;
               CardInstance currentCard = null;
               List<CardInstance> resultList = new List<CardInstance>();
               List<CardInstance> handCardsList = CardManager.getInstance().getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, playerID);
               CardManager cardManager = CardManager.getInstance();
               CardLeaderInstance leader = cardManager.getCardLeader(playerID);
               if (leader != null && leader.canBeUsed)
               {
                    leader.recalculatePowerPotential(cardManager);
                    if (leader.getOptimalTransaction().strategicValue != -1)
                    {
                         resultList.Add(leader);
                    }
               }
               counter = 0;
               while (counter < handCardsList.Count)
               {
                    currentCard = handCardsList[counter];
                    switch (criteria)
                    {
                         case SortType_None:
                              {
                                   resultList.Add(currentCard);
                                   break;
                              }
                         case SortType_PowerChange:
                              {
                                   if (currentCard.getOptimalTransaction().powerChangeResult >= 0)
                                   {
                                        resultList.Add(currentCard);
                                   }
                                   break;
                              }
                         case SortType_StrategicValue:
                              {
                                   if (currentCard.getOptimalTransaction().strategicValue >= 0)
                                   {
                                        resultList.Add(currentCard);
                                   }
                                   break;
                              }
                    }
                    ++counter;
               }
               switch (criteria)
               {
                    case SortType_StrategicValue:
                         {
                              resultList.Sort(strategicValueSorter);
                              break;
                         }
                    case SortType_PowerChange:
                         {
                              resultList.Sort(powerChangeSorter);
                              break;
                         }
               }
               return resultList;
          }

          protected int strategicValueSorter(CardInstance card1, CardInstance card2)
          {
               return card1.getOptimalTransaction().strategicValue - card2.getOptimalTransaction().strategicValue;
          }

          protected int powerChangeSorter(CardInstance card1, CardInstance card2)
          {
               if (card1.getOptimalTransaction().powerChangeResult == card2.getOptimalTransaction().powerChangeResult)
               {
                    return card1.getOptimalTransaction().strategicValue - card2.getOptimalTransaction().strategicValue;
               }
               return card1.getOptimalTransaction().powerChangeResult - card2.getOptimalTransaction().powerChangeResult;
          }

          public override void startTurn()
          {
               if (currentRoundStatus == BasePlayerController.ROUND_PLAYER_STATUS_DONE)
               {
                    return;
               }
               base.startTurn();
               _stateMachine.ChangeState("ChoosingMove");
          }
     }
}
