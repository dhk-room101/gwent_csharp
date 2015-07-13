using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class HumanPlayerController: BasePlayerController
     {
          protected GwintCardHolder _handHolder;
          protected GwintCardHolder _currentZoomedHolder = null;
          protected bool _cardConfirmation;
          //protected var mcChoiceDialog:W3ChoiceDialog;
          //protected var _skipButton:InputFeedbackButton;

          //Needs mouse/input handling?

          public HumanPlayerController()
          {
               _stateMachine.AddState("Idle", state_begin_Idle, on_state_about_to_update, state_end_Idle);
               _stateMachine.AddState("ChoosingCard", state_begin_ChoosingCard, state_update_ChoosingCard, state_end_ChoosingCard);
               _stateMachine.AddState("ChoosingHandler", state_begin_ChoosingHandler, state_update_ChoosingHandler, null);
               _stateMachine.AddState("ChoosingTargetCard", state_begin_ChoosingTargetCard, state_update_ChoosingTargetCard, null);
               _stateMachine.AddState("WaitConfirmation", state_begin_WaitConfirmation, state_update_WaitConfirmation, null);
               _stateMachine.AddState("ApplyingCard", state_begin_ApplyingCard, state_update_ApplyingCard, null);
          }

          protected void on_state_about_to_update()
          {
               //Console.WriteLine("human player controller: state about to update not implemented yet");
               /*if (_currentZoomedHolder && !mcChoiceDialog.visible) 
               {
                   closeZoomCB();
               }*/
          }

          protected void state_begin_Idle()
          {
               _decidedCardTransaction = null;
               if (_boardRenderer != null)
               {
                    _boardRenderer.activateAllHolders(true);
                    if (_handHolder != null && _boardRenderer.getSelectedCardHolder() != _handHolder)
                    {
                         _boardRenderer.selectCardHolderAdv(_handHolder);
                    }
                    _boardRenderer.getCardHolder(CardManager.CARD_LIST_LOC_LEADER, CardManager.PLAYER_1).updateLeaderStatus(false);
               }
               declineCardTransaction();
               //movie clip?
               /*if (!GwintGameMenu.mSingleton.mcTutorials.visible && _currentZoomedHolder == null) 
               {
                   resetToDefaultButtons();
                   red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.navigate, scaleform.clik.constants.NavigationCode.GAMEPAD_L3, -1, "panel_button_common_navigation");
                   if (_boardRenderer && !(_boardRenderer.getSelectedCard() == null) && cardZoomEnabled) 
                   {
                       red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.zoomCard, scaleform.clik.constants.NavigationCode.GAMEPAD_R2, red.core.constants.KeyCode.RIGHT_MOUSE, "panel_button_common_zoom");
                   }
               }*/
          }

          protected void state_end_Idle()
          {
               if (_boardRenderer != null)
               {
                    if (_boardRenderer.getSelectedCardHolder() != _handHolder)
                    {
                         _boardRenderer.selectCardHolderAdv(_handHolder);
                    }
                    _boardRenderer.getCardHolder(CardManager.CARD_LIST_LOC_LEADER, CardManager.PLAYER_1).updateLeaderStatus(true);
               }
          }

          protected void state_begin_ChoosingCard()
          {
               Console.WriteLine("human player controller: state_begin_ChoosingCard Needs skip button");
               CardLeaderInstance cardLeader = null;
               /*if (_skipButton) 
               {
                   _skipButton.visible = true;
               }*/
               if (_currentZoomedHolder == null)
               {
                    //resetToDefaultButtons();
                    //red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.navigate, scaleform.clik.constants.NavigationCode.GAMEPAD_L3, -1, "panel_button_common_navigation");
                    cardLeader = CardManager.getInstance().getCardLeader(playerID);
                    if (cardLeader != null && cardLeader.canBeUsed)
                    {
                         //red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.leaderCard, scaleform.clik.constants.NavigationCode.GAMEPAD_X, red.core.constants.KeyCode.X, "gwint_use_leader");
                    }
                    if (_handHolder.cardSlotsList.Count > 0)
                    {
                         //red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.apply, scaleform.clik.constants.NavigationCode.GAMEPAD_A, red.core.constants.KeyCode.ENTER, "panel_button_common_select");
                    }
                    if (_boardRenderer.getSelectedCard() != null && cardZoomEnabled)
                    {
                         //red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.zoomCard, scaleform.clik.constants.NavigationCode.GAMEPAD_R2, red.core.constants.KeyCode.RIGHT_MOUSE, "panel_button_common_zoom");
                    }
               }
          }

          protected void state_update_ChoosingCard()
          {
               CardInstance cardInstance = null;
               bool isCardLeader = false;
               bool hasEffect = false;
               bool isType = false;
               on_state_about_to_update();
               if (_transactionCard != null)
               {
                    cardInstance = CardManager.getInstance().getCardInstance(_transactionCard.instanceId);
                    isCardLeader = cardInstance is CardLeaderInstance;
                    hasEffect = cardInstance.templateRef.hasEffect(CardTemplate.CardEffect_UnsummonDummy);
                    isType = cardInstance.templateRef.isType(CardTemplate.CardType_Global_Effect);
                    if (isCardLeader || isType)
                    {
                         _stateMachine.ChangeState("WaitConfirmation");
                    }
                    else if (hasEffect)
                    {
                         _stateMachine.ChangeState("ChoosingTargetCard");
                    }
                    else
                    {
                         _stateMachine.ChangeState("ChoosingHandler");
                    }
               }
          }

          protected void state_end_ChoosingCard()
          {
               Console.WriteLine("human player controller: state_end_ChoosingCard Needs skip button");
               /*if (_skipButton) 
               {
                   _skipButton.visible = false;
               }*/
          }

          protected void state_begin_ChoosingTargetCard()
          {
               CardInstance cardInstance = null;
               GwintCardHolder selectedCardHolder = null;
               CardInstance cardSource = null;
               CardInstance cardTarget = null;
               if (_transactionCard != null && _boardRenderer != null)
               {
                    cardInstance = CardManager.getInstance().getCardInstance(_transactionCard.instanceId);
                    _boardRenderer.activateHoldersForCard(cardInstance, true);
               }
               if (_currentZoomedHolder == null)
               {
                    //resetToDefaultButtons();
                    //red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.navigate, scaleform.clik.constants.NavigationCode.GAMEPAD_L3, -1, "panel_button_common_navigation");
                    //red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.cancel, scaleform.clik.constants.NavigationCode.GAMEPAD_B, red.core.constants.KeyCode.ESCAPE, "panel_common_cancel");
                    selectedCardHolder = _boardRenderer.getSelectedCardHolder();
                    if (selectedCardHolder.cardSelectionEnabled && selectedCardHolder.cardSlotsList.Count > 0 && !(selectedCardHolder.getSelectedCardSlot() == null))
                    {
                         cardSource = CardManager.getInstance().getCardInstance(_transactionCard.instanceId);
                         cardTarget = CardManager.getInstance().getCardInstance(selectedCardHolder.getSelectedCardSlot().instanceId);
                         if (cardSource.canBeCastOn(cardTarget))
                         {
                              Console.WriteLine("human player controller source can be cast on target");
                              //red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.apply, scaleform.clik.constants.NavigationCode.GAMEPAD_A, red.core.constants.KeyCode.ENTER, "panel_common_apply");
                         }
                    }
               }
          }

          protected void state_update_ChoosingTargetCard()
          {
               on_state_about_to_update();
          }

          protected void state_begin_ChoosingHandler()
          {
               CardInstance transactionCardInstance = null;
               if (_transactionCard != null && _boardRenderer != null)
               {
                    transactionCardInstance = CardManager.getInstance().getCardInstance(_transactionCard.instanceId);
                    boardRenderer.activateHoldersForCard(transactionCardInstance, true);
               }
               if (_currentZoomedHolder == null)
               {
                    /*
                     resetToDefaultButtons();
                     red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.apply, scaleform.clik.constants.NavigationCode.GAMEPAD_A, red.core.constants.KeyCode.ENTER, "panel_common_apply");
                     red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.cancel, scaleform.clik.constants.NavigationCode.GAMEPAD_B, red.core.constants.KeyCode.ESCAPE, "panel_common_cancel");
                     red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.navigate, scaleform.clik.constants.NavigationCode.GAMEPAD_L3, -1, "panel_button_common_navigation");
                         */
               }
          }

          protected void state_update_ChoosingHandler()
          {
               on_state_about_to_update();
          }

          protected void state_begin_WaitConfirmation()
          {
               _cardConfirmation = false;
               if (_currentZoomedHolder == null)
               {
                    /*resetToDefaultButtons();
                    red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.cancel, scaleform.clik.constants.NavigationCode.GAMEPAD_B, red.core.constants.KeyCode.ESCAPE, "panel_common_cancel");
                    red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.apply, scaleform.clik.constants.NavigationCode.GAMEPAD_A, red.core.constants.KeyCode.ENTER, "panel_common_apply");*/
               }
          }

          protected void state_update_WaitConfirmation()
          {
               on_state_about_to_update();
               if (_cardConfirmation && _transactionCard != null)
               {
                    _cardConfirmation = false;
                    _decidedCardTransaction = new CardTransaction();
                    _decidedCardTransaction.targetPlayerID = playerID;
                    _decidedCardTransaction.sourceCardInstanceRef = CardManager.getInstance().getCardInstance(_transactionCard.instanceId);
                    _stateMachine.ChangeState("ApplyingCard");
               }
          }

          //needs events?
          public override GwintBoardRenderer boardRenderer
          {
               set
               {
                    if (_boardRenderer != value && value != null)
                    {
                         /*value.addEventListener(red.game.witcher3.events.GwintCardEvent.CARD_CHOSEN, handleCardChosen, false, 0, true);
                         value.addEventListener(red.game.witcher3.events.GwintCardEvent.CARD_SELECTED, handleCardSelected, false, 0, true);
                         value.addEventListener(red.game.witcher3.events.GwintHolderEvent.HOLDER_CHOSEN, handleHolderChosen, false, 0, true);
                         value.addEventListener(red.game.witcher3.events.GwintHolderEvent.HOLDER_SELECTED, handleHolderSelected, false, 0, true);*/
                         GwintEventListener listener = new GwintEventListener(value);
                         _handHolder = value.getCardHolder(CardManager.CARD_LIST_LOC_HAND, playerID);
                    }
                    base.boardRenderer = value;
               }
          }

          protected void handleCardChosen()
          {
               GwintCardHolder cardHolder = _handHolder;
               CardSlot cardSlot = _boardRenderer.getCardSlotById(cardHolder.selectedCardIdx);
               CardInstance source = null;
               CardInstance target = null;
               CardLeaderInstance leader = null;
               if (this._currentZoomedHolder != null)
               {
                    return;
               }
               Console.WriteLine("GFX handleCardChosen <", _stateMachine.currentState, "> ", cardSlot.cardIndex);
               if (cardSlot != null)
               {
                    switch (_stateMachine.currentState)
                    {
                         case "ChoosingCard":
                              {
                                   if (cardHolder.cardHolderID == CardManager.CARD_LIST_LOC_HAND || cardHolder.cardHolderID == CardManager.CARD_LIST_LOC_LEADER && cardHolder.playerID == CardManager.PLAYER_1)
                                   {
                                        leader = (CardLeaderInstance)cardSlot.cardInstance;
                                        if (leader == null || leader.canBeUsed)
                                        {
                                             startCardTransaction(cardSlot.instanceId);
                                        }
                                   }
                                   break;
                              }
                         case "ChoosingTargetCard":
                              {
                                   source = CardManager.getInstance().getCardInstance(_transactionCard.instanceId);
                                   target = CardManager.getInstance().getCardInstance(cardSlot.instanceId);
                                   if (source.canBeCastOn(target))
                                   {
                                        _decidedCardTransaction = new CardTransaction();
                                        _decidedCardTransaction.targetPlayerID = playerID;
                                        _decidedCardTransaction.targetSlotID = CardManager.CARD_LIST_LOC_INVALID;
                                        _decidedCardTransaction.targetCardInstanceRef = target;
                                        _decidedCardTransaction.sourceCardInstanceRef = source;
                                        _stateMachine.ChangeState("ApplyingCard");
                                   }
                                   break;
                              }
                         default:
                              {
                                   break;
                              }
                    }
               }
          }

          public override void startTurn()
          {
               if (CardManager.getInstance().getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, playerID).Count == 0 && !CardManager.getInstance().getCardLeader(playerID).canBeUsed)
               {
                    currentRoundStatus = BasePlayerController.ROUND_PLAYER_STATUS_DONE;
               }
               else
               {
                    base.startTurn();
                    _stateMachine.ChangeState("ChoosingCard");
               }
          }

          public override void skipTurn()
          {
               if (_stateMachine.currentState == "ChoosingCard" && _currentZoomedHolder == null)
               {
                    currentRoundStatus = BasePlayerController.ROUND_PLAYER_STATUS_DONE;
                    _turnOver = true;
                    if (_transactionCard != null)
                    {
                         _boardRenderer.activateAllHolders(true);
                         _boardRenderer.selectCard(_transactionCard);
                         declineCardTransaction();
                    }
                    _stateMachine.ChangeState("Idle");
               }
          }

          public override bool cardZoomEnabled
          {
               set
               {
                    base.cardZoomEnabled = value;
                    if (!value && _currentZoomedHolder != null)
                    {
                         closeZoomCB();
                    }
               }
          }

          //TO DO
          protected void closeZoomCB(int useless = 0)//parameter not needed?
          {
               /*var loc1=null;
               var loc2=null;
               if (_currentZoomedHolder) 
               {
                   GwintGameMenu.mSingleton.playSound("gui_gwint_preview_card");
                   if (_skipButton) 
                   {
                       _skipButton.visible = false;
                   }
                   if (mcChoiceDialog.visible) 
                   {
                       mcChoiceDialog.hideDialog();
                   }
                   resetToDefaultButtons();
                   loc1 = CardManager.getInstance().getCardLeader(playerID);
                   loc2 = _boardRenderer.getSelectedCardHolder();
                   if (_stateMachine.currentState == "ChoosingCard" && (_currentZoomedHolder.cardHolderID == CardManager.CARD_LIST_LOC_HAND || _currentZoomedHolder.cardHolderID == CardManager.CARD_LIST_LOC_LEADER && _currentZoomedHolder.playerID == CardManager.PLAYER_1 && loc1 && loc1.canBeUsed)) 
                   {
                       red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.apply, scaleform.clik.constants.NavigationCode.GAMEPAD_A, red.core.constants.KeyCode.ENTER, "panel_button_common_select");
                   }
                   red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.zoomCard, scaleform.clik.constants.NavigationCode.GAMEPAD_R2, red.core.constants.KeyCode.RIGHT_MOUSE, "panel_button_common_zoom");
                   if (loc1 && loc1.canBeUsed) 
                   {
                       red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.leaderCard, scaleform.clik.constants.NavigationCode.GAMEPAD_X, red.core.constants.KeyCode.X, "gwint_use_leader");
                   }
                   mcChoiceDialog.cardsCarousel.removeEventListener(scaleform.clik.events.ListEvent.INDEX_CHANGE, onCarouselSelectionChanged, false);
                   red.game.witcher3.managers.InputFeedbackManager.appendButtonById(red.game.witcher3.constants.GwintInputFeedback.navigate, scaleform.clik.constants.NavigationCode.GAMEPAD_L3, -1, "panel_button_common_navigation");
                   _currentZoomedHolder = null;
               }*/
          }

          //choice dialogue visible?
          protected override bool cardEffectApplying()
          {
               return base.cardEffectApplying();// || mcChoiceDialog.visible;
          }

          protected override void state_begin_ApplyingCard()
          {
               base.state_begin_ApplyingCard();
               _boardRenderer.activateAllHolders(true);
               if (_handHolder != null && !(_boardRenderer.getSelectedCardHolder() == _handHolder))
               {
                    _boardRenderer.selectCardHolderAdv(_handHolder);
               }
          }
     }
}
