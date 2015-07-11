﻿using System.Reactive;
using System.Reactive.Linq;
using MVVMSidekick.ViewModels;
using MVVMSidekick.Views;
using MVVMSidekick.Reactive;
using MVVMSidekick.Services;
using MVVMSidekick.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Shapes;
using Gwent.Models;

namespace Gwent.ViewModels
{
     public class MainWindow_ViewModel : ViewModelBase<MainWindow_ViewModel>
     {
          GwintGameFlowController gameFlowController;
          public MainWindow_ViewModel()
          {
               PlayerSiegeCards = new ObservableCollection<Card>();
               gameFlowController = new GwintGameFlowController();
               //Previous();
          }

          public void Previous()
          {
               PlayerSiegeCards = new ObservableCollection<Card>();

               for (var i = 0; i < 6; i++)
               {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Fill = Brushes.DarkBlue;//Fill
                    rectangle.Stroke = Brushes.DarkMagenta;//Frame

                    /*//Overlapping
                    Thickness margin = rectangle.Margin;
                    margin.Left = -15;
                    rectangle.Margin = margin;*/

                    Card card = new Card();
                    card.Face = rectangle;

                    PlayerSiegeCards.Add(card);
               }

               /*Uri uri;
               Image imageRange = new Image();
               uri = new Uri("pack://application:,,,/Images/range.jpg");
               imageRange.Source = new BitmapImage(uri);

               PlayerSiegeCards.Add(imageRange);*/

               Title = "Gwent";

               ResizeCards();
          }

          public void ResizeCards()
          {
               foreach ( Card card in PlayerSiegeCards)
               {
                    card.Face.Height = ObservedHeight * 0.1;
                    card.Face.Width = ObservedWidth * 0.05;
               }
          }

          public ObservableCollection<Card> PlayerSiegeCards
          {
               get { return _PlayerSiegeCardsLocator(this).Value; }
               set { _PlayerSiegeCardsLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<Card> PlayerSiegeCards Setup
          protected Property<ObservableCollection<Card>> _PlayerSiegeCards = new Property<ObservableCollection<Card>> { LocatorFunc = _PlayerSiegeCardsLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<Card>>> _PlayerSiegeCardsLocator = RegisterContainerLocator<ObservableCollection<Card>>("PlayerSiegeCards", model => model.Initialize("PlayerSiegeCards", ref model._PlayerSiegeCards, ref _PlayerSiegeCardsLocator, _PlayerSiegeCardsDefaultValueFactory));
          static Func<ObservableCollection<Card>> _PlayerSiegeCardsDefaultValueFactory = null;
          #endregion

          public double ObservedHeight
          {
               get { return _ObservedHeightLocator(this).Value; }
               set 
               {
                    _ObservedHeightLocator(this).SetValueAndTryNotify(value);
                    ResizeCards();
               }
          }

          #region Property double ObservedHeight Setup
          protected Property<double> _ObservedHeight = new Property<double> { LocatorFunc = _ObservedHeightLocator };
          static Func<BindableBase, ValueContainer<double>> _ObservedHeightLocator = RegisterContainerLocator<double>("ObservedHeight", model => model.Initialize("ObservedHeight", ref model._ObservedHeight, ref _ObservedHeightLocator, _ObservedHeightDefaultValueFactory));
          static Func<double> _ObservedHeightDefaultValueFactory = null;
          #endregion

          public double ObservedWidth
          {
               get { return _ObservedWidthLocator(this).Value; }
               set
               {
                    _ObservedWidthLocator(this).SetValueAndTryNotify(value);
                    ResizeCards();
               }
          }

          #region Property double ObservedWidth Setup
          protected Property<double> _ObservedWidth = new Property<double> { LocatorFunc = _ObservedWidthLocator };
          static Func<BindableBase, ValueContainer<double>> _ObservedWidthLocator = RegisterContainerLocator<double>("ObservedWidth", model => model.Initialize("ObservedWidth", ref model._ObservedWidth, ref _ObservedWidthLocator, _ObservedWidthDefaultValueFactory));
          static Func<double> _ObservedWidthDefaultValueFactory = null;
          #endregion

          public String Title
          {
               get { return _TitleLocator(this).Value; }
               set { _TitleLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property String Title Setup
          protected Property<String> _Title = new Property<String> { LocatorFunc = _TitleLocator };
          static Func<BindableBase, ValueContainer<String>> _TitleLocator = RegisterContainerLocator<String>("Title", model => model.Initialize("Title", ref model._Title, ref _TitleLocator, _TitleDefaultValueFactory));
          static Func<String> _TitleDefaultValueFactory = () => "Title is Here";
          #endregion

          #region Life Time Event Handling

          ///// <summary>
          ///// This will be invoked by view when this viewmodel instance is set to view's ViewModel property. 
          ///// </summary>
          ///// <param name="view">Set target</param>
          ///// <param name="oldValue">Value before set.</param>
          ///// <returns>Task awaiter</returns>
          //protected override Task OnBindedToView(MVVMSidekick.Views.IView view, IViewModel oldValue)
          //{
          //    return base.OnBindedToView(view, oldValue);
          //}

          ///// <summary>
          ///// This will be invoked by view when this instance of viewmodel in ViewModel property is overwritten.
          ///// </summary>
          ///// <param name="view">Overwrite target view.</param>
          ///// <param name="newValue">The value replacing </param>
          ///// <returns>Task awaiter</returns>
          //protected override Task OnUnbindedFromView(MVVMSidekick.Views.IView view, IViewModel newValue)
          //{
          //    return base.OnUnbindedFromView(view, newValue);
          //}

          ///// <summary>
          ///// This will be invoked by view when the view fires Load event and this viewmodel instance is already in view's ViewModel property
          ///// </summary>
          ///// <param name="view">View that firing Load event</param>
          ///// <returns>Task awaiter</returns>
          //protected override Task OnBindedViewLoad(MVVMSidekick.Views.IView view)
          //{
          //    return base.OnBindedViewLoad(view);
          //}

          ///// <summary>
          ///// This will be invoked by view when the view fires Unload event and this viewmodel instance is still in view's  ViewModel property
          ///// </summary>
          ///// <param name="view">View that firing Unload event</param>
          ///// <returns>Task awaiter</returns>
          //protected override Task OnBindedViewUnload(MVVMSidekick.Views.IView view)
          //{
          //    return base.OnBindedViewUnload(view);
          //}

          ///// <summary>
          ///// <para>If dispose actions got exceptions, will handled here. </para>
          ///// </summary>
          ///// <param name="exceptions">
          ///// <para>The exception and dispose infomation</para>
          ///// </param>
          //protected override async void OnDisposeExceptions(IList<DisposeInfo> exceptions)
          //{
          //    base.OnDisposeExceptions(exceptions);
          //    await TaskExHelper.Yield();
          //}

          #endregion
     }
}
