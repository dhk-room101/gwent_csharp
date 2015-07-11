/* movieclips/sounds/text */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class BasePlayerController
     {
          public const int ROUND_PLAYER_STATUS_DONE = 2;
          public const int ROUND_PLAYER_STATUS_ACTIVE = 1;
          protected CardTransaction _decidedCardTransaction;
          protected FiniteStateMachine _stateMachine;
          protected bool isAI = false;
          public int opponentID;
          public int playerID;
          public GwintGameFlowController gameFlowControllerRef;
          protected CardSlot _transactionCard;
          private int _currentRoundStatus = 1;
          protected bool _cardZoomEnabled = true;
          protected GwintPlayerRenderer _playerRenderer;
          protected GwintBoardRenderer _boardRenderer;
          public bool inputEnabled = true;
          protected bool _turnOver;

          public BasePlayerController()
          {
               _stateMachine = new FiniteStateMachine();
          }

          public int currentRoundStatus
          {
               get
               {
                    if (CardManager.getInstance().getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, playerID).Count == 0 && 
                         !CardManager.getInstance().getCardLeader(playerID).canBeUsed)
                    {
                         return ROUND_PLAYER_STATUS_DONE;
                    }
                    return _currentRoundStatus;
               }

               set
               {
                    _currentRoundStatus = value;
                    if (_playerRenderer != null)
                    {
                         _playerRenderer.showPassed(_currentRoundStatus == ROUND_PLAYER_STATUS_DONE);
                    }
               }
          }

          public GwintPlayerRenderer playerRenderer
          {
               get
               {
                    return _playerRenderer;
               }

               set
               {
                    _playerRenderer = value;
                    GwintDeck deck = CardManager.getInstance().playerDeckDefinitions[_playerRenderer.playerID];
                    Console.WriteLine("bass player controller: deck namenot implemented yet!");
                    //_playerRenderer.txtFactionName.text = deck.getFactionNameString();
                    //_playerRenderer.mcFactionIcon.gotoAndStop(deck.getDeckKingTemplate().getFactionString());
                    _playerRenderer.numCardsInHand = 0;
               }
          }

          public virtual GwintBoardRenderer boardRenderer
          {
               get
               {
                    return _boardRenderer;
               }

               set
               {
                    _boardRenderer = value;
               }
          }

          public CardSlot transactionCard
          {
               set
               {
                    if (_transactionCard != null)
                    {
                         _transactionCard.cardState = CardSlot.STATE_BOARD;
                    }
                    _transactionCard = value;
                    if (_boardRenderer != null)
                    {
                         _boardRenderer.updateTransactionCardTooltip(value);
                    }
                    if (_transactionCard != null)
                    {
                         _transactionCard.cardState = CardSlot.STATE_CAROUSEL;
                    }
               }
          }

          //includes tweening?
          protected void startCardTransaction(int slotID)
          {
               CardSlot cardSlot = null;
               //int _x = 0;
               //int _y = 0;
               if (boardRenderer != null)
               {
                    cardSlot = boardRenderer.getCardSlotById(slotID);
                    //_x = boardRenderer.mcTransitionAnchor.x;
                    //_y = boardRenderer.mcTransitionAnchor.y;
                    //CardTweenManager.getInstance().storePosition(cardSlot);
                    //CardTweenManager.getInstance().tweenTo(cardSlot, _x, _y);
                    transactionCard = cardSlot;
               }
          }

          protected virtual void state_begin_ApplyingCard()
          {
               CardLeaderInstance leader = (CardLeaderInstance)_decidedCardTransaction.sourceCardInstanceRef;
               if (leader != null)
               {
                    leader.ApplyLeaderAbility(isAI);
                    //CardTweenManager.getInstance().restorePosition(_transactionCard, true);
                    transactionCard = null;
               }
               else if (_decidedCardTransaction.targetSlotID == CardManager.CARD_LIST_LOC_INVALID)
               {
                    if (_decidedCardTransaction.targetCardInstanceRef != null)
                    {
                         applyTransactionCardToCardInstance(_decidedCardTransaction.targetCardInstanceRef);
                    }
                    else if (_decidedCardTransaction.sourceCardInstanceRef.templateRef.isType(CardTemplate.CardType_Global_Effect))
                    {
                         applyGlobalEffectTransactionCard();
                    }
                    else
                    {
                         declineCardTransaction();
                    }
               }
               else
               {
                    transferTransactionCardToDestination(_decidedCardTransaction.targetSlotID, _decidedCardTransaction.targetPlayerID);
               }
          }

          protected void state_update_ApplyingCard()
          {
               if (!cardEffectApplying())
               {
                    if (gameFlowControllerRef.playerControllers[opponentID].currentRoundStatus != ROUND_PLAYER_STATUS_DONE)
                    {
                         _stateMachine.ChangeState("Idle");
                    }
                    else
                    {
                         _stateMachine.ChangeState("ChoosingMove");
                    }
               }
          }

          protected virtual bool cardEffectApplying()
          {
               return CardTweenManager.getInstance().isAnyCardMoving() ||
                  //gameFlowControllerRef.mcMessageQueue.ShowingMessage() ||//TO DO
                  CardFXManager.getInstance().isPlayingAnyCardFX();
          }

          protected void applyTransactionCardToCardInstance(CardInstance card)
          {
               CardManager.getInstance().replaceCardInstanceIDs(_transactionCard.instanceId, card.instanceId);
               transactionCard = null;
          }

          protected void applyGlobalEffectTransactionCard()
          {
               if (_transactionCard != null)
               {
                    CardManager.getInstance().applyCardEffectsID(_transactionCard.instanceId);
                    CardManager.getInstance().sendToGraveyardID(_transactionCard.instanceId);
                    transactionCard = null;
               }
          }

          protected void declineCardTransaction()
          {
               if (_transactionCard != null)
               {
                    Console.WriteLine("declined transaction restore position not implemented yet!");
                    //CardTweenManager.getInstance().restorePosition(_transactionCard, true);
                    transactionCard = null;
               }
          }

          protected void transferTransactionCardToDestination(int slotID, int playerID)
          {
               if (_transactionCard != null)
               {
                    CardManager.getInstance().addCardInstanceIDToList(_transactionCard.instanceId, slotID, playerID);
                    transactionCard = null;
               }
          }

          public virtual void startTurn()
          {
               if (currentRoundStatus == ROUND_PLAYER_STATUS_DONE)
               {
                    return;
               }
               _turnOver = false;
          }

          public virtual void skipTurn()
          {

          }

          public virtual bool cardZoomEnabled
          {
               get { return _cardZoomEnabled; }
               set { _cardZoomEnabled = value; }
          }

          public bool turnOver
          {
               get
               {
                    Console.WriteLine("bass player controller: turn over get");
                    return _turnOver && (_transactionCard == null);
               }
          }

          public void resetCurrentRoundStatus()
          {
               if (CardManager.getInstance().getCardInstanceList(CardManager.CARD_LIST_LOC_HAND, playerID).Count > 0)
               {
                    currentRoundStatus = ROUND_PLAYER_STATUS_ACTIVE;
               }
          }
     }
}
