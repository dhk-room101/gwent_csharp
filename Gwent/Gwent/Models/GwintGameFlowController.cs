using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class GwintGameFlowController
     {
          //public var mcMessageQueue:red.game.witcher3.controls.W3MessageQueue;
          //public var mcTutorials:GwintTutorial;
          //public var mcChoiceDialog:red.game.witcher3.controls.W3ChoiceDialog;
          //public var mcEndGameDialog:GwintEndGameDialog;
          //protected var _skipButton:red.game.witcher3.controls.InputFeedbackButton;
          
          private Random random = new Random();
          public const string COIN_TOSS_POPUP_NEEDED = "Gameflow.event.Cointoss.needed";
          public Action closeMenuFunctor;
          public List<BasePlayerController> playerControllers;
          public FiniteStateMachine stateMachine;
          private CardManager cardManager;
          protected static GwintGameFlowController _instance;

          protected bool _tutorial = true;
          private bool sawStartMessage;
          public bool gameStarted = false;
          protected bool sawScoreChangeTutorial = false;
          protected bool sawRoundStartTutorial = false;
          protected bool sawEndGameTutorial = false;
          protected bool _mulliganDecided;
          protected bool sawRoundEndTutorial = false;
          protected bool allNeutralInRound = true;
          protected bool playedCreaturesInRound = false;
          protected bool playedThreeHeroesOneRound = false;

          protected int _mulliganCardsCount = 0;
          protected int lastRoundWinner = -1;
          private int currentRound;
          public int currentPlayer = -1;

          public GwintGameFlowController()
          {
               _instance = this;

               stateMachine = new FiniteStateMachine();

               stateMachine.AddState("Initializing", null, state_update_Initializing, state_leave_Initializing);
               stateMachine.AddState("Tutorials", state_begin_Tutorials, state_update_Tutorials, null);
               stateMachine.AddState("SpawnLeaders", state_begin_SpawnLeaders, state_update_SpawnLeaders, null);
               stateMachine.AddState("CoinToss", state_begin_CoinToss, state_update_CoinToss, null);
               stateMachine.AddState("Mulligan", state_begin_Mulligan, state_update_Mulligan, null);
               stateMachine.AddState("RoundStart", state_begin_RoundStart, state_update_RoundStart, null);
               stateMachine.AddState("PlayerTurn", state_begin_PlayerTurn, state_update_PlayerTurn, state_leave_PlayerTurn);
               stateMachine.AddState("ChangingPlayer", state_begin_ChangingPlayer, state_update_ChangingPlayer, null);
               stateMachine.AddState("ShowingRoundResult", state_begin_ShowingRoundResult, state_update_ShowingRoundResult, null);
               stateMachine.AddState("ClearingBoard", state_begin_ClearingBoard, state_update_ClearingBoard, state_leave_ClearingBoard);
               stateMachine.AddState("ShowingFinalResult", state_begin_ShowingFinalResult, state_update_ShowingFinalResult, null);
               stateMachine.AddState("Reset", state_begin_reset, null, null);

               playerControllers = new List<BasePlayerController>();

               var humanPlayer = new HumanPlayerController();
               humanPlayer.gameFlowControllerRef = this;
               humanPlayer.playerID = CardManager.PLAYER_1;
               humanPlayer.opponentID = CardManager.PLAYER_2;
               //(humanPlayer as HumanPlayerController).skipButton = _skipButton;
               playerControllers.Add(humanPlayer);

               var aiPlayer = new AIPlayerController();
               aiPlayer.gameFlowControllerRef = this;
               aiPlayer.playerID = CardManager.PLAYER_2;
               aiPlayer.opponentID = CardManager.PLAYER_1;
               playerControllers.Add(aiPlayer);

               currentRound = 0;
          }

          protected void state_update_Initializing()
          {
               if (CardManager.getInstance() != null &&
                   CardManager.getInstance().cardTemplatesReceived &&
                   CardFXManager.getInstance() != null)
               {
                    stateMachine.ChangeState("Tutorials");
               }
          }

          protected void state_leave_Initializing()
          {
               cardManager = CardManager.getInstance();
               if (cardManager == null)
               {
                    throw new ArgumentException("GFX --- Tried to link reference to card manager after initializing, was unable to!");
               }
               if (playerControllers[CardManager.PLAYER_1] is HumanPlayerController)
               {
                    Console.WriteLine("game flow controller: choice dialogue not implemented yet!");
                    //(playerControllers[CardManager.PLAYER_1] as HumanPlayerController).setChoiceDialog(mcChoiceDialog);
               }
               playerControllers[CardManager.PLAYER_1].boardRenderer = cardManager.boardRenderer;
               playerControllers[CardManager.PLAYER_2].boardRenderer = cardManager.boardRenderer;
               playerControllers[CardManager.PLAYER_1].playerRenderer = cardManager.playerRenderers[CardManager.PLAYER_1];
               playerControllers[CardManager.PLAYER_2].playerRenderer = cardManager.playerRenderers[CardManager.PLAYER_2];
               stateMachine.pauseOnStateChangeIfFunc = () => shouldDisallowStateChangeFunc();
               Console.WriteLine("game flow controller: skip button not implemented yet!");
               /*if (_skipButton != null) 
               {
                   loc1 = playerControllers[CardManager.PLAYER_1] as HumanPlayerController;
                   if (loc1) 
                   {
                       loc1.skipButton = _skipButton;
                   }
               }*/
          }

          protected void state_begin_Tutorials()
          {

          }

          protected void state_update_Tutorials()
          {
               stateMachine.ChangeState("SpawnLeaders");
               Console.WriteLine("game flow controller: updates tutorial state not implemented yet!");
               /*if (!mcTutorials.visible || mcTutorials.isPaused) 
               {
                   stateMachine.ChangeState("SpawnLeaders");
                   red.game.witcher3.managers.InputFeedbackManager.cleanupButtons();
                   red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.navigate, scaleform.clik.constants.NavigationCode.GAMEPAD_L3, -1, "panel_button_common_navigation");
                   red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.quitGame, scaleform.clik.constants.NavigationCode.START, red.core.constants.KeyCode.Q, "gwint_pass_game");
               }*/
          }

          protected void state_begin_SpawnLeaders()
          {
               Console.WriteLine("GFX ##########################################################");
               Console.WriteLine("GFX -#AI#-----------------------------------------------------------------------------------------------------");
               Console.WriteLine("GFX -#AI#----------------------------- NEW GWINT GAME ------------------------------------");
               Console.WriteLine("GFX -#AI#-----------------------------------------------------------------------------------------------------");
               cardManager.spawnLeaders();
               gameStarted = false;
               playedThreeHeroesOneRound = false;
               CardLeaderInstance leaderP1 = cardManager.getCardLeader(CardManager.PLAYER_1);
               CardLeaderInstance leaderP2 = cardManager.getCardLeader(CardManager.PLAYER_2);
               if (playerControllers[CardManager.PLAYER_1] is HumanPlayerController)
               {
                    Console.WriteLine("attachToTutorialCarouselMessage not implemented yet!");
                    //(playerControllers[CardManager.PLAYER_1] as HumanPlayerController).attachToTutorialCarouselMessage();
               }
               playerControllers[CardManager.PLAYER_1].cardZoomEnabled = false;
               playerControllers[CardManager.PLAYER_2].cardZoomEnabled = false;
               playerControllers[CardManager.PLAYER_1].inputEnabled = true;
               playerControllers[CardManager.PLAYER_2].inputEnabled = true;
               if (!(leaderP1 == null) && !(leaderP2 == null) && !(leaderP1.templateId == leaderP2.templateId))
               {
                    if (leaderP1.templateRef.getFirstEffect() == CardTemplate.CardEffect_Counter_King || leaderP2.templateRef.getFirstEffect() == CardTemplate.CardEffect_Counter_King)
                    {
                         if (leaderP1.templateRef.getFirstEffect() != leaderP2.templateRef.getFirstEffect())
                         {
                              Console.WriteLine("game flow controller:beginning spawn leaders: counter king effect not implemented yet!");
                              if (leaderP1.templateRef.getFirstEffect() != CardTemplate.CardEffect_Counter_King)
                              {
                                   //mcMessageQueue.PushMessage("[[gwint_opponent_counter_leader]]");
                                   //GwintGameMenu.mSingleton.playSound("gui_gwint_using_ability");
                              }
                              else
                              {
                                   //mcMessageQueue.PushMessage("[[gwint_player_counter_leader]]");
                                   //GwintGameMenu.mSingleton.playSound("gui_gwint_using_ability");
                              }
                         }
                         leaderP1.canBeUsed = false;
                         leaderP2.canBeUsed = false;
                         if (cardManager.boardRenderer != null)
                         {
                              cardManager.boardRenderer.getCardHolder(CardManager.CARD_LIST_LOC_LEADER, CardManager.PLAYER_1).updateLeaderStatus(false);
                              cardManager.boardRenderer.getCardHolder(CardManager.CARD_LIST_LOC_LEADER, CardManager.PLAYER_2).updateLeaderStatus(false);
                         }
                    }
               }
               playerControllers[CardManager.PLAYER_1].currentRoundStatus = BasePlayerController.ROUND_PLAYER_STATUS_ACTIVE;
               playerControllers[CardManager.PLAYER_2].currentRoundStatus = BasePlayerController.ROUND_PLAYER_STATUS_ACTIVE;
          }

          protected void state_update_SpawnLeaders()
          {
               stateMachine.ChangeState("CoinToss");
          }

          protected void state_begin_CoinToss()
          {
               int factionP1 = cardManager.playerDeckDefinitions[CardManager.PLAYER_1].getDeckFaction();
               int factionP2 = cardManager.playerDeckDefinitions[CardManager.PLAYER_2].getDeckFaction();
               Console.WriteLine("GFX - Coing flip logic, player1faction:", factionP1, ", player2Faction:", factionP2);
               if (!(factionP1 == factionP2) && (factionP1 == CardTemplate.FactionId_Scoiatael || factionP2 == CardTemplate.FactionId_Scoiatael))
               //if (!(factionP1 == factionP2) && !mcTutorials.visible && (factionP1 == CardTemplate.FactionId_Scoiatael || factionP2 == CardTemplate.FactionId_Scoiatael)) 
               {
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_scoia_tael_ability");
                    if (factionP1 != CardTemplate.FactionId_Scoiatael)
                    {
                         currentPlayer = CardManager.PLAYER_2;
                         //mcMessageQueue.PushMessage("[[gwint_opponent_scoiatael_start_special]]", "sco_ability");
                    }
                    else
                    {
                         currentPlayer = CardManager.PLAYER_INVALID;
                         Console.WriteLine("game flow controller: coin toss dispatch events needed");
                         //dispatchEvent(new flash.events.Event(COIN_TOSS_POPUP_NEEDED, false, false));
                    }
               }
               else
               {
                    //if (mcTutorials.visible) 
                    Console.WriteLine("game flow controller: coin toss defaults to tutorial/player");
                    if (tutorial)
                    {
                         currentPlayer = CardManager.PLAYER_1;
                    }
                    else
                    {
                         currentPlayer = (int)Math.Floor(random.NextDouble() * 2);
                    }
                    if (currentPlayer != CardManager.PLAYER_1)
                    {
                         if (currentPlayer == CardManager.PLAYER_2)
                         {
                              //mcMessageQueue.PushMessage("[[gwint_opponent_will_go_first]]", "coin_flip_loss");
                         }
                    }
                    else
                    {
                         //mcMessageQueue.PushMessage("[[gwint_player_will_go_first_message]]", "coin_flip_win");
                    }
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_coin_toss");
               }
          }

          protected void state_update_CoinToss()
          {
               if (currentPlayer != CardManager.PLAYER_INVALID)
               {
                    stateMachine.ChangeState("Mulligan");
               }
          }

          protected void state_begin_Mulligan()
          {
               List<CardInstance> playerCardsList = null;
               _mulliganDecided = false;
               _mulliganCardsCount = 0;
               cardManager.shuffleAndDrawCards();
               playerCardsList = cardManager.getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, CardManager.PLAYER_1);
               Console.WriteLine("game flow controller: states  begin Mulligan not implemented yet ");
               //mcChoiceDialog.showDialogCardInstances(playerCardsList, handleAcceptMulligan, handleDeclineMulligan, "[[gwint_can_choose_card_to_redraw]]");
               //mcChoiceDialog.appendDialogText(" 0/2");
               /*if (mcTutorials.visible) 
               {
                   mcTutorials.continueTutorial();
                   mcChoiceDialog.inputEnabled = false;
                   mcTutorials.hideCarouselCB = handleDeclineMulligan;
                   mcTutorials.changeChoiceCB = handleForceCardSelected;
               }*/
               //GwintGameMenu.mSingleton.playSound("gui_gwint_draw_2");
          }

          protected void state_update_Mulligan()
          {
               _mulliganDecided = true;
               stateMachine.ChangeState("RoundStart");
               Console.WriteLine("game flow controller: state update Mulligan not implemented");
               /*if (_mulliganDecided && (!mcTutorials.visible || mcTutorials.isPaused)) 
            {
                stateMachine.ChangeState("RoundStart");
                mcChoiceDialog.hideDialog();
                gameStarted = true;
                playerControllers[CardManager.PLAYER_1].cardZoomEnabled = true;
                playerControllers[CardManager.PLAYER_2].cardZoomEnabled = true;
                GwintGameMenu.mSingleton.playSound("gui_gwint_game_start");
            }*/
          }

          protected void state_begin_RoundStart()
          {
               Console.WriteLine("game flow controller: state begin around start not fully implemented");
               //mcMessageQueue.PushMessage("[[gwint_round_start]]", "round_start");
               allNeutralInRound = true;
               playedCreaturesInRound = false;
               if (!(lastRoundWinner == CardManager.PLAYER_INVALID) && cardManager.playerDeckDefinitions[lastRoundWinner].getDeckFaction() == CardTemplate.FactionId_Northern_Kingdom)
               {
                    //mcMessageQueue.PushMessage("[[gwint_northern_ability_triggered]]", "north_ability", onShowNorthAbilityShown, null);
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_northern_realms_ability");
               }
          }

          protected void state_update_RoundStart()
          {
               //if (!mcMessageQueue.ShowingMessage()) 
               //{
               Console.WriteLine("game flow controller: state update drowned start tutorial not implemented");
               /*if (mcTutorials.visible && !sawRoundStartTutorial) 
               {
                   sawRoundStartTutorial = true;
                   mcTutorials.continueTutorial();
               }
               playerControllers[CardManager.PLAYER_1].resetCurrentRoundStatus();
               playerControllers[CardManager.PLAYER_2].resetCurrentRoundStatus();
               if (playerControllers[currentPlayer].currentRoundStatus != BasePlayerController.ROUND_PLAYER_STATUS_DONE) 
               {
                   stateMachine.ChangeState("PlayerTurn");
               }*/
               //else 
               //  {
               currentPlayer = currentPlayer != CardManager.PLAYER_1 ? CardManager.PLAYER_1 : CardManager.PLAYER_2;
               if (playerControllers[currentPlayer].currentRoundStatus != BasePlayerController.ROUND_PLAYER_STATUS_DONE)
               {
                    stateMachine.ChangeState("PlayerTurn");
               }
               else
               {
                    stateMachine.ChangeState("ShowingRoundResult");
               }
               //  }
               //}
          }

          protected void state_begin_PlayerTurn()
          {
               Console.WriteLine("GFX -#AI# starting player turn for player: " + currentPlayer);
               if (currentPlayer == CardManager.PLAYER_2)
               {
                    //mcMessageQueue.PushMessage("[[gwint_opponent_turn_start_message]]", "Opponents_turn");
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_opponents_turn");
               }
               if (currentPlayer == CardManager.PLAYER_1)
               {
                    //mcMessageQueue.PushMessage("[[gwint_player_turn_start_message]]", "your_turn");
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_your_turn");
               }
               //sawStartMessage = false;
               playerControllers[currentPlayer].playerRenderer.turnActive = true;
          }

          protected void state_update_PlayerTurn()
          {
               BasePlayerController currentPlayerController = playerControllers[currentPlayer];
               BasePlayerController opponentPlayerController = playerControllers[currentPlayer != CardManager.PLAYER_1 ? CardManager.PLAYER_1 : CardManager.PLAYER_2];
               if (currentPlayerController == null)
               {
                    throw new ArgumentException("GFX ---- currentPlayer not found for player: " + currentPlayerController.ToString());
               }
               /*if (mcMessageQueue.ShowingMessage()) 
               {
                       return;
               }*/
               if (!sawStartMessage)
               {
                    sawStartMessage = true;
                    currentPlayerController.startTurn();
               }
               if (currentPlayerController.turnOver)
               {
                    /*if (mcTutorials.visible && !sawScoreChangeTutorial) 
                    {
                        if (!(cardManager.currentPlayerControllerScores[CardManager.PLAYER_1] == 0) || !(cardManager.currentPlayerControllerScores[CardManager.PLAYER_2] == 0)) 
                        {
                            sawScoreChangeTutorial = true;
                            mcTutorials.continueTutorial();
                        }
                    }
                    if (mcTutorials.visible && !mcTutorials.isPaused) 
                    {
                             return;
                        }*/
                    if (currentPlayerController.currentRoundStatus != BasePlayerController.ROUND_PLAYER_STATUS_ACTIVE)
                    {
                         if (opponentPlayerController.currentRoundStatus != BasePlayerController.ROUND_PLAYER_STATUS_ACTIVE)
                         {
                              stateMachine.ChangeState("ShowingRoundResult");
                         }
                         else
                         {
                              stateMachine.ChangeState("ChangingPlayer");
                         }
                    }
                    else if (opponentPlayerController.currentRoundStatus != BasePlayerController.ROUND_PLAYER_STATUS_DONE)
                    {
                         stateMachine.ChangeState("ChangingPlayer");
                    }
                    else
                    {
                         currentPlayerController.startTurn();
                    }
               }
          }

          protected void state_leave_PlayerTurn()
          {
               List<CardInstance> creaturesList = null;
               CardTemplate cardTemplate = null;
               int counter = 0;
               playerControllers[currentPlayer].playerRenderer.turnActive = false;
               if (allNeutralInRound || !playedCreaturesInRound)
               {
                    creaturesList = cardManager.getAllCreatures(CardManager.PLAYER_1);
                    counter = 0;
                    while (counter < creaturesList.Count)
                    {
                         cardTemplate = creaturesList[counter].templateRef;
                         if (!cardTemplate.isType(CardTemplate.CardType_Spy))
                         {
                              playedCreaturesInRound = true;
                              if (cardTemplate.factionIdx != CardTemplate.FactionId_Neutral)
                              {
                                   allNeutralInRound = false;
                              }
                         }
                         ++counter;
                    }
                    creaturesList = cardManager.getAllCreatures(CardManager.PLAYER_2);
                    counter = 0;
                    while (counter < creaturesList.Count)
                    {
                         cardTemplate = creaturesList[counter].templateRef;
                         if (cardTemplate.isType(CardTemplate.CardType_Spy))
                         {
                              playedCreaturesInRound = true;
                              if (cardTemplate.factionIdx != CardTemplate.FactionId_Neutral)
                              {
                                   allNeutralInRound = false;
                              }
                         }
                         ++counter;
                    }
               }
          }

          protected void state_begin_ChangingPlayer()
          {
               if (playerControllers[currentPlayer].currentRoundStatus == BasePlayerController.ROUND_PLAYER_STATUS_DONE)
               {
                    Console.WriteLine("game flow controller: state begin changing player messages not implemented");
                    if (currentPlayer != CardManager.PLAYER_1)
                    {
                         //mcMessageQueue.PushMessage("[[gwint_opponent_passed_turn]]", "passed");
                    }
                    else
                    {
                         //mcMessageQueue.PushMessage("[[gwint_player_passed_turn]]", "passed");
                    }
               }
          }

          protected void state_update_ChangingPlayer()
          {
               //if (!mcMessageQueue.ShowingMessage()) 
               //{
               Console.WriteLine("game flow controller: state of the changing player not implemented");
               currentPlayer = currentPlayer != CardManager.PLAYER_1 ? CardManager.PLAYER_1 : CardManager.PLAYER_2;
               stateMachine.ChangeState("PlayerTurn");
               //}
          }

          protected void state_begin_ShowingRoundResult()
          {
               int counter = 0;
               int scoreP1 = cardManager.currentPlayerScores[CardManager.PLAYER_1];
               int scoreP2 = cardManager.currentPlayerScores[CardManager.PLAYER_2];
               int factionP1 = cardManager.playerDeckDefinitions[CardManager.PLAYER_1].getDeckFaction();
               int factionP2 = cardManager.playerDeckDefinitions[CardManager.PLAYER_2].getDeckFaction();
               int playerID = CardManager.PLAYER_INVALID;
               playerControllers[CardManager.PLAYER_1].resetCurrentRoundStatus();
               playerControllers[CardManager.PLAYER_2].resetCurrentRoundStatus();
               /*if (mcTutorials.visible && !sawRoundEndTutorial) 
               {
                   sawRoundEndTutorial = true;
                   mcTutorials.continueTutorial();
               }*/
               if (scoreP1 != scoreP2)
               {
                    if (scoreP1 > scoreP2)
                    {
                         //mcMessageQueue.PushMessage("[[gwint_player_won_round]]", "battle_won");
                         playerID = CardManager.PLAYER_1;
                         lastRoundWinner = CardManager.PLAYER_1;
                         //GwintGameMenu.mSingleton.playSound("gui_gwint_clash_victory");
                    }
                    else
                    {
                         //mcMessageQueue.PushMessage("[[gwint_opponent_won_round]]", "battle_lost");
                         playerID = CardManager.PLAYER_2;
                         lastRoundWinner = CardManager.PLAYER_2;
                         //GwintGameMenu.mSingleton.playSound("gui_gwint_clash_defeat");
                    }
               }
               else if (!(factionP1 == factionP2) && (factionP1 == CardTemplate.FactionId_Nilfgaard || factionP2 == CardTemplate.FactionId_Nilfgaard))
               {
                    //mcMessageQueue.PushMessage("[[gwint_nilfgaard_ability_triggered]]", "nilf_ability");
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_nilfgaardian_ability");
                    if (factionP1 != CardTemplate.FactionId_Nilfgaard)
                    {
                         //mcMessageQueue.PushMessage("[[gwint_opponent_won_round]]", "battle_lost");
                         playerID = CardManager.PLAYER_2;
                         lastRoundWinner = CardManager.PLAYER_2;
                         //GwintGameMenu.mSingleton.playSound("gui_gwint_clash_defeat");
                    }
                    else
                    {
                         //mcMessageQueue.PushMessage("[[gwint_player_won_round]]", "battle_won");
                         playerID = CardManager.PLAYER_1;
                         lastRoundWinner = CardManager.PLAYER_1;
                         //GwintGameMenu.mSingleton.playSound("gui_gwint_clash_victory");
                    }
               }
               else
               {
                    //mcMessageQueue.PushMessage("[[gwint_round_draw]]", "battle_draw");
                    playerID = CardManager.PLAYER_INVALID;
                    lastRoundWinner = CardManager.PLAYER_INVALID;
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_round_draw");
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_gem_destruction");
               }
               if (playerID != CardManager.PLAYER_INVALID)
               {
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_gem_destruction");
               }
               cardManager.roundResults[currentRound].setResults(scoreP1, scoreP2, playerID);
               cardManager.traceRoundResults();
               cardManager.updatePlayerLives();
               int heroCounter = 0;
               List<CardInstance> creaturesList = cardManager.getAllCreatures(CardManager.PLAYER_1);
               counter = 0;
               while (counter < creaturesList.Count)
               {
                    if (creaturesList[counter].templateRef.isType(CardTemplate.CardType_Hero))
                    {
                         ++heroCounter;
                    }
                    ++counter;
               }
               if (heroCounter >= 3)
               {
                    playedThreeHeroesOneRound = true;
               }
               if (allNeutralInRound && playedCreaturesInRound && playerID == CardManager.PLAYER_1)
               {
                    //GwintGameMenu.mSingleton.sendNeutralRoundVictoryAchievement();
               }
          }

          protected void state_update_ShowingRoundResult()
          {
               //if (!mcMessageQueue.ShowingMessage())
               //{
                    if (currentRound == 2 || currentRound == 1 && (cardManager.roundResults[0].winningPlayer == cardManager.roundResults[1].winningPlayer || cardManager.roundResults[0].winningPlayer == CardManager.PLAYER_INVALID || cardManager.roundResults[1].winningPlayer == CardManager.PLAYER_INVALID))
                    {
                         cardManager.clearBoard(false);
                         stateMachine.ChangeState("ShowingFinalResult");
                    }
                    else
                    {
                         if (lastRoundWinner != CardManager.PLAYER_INVALID)
                         {
                              currentPlayer = lastRoundWinner;
                         }
                         stateMachine.ChangeState("ClearingBoard");
                    }
               //}
          }

          protected void state_begin_ClearingBoard()
          {
               bool isMonster = false;
               if (cardManager.playerDeckDefinitions[CardManager.PLAYER_1].getDeckFaction() == CardTemplate.FactionId_No_Mans_Land && !(cardManager.chooseCreatureToExclude(CardManager.PLAYER_1) == null))
               {
                    isMonster = true;
               }
               else if (!isMonster && cardManager.playerDeckDefinitions[CardManager.PLAYER_2].getDeckFaction() == CardTemplate.FactionId_No_Mans_Land && !(cardManager.chooseCreatureToExclude(CardManager.PLAYER_2) == null))
               {
                    isMonster = true;
               }
               if (isMonster)
               {
                    Console.WriteLine("game flow controller:begin clearing board: monsters ability not implemented");
                    //mcMessageQueue.PushMessage("[[gwint_monster_faction_ability_triggered]]", "monster_ability");
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_monster_ability");
               }
               cardManager.clearBoard(true);
          }

          protected void state_update_ClearingBoard()
          {
               //if (!mcMessageQueue.ShowingMessage()) 
               //{
               cardManager.recalculateScores();
               ++currentRound;
               stateMachine.ChangeState("RoundStart");
               //}
          }

          protected void state_leave_ClearingBoard()
          {
               cardManager.recalculateScores();
          }

          protected void state_begin_ShowingFinalResult()
          {
               int factionP1 = 0;
               int factionP2 = 0;
               int playerID = CardManager.PLAYER_INVALID;
               int winnerR0 = cardManager.roundResults[0].winningPlayer;
               int winnerR1 = cardManager.roundResults[1].winningPlayer;
               if (currentRound == 1 && !(winnerR0 == winnerR1) && !cardManager.roundResults[2].played)
               {
                    factionP1 = cardManager.playerDeckDefinitions[CardManager.PLAYER_1].getDeckFaction();
                    factionP2 = cardManager.playerDeckDefinitions[CardManager.PLAYER_2].getDeckFaction();
                    if (factionP1 != factionP2)
                    {
                         if (factionP1 != CardTemplate.FactionId_Nilfgaard)
                         {
                              if (factionP2 == CardTemplate.FactionId_Nilfgaard)
                              {
                                   cardManager.roundResults[2].setResults(0, 0, CardManager.PLAYER_2);
                              }
                         }
                         else
                         {
                              cardManager.roundResults[2].setResults(0, 0, CardManager.PLAYER_1);
                         }
                    }
               }
               Console.WriteLine("game flow controller: state_begin_ShowingFinalResult. Tutorial needed");
               /*if (mcTutorials.visible && !sawEndGameTutorial) 
               {
                   sawEndGameTutorial = true;
                   mcTutorials.continueTutorial();
               }*/
               int winnerR2 = cardManager.roundResults[2].winningPlayer;
               playerControllers[CardManager.PLAYER_1].cardZoomEnabled = false;
               playerControllers[CardManager.PLAYER_2].cardZoomEnabled = false;
               playerControllers[CardManager.PLAYER_1].inputEnabled = false;
               playerControllers[CardManager.PLAYER_2].inputEnabled = false;
               //mcChoiceDialog.hideDialog();
               if (currentRound == 1 && (winnerR0 == winnerR1 || winnerR0 == CardManager.PLAYER_INVALID || winnerR1 == CardManager.PLAYER_INVALID))
               {
                    if (winnerR0 != CardManager.PLAYER_INVALID)
                    {
                         playerID = winnerR0;
                    }
                    else
                    {
                         playerID = winnerR1;
                    }
               }
               else if (currentRound != 2)
               {
                    throw new ArgumentException("GFX - Danger will robinson, danger!");
               }
               else if (winnerR0 == winnerR1 || winnerR0 == winnerR2)
               {
                    playerID = winnerR0;
               }
               else if (winnerR1 == winnerR2)
               {
                    playerID = winnerR1;
               }
               cardManager.traceRoundResults();
               Console.WriteLine("GFX -#AI#--- game winner was: " + playerID);
               Console.WriteLine("GFX -#AI#--- current round was: " + currentRound);
               Console.WriteLine("GFX -#AI#--- Round 1 winner: " + winnerR0);
               Console.WriteLine("GFX -#AI#--- Round 2 winner: " + winnerR0);
               Console.WriteLine("GFX -#AI#--- Round 3 winner: " + winnerR0);
               if (playerID != CardManager.PLAYER_1)
               {
                    if (playerID != CardManager.PLAYER_2)
                    {
                         //GwintGameMenu.mSingleton.playSound("gui_gwint_battle_draw");
                    }
                    else
                    {
                         //GwintGameMenu.mSingleton.playSound("gui_gwint_battle_lost");
                    }
               }
               else
               {
                    if (playedThreeHeroesOneRound)
                    {
                         //GwintGameMenu.mSingleton.sendHeroRoundVictoryAchievement();
                    }
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_battle_won");
               }
               //mcEndGameDialog.show(playerID, OnEndGameResult);
          }

          protected void state_update_ShowingFinalResult()
          {
          }

          protected void state_begin_reset()
          {
               currentRound = 0;
               cardManager.reset();
               //mcMessageQueue.PushMessage("[[gwint_resetting]]");
               Console.WriteLine("game flow controller: state begin reset message not implemented");
               stateMachine.ChangeState("SpawnLeaders");
          }

          protected bool shouldDisallowStateChangeFunc()
          {
               /*if (mcTutorials && mcTutorials.visible && !mcTutorials.isPaused) 
               {
                   return true;
               }
               return mcMessageQueue.ShowingMessage() || CardFXManager.getInstance().isPlayingAnyCardFX() || mcChoiceDialog.isShown() || CardTweenManager.getInstance().isAnyCardMoving();*/
               Console.WriteLine("game flow controller: should disallow state change function not implemented yet!");
               return false;
          }

          public static GwintGameFlowController getInstance()
          {
               return _instance;
          }

          public bool tutorial
          {
               get { return _tutorial; }
               set { _tutorial = value; }
          }

          protected void handleAcceptMulligan(int cardID)
          {
               CardInstance mulliganCard = null;
               CardInstance card = cardManager.getCardInstance(cardID);
               if (card != null)
               {
                    Console.WriteLine("game flow controller: handle except Mulligan visual replacement not implemented");
                    mulliganCard = cardManager.mulliganCard(card);
                    //mcChoiceDialog.replaceCard(card, mulliganCard);
                    ++_mulliganCardsCount;
                    //GwintGameMenu.mSingleton.playSound("gui_gwint_card_redrawn");
                    if (_mulliganCardsCount < 2)
                    {
                         //mcChoiceDialog.updateDialogText("[[gwint_can_choose_card_to_redraw]]");
                         //mcChoiceDialog.appendDialogText(" 1/2");
                    }
                    _mulliganDecided = _mulliganCardsCount >= 2;
               }
          }

          protected void handleDeclineMulligan()
          {
               //mcChoiceDialog.hideDialog();
               _mulliganDecided = true;
               Console.WriteLine("game flow controller: handle decline Mulligan and tutorial not implemented");
               /*if (playerControllers[CardManager.PLAYER_1] is HumanPlayerController)
               {
                    (playerControllers[CardManager.PLAYER_1] as HumanPlayerController).attachToTutorialCarouselMessage();
               }*/
          }

          protected void handleForceCardSelected(int cardID)
          {
               /*if (mcChoiceDialog && mcChoiceDialog.visible)
               {
                    mcChoiceDialog.cardsCarousel.selectedIndex = cardID;
               }*/
               Console.WriteLine("game flow controller: handle force card selected not implemented");
          }
     }
}
