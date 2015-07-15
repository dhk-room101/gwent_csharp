using System.Reactive;
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
using System.IO;
using System.Xml.XPath;
using System.Xml.Linq;

namespace Gwent.ViewModels
{
     public class MainWindow_ViewModel : ViewModelBase<MainWindow_ViewModel>
     {
          //public var mcMessageQueue:W3MessageQueue;
          //public var mcChoiceDialog:W3ChoiceDialog;
          //public var mcEndGameDialog:GwintEndGameDialog;
          //public var btnSkipTurn:InputFeedbackButton;
          //public var mcCloseBtn:ConditionalCloseButton;
          //public var mcTutorials:GwintTutorial;

          public bool debug = true;
          public bool tutorialsOn = false;
          private const int SKIP_TURN_HOLD_DELAY = 1000;
          public GwintGameFlowController gameFlowController;
          public GwintPlayerRenderer mcPlayer1Renderer;
          public GwintPlayerRenderer mcPlayer2Renderer;
          public GwintDeckRenderer mcP1DeckRenderer = new GwintDeckRenderer();
          public GwintDeckRenderer mcP2DeckRenderer = new GwintDeckRenderer();
          public GwintBoardRenderer mcBoardRenderer;
          public CardFXManager mcCardFXManager;
          public static MainWindow_ViewModel mSingleton;//GwintGameMenu
          public CardManager _cardManager;
          
          public MainWindow_ViewModel()
          {
               PlayerSiegeCards = new ObservableCollection<Card>();
               gameFlowController = new GwintGameFlowController();

               _cardManager = CardManager.getInstance();
               _cardManager.cardValues = new GwintCardValues();

               //create empty decks
               createDecks();

               //handle card template from XML
               getCardTemplates();

               if (debug)
               {
                    randomizeFaction();
               }

               //board renderer
               mcBoardRenderer = new GwintBoardRenderer();
               _cardManager.boardRenderer = mcBoardRenderer;

               //players renderers
               mcPlayer1Renderer = new GwintPlayerRenderer { playerID = 0, playerName = "player" };
               mcPlayer2Renderer = new GwintPlayerRenderer { playerID = 1, playerName = "AI" };
               
               _cardManager.playerRenderers.Add(mcPlayer1Renderer);
               _cardManager.playerRenderers.Add(mcPlayer2Renderer);

               //test
               if (debug)
               {
                    //testCardsCalculations();
               }

               mSingleton = this;
               //Previous();
          }

          private void createDecks()
          {
               _cardManager.completeDecks = new List<GwintDeck>[2];
               _cardManager.completeDecks[0] = new List<GwintDeck>();
               _cardManager.completeDecks[1] = new List<GwintDeck>();

               //complete decks for player ID 0
               _cardManager.completeDecks[0].Add(new GwintDeck { deckName = "Neutral", selectedKingIndex = 0});
               _cardManager.completeDecks[0].Add(new GwintDeck { deckName = "NorthKingdom", selectedKingIndex = 1 });
               _cardManager.completeDecks[0].Add(new GwintDeck { deckName = "Nilfgaard", selectedKingIndex = 2 });
               _cardManager.completeDecks[0].Add(new GwintDeck { deckName = "Scoiatael", selectedKingIndex = 3 });
               _cardManager.completeDecks[0].Add(new GwintDeck { deckName = "NoMansLand", selectedKingIndex = 4 });

               //complete decks for player ID 1
               _cardManager.completeDecks[1].Add(new GwintDeck { deckName = "Neutral", selectedKingIndex = 0 });
               _cardManager.completeDecks[1].Add(new GwintDeck { deckName = "NorthKingdom", selectedKingIndex = 1 });
               _cardManager.completeDecks[1].Add(new GwintDeck { deckName = "Nilfgaard", selectedKingIndex = 2 });
               _cardManager.completeDecks[1].Add(new GwintDeck { deckName = "Scoiatael", selectedKingIndex = 3 });
               _cardManager.completeDecks[1].Add(new GwintDeck { deckName = "NoMansLand", selectedKingIndex = 4 });
          }

          private void randomizeCards(int playerID)
          {
               SafeRandom random = new SafeRandom();
               int i = 0;
               while (i < 10)
               {
                    bool found = false;
                    int cardIndex = random.Next(_cardManager.playerDeckDefinitions[playerID].cardIndicesInDeck.Count);
                    int cardID = _cardManager.playerDeckDefinitions[playerID].cardIndicesInDeck.ElementAt(cardIndex);
                    foreach (int idx in _cardManager.playerDeckDefinitions[playerID].cardIndices)
                    {
                         if (idx == cardID && found == false)
                         {
                              found = true;
                              Console.WriteLine("randomize cards: match found for player {3}, try again {0}, {1}, {2}", i, idx, cardID, playerID);
                         }
                    }
                    if (found == false)
                    {
                         _cardManager.playerDeckDefinitions[playerID].cardIndices.Add(cardID);
                         ++i;
                    }
               }
          }

          private void randomizeFaction()
          {
               SafeRandom random = new SafeRandom();
               int randomFaction;

               randomFaction = random.Next(1,5);//P1
               _cardManager.playerDeckDefinitions[CardManager.PLAYER_1] = _cardManager.completeDecks[CardManager.PLAYER_1].ElementAt(randomFaction);
               randomizeCards(CardManager.PLAYER_1);

               randomFaction = random.Next(1,5);//P2
               _cardManager.playerDeckDefinitions[CardManager.PLAYER_2] = _cardManager.completeDecks[CardManager.PLAYER_2].ElementAt(randomFaction);
               randomizeCards(CardManager.PLAYER_2);
               
               foreach (GwintDeck deck in _cardManager.playerDeckDefinitions)
               {
                    Console.WriteLine("player faction is {0}, with selectedKingIndex {1}", deck.deckName, deck.selectedKingIndex);
               }
          }

          private void getCardTemplates()
          {
               string gwent_cards = MainWindow.gamePath + "\\def_gwint_cards_final.xml";
               if (File.Exists(gwent_cards))
               {
                    var doc = new XPathDocument(gwent_cards);
                    var nav = doc.CreateNavigator();
                    var locations = nav.Select("/redxml/custom/gwint_card_definitions_final");
                    while (locations.MoveNext() == true)
                    {
                         var cards = locations.Current.Select("card");
                         while (cards.MoveNext() == true)
                         {
                              int index = Convert.ToInt32(cards.Current.GetAttribute("index", ""));
                              string title = cards.Current.GetAttribute("title", "");
                              string description = cards.Current.GetAttribute("description", "");
                              int power = Convert.ToInt32(cards.Current.GetAttribute("power", ""));
                              string picture = cards.Current.GetAttribute("picture", "");
                              int faction_index = CardTemplate.factionStringToInt(cards.Current.GetAttribute("faction_index", ""));
                              int typeArray = 0;

                              var xml_type_flags = cards.Current.Select("type_flags");
                              List<int> type_flags = new List<int>();
                              while (xml_type_flags.MoveNext() == true)
                              {
                                   var xml_flag = xml_type_flags.Current.Select("flag");
                                   while (xml_flag.MoveNext() == true)
                                   {
                                        int flag = CardTemplate.typeStringToInt(xml_flag.Current.GetAttribute("name", ""));
                                        type_flags.Add(flag);
                                   }
                              }

                              if (type_flags.Count > 0)
                              {
                                   typeArray = type_flags.ElementAt(0);
                              }

                              if (type_flags.Count > 1)
                              {
                                   for (var i = 1; i < type_flags.Count; i++)
                                   {
                                        typeArray |= type_flags.ElementAt(i);
                                   }
                              }
                              
                              var xml_effect_flags = cards.Current.Select("effect_flags");
                              List<int> effect_flags = new List<int>();
                              while (xml_effect_flags.MoveNext() == true)
                              {
                                   var xml_flag = xml_effect_flags.Current.Select("flag");
                                   while (xml_flag.MoveNext() == true)
                                   {
                                        int flag = CardTemplate.effectStringToInt(xml_flag.Current.GetAttribute("name", ""));
                                        effect_flags.Add(flag);
                                   }
                              }

                              var xml_summonFlags = cards.Current.Select("summonFlags");
                              List<int> summonFlags = new List<int>();
                              while (xml_summonFlags.MoveNext() == true)
                              {
                                   var xml_card = xml_summonFlags.Current.Select("card");
                                   while (xml_card.MoveNext() == true)
                                   {
                                        int card = Convert.ToInt32(xml_card.Current.GetAttribute("id", ""));
                                        summonFlags.Add(card);
                                   }
                              }

                              CardTemplate template = new CardTemplate
                              {
                                   index = index,
                                   power = power,
                                   title = title,
                                   description = description,
                                   imageLoc = picture,
                                   factionIdx = faction_index,
                                   typeArray = typeArray,
                                   effectFlags = effect_flags,
                                   summonFlags = summonFlags
                              };

                              _cardManager._cardTemplates[index] = template;

                              addToDeck(template);
                              //Console.WriteLine("something");
                         }
                    }

                    if (debug)
                    {
                         for (var i = 0; i < _cardManager.completeDecks.Length; i++)
                         {
                              foreach (GwintDeck deck in _cardManager.completeDecks[i])
                              {
                                   Console.WriteLine("deck {0} for player {1} has {2} cards", deck.deckName, i, deck.cardIndicesInDeck.Count);
                              }
                         }
                    }
               }
          }

          private void addToDeck(CardTemplate template)
          {
               string factionString = template.getFactionString();
               for (var i = 0; i < _cardManager.completeDecks.Length; i++)
               {
                    foreach (GwintDeck deck in _cardManager.completeDecks[i])
                    {
                         if (deck.deckName == factionString)
                         {
                              deck.cardIndicesInDeck.Add(template.index);
                         }
                    }
               }
          }

          protected void testCardsCalculations()
          {
               CardInstance cardInstance = null;
               CardTemplate cardTemplate = null;
               int counter = 0;
               List<CardInstance> cardInstanceList = new List<CardInstance>();
               Console.WriteLine("GFX --------------------------------------------------------- Commencing card test ---------------------------------------------------------");
               Console.WriteLine("GFX ================================================== Creating temporary card instances ===================================================");
               Dictionary<int, CardTemplate> templateDictionary = _cardManager._cardTemplates;
               foreach (KeyValuePair<int, CardTemplate> entry in templateDictionary)
               {
                    cardTemplate = entry.Value;//entry.Key is int
                    cardInstance = new CardInstance();
                    cardInstance.templateId = cardTemplate.index;
                    cardInstance.templateRef = cardTemplate;
                    cardInstance.owningPlayer = CardManager.PLAYER_1;
                    cardInstance.listsPlayer = CardManager.PLAYER_1;
                    cardInstance.instanceId = 100;
                    cardInstanceList.Add(cardInstance);
               }
               Console.WriteLine("GFX - Successfully created: {0} card instances", cardInstanceList.Count);
               counter = 0;
               while (counter < cardInstanceList.Count)
               {
                    Console.WriteLine("GFX - Checking Card with ID: {0} --------------------------", cardInstanceList[counter].templateId);
                    Console.WriteLine("GFX ---------------------------------------------------------");
                    Console.WriteLine("GFX - template Ref: " + cardInstanceList[counter].templateRef.title);
                    Console.WriteLine("GFX - instance info: " + cardInstanceList[counter].toString());
                    Console.WriteLine("GFX - recalculating optimal transaction for card");
                    cardInstanceList[counter].recalculatePowerPotential(_cardManager);
                    Console.WriteLine("GFX - successfully recalculated following power info: ");
                    Console.WriteLine("GFX - " + cardInstanceList[counter].getOptimalTransaction().toString());
                    ++counter;
               }
               Console.WriteLine("GFX ================================ Successfully Finished Test of Card Instances ====================================");
               Console.WriteLine("GFX ------------------------------------------------------------------------------------------------------------------");
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

