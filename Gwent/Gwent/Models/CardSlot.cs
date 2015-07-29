using Gwent.Core;
using Gwent.ViewModels;
using MVVMSidekick.Reactive;
using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Gwent.Models
{
     public class CardSlot : ViewModelBase<CardSlot>
     {
          public Image cardImage { get; set; }
          public int owningPlayer { get; set; }
          public int owningHolder { get; set; }
          public CardTemplate template { get; set; }
          public int power { get; set; }
          public Guid instance { get; set; }

          //local
          protected int _counter = 0;
          protected bool _clickedOnce = false;
          protected bool _clickedTwice = false;

          public CardSlot()
          {
               owningPlayer = ValuesRepository.PLAYER_INVALID;
               owningHolder = ValuesRepository.CARD_LIST_LOC_INVALID;
               template = new CardTemplate { index = ValuesRepository.TEMPLATE_INVALID };
               power = 0;
               instance = Guid.NewGuid();
               IsTransactionReady = false;
          }

          protected int counter
          {
               get { return _counter; }
               set { _counter = value; }
          }

          protected bool clickedOnce
          {
               get { return _clickedOnce; }
               set { _clickedOnce = value; }
          }

          protected bool clickedTwice
          {
               get { return _clickedTwice; }
               set { _clickedTwice = value; }
          }

          //Handle commands
          public CommandModel<ReactiveCommand, String> CommandEvaluateClick
          {
               get { return _CommandEvaluateClickLocator(this).Value; }
               set { _CommandEvaluateClickLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property CommandModel<ReactiveCommand, String> CommandEvaluateClick Setup
          protected Property<CommandModel<ReactiveCommand, String>> _CommandEvaluateClick = new Property<CommandModel<ReactiveCommand, String>> { LocatorFunc = _CommandEvaluateClickLocator };
          static Func<BindableBase, ValueContainer<CommandModel<ReactiveCommand, String>>> _CommandEvaluateClickLocator = RegisterContainerLocator<CommandModel<ReactiveCommand, String>>("CommandEvaluateClick", model => model.Initialize("CommandEvaluateClick", ref model._CommandEvaluateClick, ref _CommandEvaluateClickLocator, _CommandEvaluateClickDefaultValueFactory));
          static Func<BindableBase, CommandModel<ReactiveCommand, String>> _CommandEvaluateClickDefaultValueFactory =
              model =>
              {
                   var cmd = new ReactiveCommand(canExecute: true) { ViewModel = model };
                   var vm = CastToCurrentType(model);
                   cmd.Subscribe(_ =>
                   {
                        vm.IncrementingCounter();
                        vm.EvaluatingClick(vm);
                   }).DisposeWith(model);
                   return cmd.CreateCommandModel("EvaluateClick");
              };
          #endregion

          //commands functions
          private void IncrementingCounter()
          {
               ++counter;
          }

          private async void EvaluatingClick( CardSlot card)
          {
               if (!clickedOnce)
               {
                    clickedOnce = true;
                    await ValuesRepository.PutTaskDelay(100);
               }
               if ( counter == 1)
               {
                    //Console.WriteLine("Click ONCE");
                    HandleMouseClick(card);
               }
               else
               {
                    if (!clickedTwice)
                    {
                         clickedTwice = true;
                         //Console.WriteLine("Click TWICE");
                         HandleMouseDoubleClick(card);
                    }
               }
          }

          private void HandleMouseClick(CardSlot cardSlot)
          {
               MainWindow_ViewModel.mSingleton.SelectedCardSlot = cardSlot;
               //Console.WriteLine("!!!LEFT CLICK!!!");
               ResetStates();
          }

          private async void HandleMouseDoubleClick(CardSlot cardSlot)
          {
               await ValuesRepository.PutTaskDelay(100);
               //Console.WriteLine("double-click");
               ResetStates();
               MainWindow_ViewModel.mSingleton.startCardTransaction(cardSlot);
          }

          private void ResetStates()
          {
               //Console.WriteLine("resetting states");
               clickedOnce = false;
               clickedTwice = false;
               counter = 0;
          }

          public bool IsTransactionReady
          {
               get { return _IsTransactionReadyLocator(this).Value; }
               set { _IsTransactionReadyLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property bool IsTransactionReady Setup
          protected Property<bool> _IsTransactionReady = new Property<bool> { LocatorFunc = _IsTransactionReadyLocator };
          static Func<BindableBase, ValueContainer<bool>> _IsTransactionReadyLocator = RegisterContainerLocator<bool>("IsTransactionReady", model => model.Initialize("IsTransactionReady", ref model._IsTransactionReady, ref _IsTransactionReadyLocator, _IsTransactionReadyDefaultValueFactory));
          static Func<bool> _IsTransactionReadyDefaultValueFactory = null;
          #endregion

          public CardTransaction OptimalTransaction
          {
               get { return _OptimalTransactionLocator(this).Value; }
               set { _OptimalTransactionLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property CardTransaction OptimalTransaction Setup
          protected Property<CardTransaction> _OptimalTransaction = new Property<CardTransaction> { LocatorFunc = _OptimalTransactionLocator };
          static Func<BindableBase, ValueContainer<CardTransaction>> _OptimalTransactionLocator = RegisterContainerLocator<CardTransaction>("OptimalTransaction", model => model.Initialize("OptimalTransaction", ref model._OptimalTransaction, ref _OptimalTransactionLocator, _OptimalTransactionDefaultValueFactory));
          static Func<CardTransaction> _OptimalTransactionDefaultValueFactory = null;
          #endregion
     }
}
