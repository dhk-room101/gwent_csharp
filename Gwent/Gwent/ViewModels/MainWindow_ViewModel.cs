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
          private Image image;

          public MainWindow_ViewModel()
          {
               DefaultSettings();
               
               InitializeHolders();

               gameFlowController = new GwintGameFlowController();

               _cardManager = CardManager.getInstance();
               _cardManager.cardValues = new GwintCardValues();

               //create empty decks
               createDecks();

               //handle card template from XML
               getCardTemplates();

               if (debug) { randomizeFaction(); }

               InitializeRenderers();

               /*if (debug) { testCardsCalculations(); }*/

               mSingleton = this;
          }

          private void DefaultSettings()
          {
               DebugBorder = "0";
               CardZoomFrameVisibility = "Hidden";
               SelectedCardSlot = null;
          }

          private void InitializeRenderers()
          {
               //board renderer
               mcBoardRenderer = new GwintBoardRenderer();
               _cardManager.boardRenderer = mcBoardRenderer;

               //players renderers
               mcPlayer1Renderer = new GwintPlayerRenderer { playerID = 0, playerName = "player" };
               mcPlayer2Renderer = new GwintPlayerRenderer { playerID = 1, playerName = "AI" };

               _cardManager.playerRenderers.Add(mcPlayer1Renderer);
               _cardManager.playerRenderers.Add(mcPlayer2Renderer);
          }

          private void InitializeHolders()
          {
               Holders = new List<ObservableCollection<CardSlot>>();

               WeatherHolder = new ObservableCollection<CardSlot>();
               P1LeaderHolder = new ObservableCollection<CardSlot>();
               P2LeaderHolder = new ObservableCollection<CardSlot>();
               P1DeckHolder = new ObservableCollection<CardSlot>();
               P2DeckHolder = new ObservableCollection<CardSlot>();
               P1HandHolder = new ObservableCollection<CardSlot>();
               P2HandHolder = new ObservableCollection<CardSlot>();
               P1GraveyardHolder = new ObservableCollection<CardSlot>();
               P2GraveyardHolder = new ObservableCollection<CardSlot>();
               P1SiegeHolder = new ObservableCollection<CardSlot>();
               P2SiegeHolder = new ObservableCollection<CardSlot>();
               P1RangeHolder = new ObservableCollection<CardSlot>();
               P2RangeHolder = new ObservableCollection<CardSlot>();
               P1MeleeHolder = new ObservableCollection<CardSlot>();
               P2MeleeHolder = new ObservableCollection<CardSlot>();
               P1SiegeModifHolder = new ObservableCollection<CardSlot>();
               P2SiegeModifHolder = new ObservableCollection<CardSlot>();
               P1RangeModifHolder = new ObservableCollection<CardSlot>();
               P2RangeModifHolder = new ObservableCollection<CardSlot>();
               P1MeleeModifHolder = new ObservableCollection<CardSlot>();
               P2MeleeModifHolder = new ObservableCollection<CardSlot>();

               Holders.Add(WeatherHolder);
               Holders.Add(P1LeaderHolder);
               Holders.Add(P2LeaderHolder);
               Holders.Add(P1DeckHolder);
               Holders.Add(P2DeckHolder);
               Holders.Add(P1HandHolder);
               Holders.Add(P2HandHolder);
               Holders.Add(P1GraveyardHolder);
               Holders.Add(P2GraveyardHolder);
               Holders.Add(P1SiegeHolder);
               Holders.Add(P2SiegeHolder);
               Holders.Add(P1RangeHolder);
               Holders.Add(P2RangeHolder);
               Holders.Add(P1MeleeHolder);
               Holders.Add(P2MeleeHolder);
               Holders.Add(P1SiegeModifHolder);
               Holders.Add(P2SiegeModifHolder);
               Holders.Add(P1RangeModifHolder);
               Holders.Add(P2RangeModifHolder);
               Holders.Add(P1MeleeModifHolder);
               Holders.Add(P2MeleeModifHolder);
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
               bool leaderDrawn = false;
               int kingIndex = _cardManager.playerDeckDefinitions[playerID].selectedKingIndex;
               SafeRandom random = new SafeRandom();
               int i = 0;
               while (i < 10)
               {
                    int cardIndex = random.Next(_cardManager.playerDeckDefinitions[playerID].cardIndicesInDeck.Count); 
                    int cardID = _cardManager.playerDeckDefinitions[playerID].cardIndicesInDeck.ElementAt(cardIndex);
                    //leader cards
                    if (cardID >= kingIndex * 100 && cardID < (kingIndex * 100 + 5) && leaderDrawn == false)
                    {
                         leaderDrawn = true;
                         Console.WriteLine("player {0} leader found {1}", playerID, cardID);
                         _cardManager.playerDeckDefinitions[playerID].cardIndices.Add(cardID);
                         ++i;
                    }
                    else if (cardID >= (kingIndex * 100 + 5))//regular cards
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

          public CardSlot getSlot(CardInstance card)
          {
               Uri uri;
               image = new Image();
               uri = new Uri("pack://application:,,,/Images/Cards/" + card.templateRef.imageLoc + ".jpg");
               image.Source = new BitmapImage(uri);
               CardSlot result = new CardSlot { cardImage = image };
               ReferenceSlot = result;
               return result;
          }

          public void Previous()
          {
               /*//Overlapping
               Thickness margin = rectangle.Margin;
               margin.Left = -15;
               rectangle.Margin = margin;*/

               /*Uri uri;
               Image imageRange = new Image();
               uri = new Uri("pack://application:,,,/Images/range.jpg");
               imageRange.Source = new BitmapImage(uri);

               PlayerSiegeCards.Add(imageRange);*/

               Title = "Gwent";
          }

          public CardSlot SelectedCardSlot
          {
               get { return _SelectedCardSlotLocator(this).Value; }
               set 
               { 
                    _SelectedCardSlotLocator(this).SetValueAndTryNotify(value);
                    if (value != null)
                    {
                         //if current player is Player( not AI)
                         if (GwintGameFlowController.getInstance().currentPlayer == 0)
                         {
                              HumanPlayerController hpc = (HumanPlayerController)GwintGameFlowController.getInstance().playerControllers[GwintGameFlowController.getInstance().currentPlayer];
                              hpc.handleCardChosen();
                         }
                         //show card zoom frame
                         CardZoomFrameVisibility = "Visible";
                    }
                    else
                    { 
                         //hide card zoom frame
                         CardZoomFrameVisibility = "Hidden";
                    }
               }
          }

          #region Property CardSlot SelectedCardSlot Setup
          protected Property<CardSlot> _SelectedCardSlot = new Property<CardSlot> { LocatorFunc = _SelectedCardSlotLocator };
          static Func<BindableBase, ValueContainer<CardSlot>> _SelectedCardSlotLocator = RegisterContainerLocator<CardSlot>("SelectedCardSlot", model => model.Initialize("SelectedCardSlot", ref model._SelectedCardSlot, ref _SelectedCardSlotLocator, _SelectedCardSlotDefaultValueFactory));
          static Func<CardSlot> _SelectedCardSlotDefaultValueFactory = null;
          #endregion

          public CardSlot ReferenceSlot
          {
               get { return _ReferenceSlotLocator(this).Value; }
               set { _ReferenceSlotLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property CardSlot ReferenceSlot Setup
          protected Property<CardSlot> _ReferenceSlot = new Property<CardSlot> { LocatorFunc = _ReferenceSlotLocator };
          static Func<BindableBase, ValueContainer<CardSlot>> _ReferenceSlotLocator = RegisterContainerLocator<CardSlot>("ReferenceSlot", model => model.Initialize("ReferenceSlot", ref model._ReferenceSlot, ref _ReferenceSlotLocator, _ReferenceSlotDefaultValueFactory));
          static Func<CardSlot> _ReferenceSlotDefaultValueFactory = null;
          #endregion

          public List<ObservableCollection<CardSlot>> Holders
          {
               get { return _HoldersLocator(this).Value; }
               set { _HoldersLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property List<ObservableCollection<CardSlot>> Holders Setup
          protected Property<List<ObservableCollection<CardSlot>>> _Holders = new Property<List<ObservableCollection<CardSlot>>> { LocatorFunc = _HoldersLocator };
          static Func<BindableBase, ValueContainer<List<ObservableCollection<CardSlot>>>> _HoldersLocator = RegisterContainerLocator<List<ObservableCollection<CardSlot>>>("Holders", model => model.Initialize("Holders", ref model._Holders, ref _HoldersLocator, _HoldersDefaultValueFactory));
          static Func<List<ObservableCollection<CardSlot>>> _HoldersDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> WeatherHolder
          {
               get { return _WeatherHolderLocator(this).Value; }
               set { _WeatherHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> WeatherHolder Setup
          protected Property<ObservableCollection<CardSlot>> _WeatherHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _WeatherHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _WeatherHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("WeatherHolder", model => model.Initialize("WeatherHolder", ref model._WeatherHolder, ref _WeatherHolderLocator, _WeatherHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _WeatherHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P1LeaderHolder
          {
               get { return _P1LeaderHolderLocator(this).Value; }
               set { _P1LeaderHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P1LeaderHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P1LeaderHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P1LeaderHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P1LeaderHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P1LeaderHolder", model => model.Initialize("P1LeaderHolder", ref model._P1LeaderHolder, ref _P1LeaderHolderLocator, _P1LeaderHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P1LeaderHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P2LeaderHolder
          {
               get { return _P2LeaderHolderLocator(this).Value; }
               set { _P2LeaderHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P2LeaderHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P2LeaderHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P2LeaderHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P2LeaderHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P2LeaderHolder", model => model.Initialize("P2LeaderHolder", ref model._P2LeaderHolder, ref _P2LeaderHolderLocator, _P2LeaderHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P2LeaderHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P1DeckHolder
          {
               get { return _P1DeckHolderLocator(this).Value; }
               set { _P1DeckHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P1DeckHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P1DeckHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P1DeckHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P1DeckHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P1DeckHolder", model => model.Initialize("P1DeckHolder", ref model._P1DeckHolder, ref _P1DeckHolderLocator, _P1DeckHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P1DeckHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P2DeckHolder
          {
               get { return _P2DeckHolderLocator(this).Value; }
               set { _P2DeckHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P2DeckHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P2DeckHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P2DeckHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P2DeckHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P2DeckHolder", model => model.Initialize("P2DeckHolder", ref model._P2DeckHolder, ref _P2DeckHolderLocator, _P2DeckHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P2DeckHolderDefaultValueFactory = null;
          #endregion
          
          public ObservableCollection<CardSlot> P1HandHolder
          {
               get { return _P1HandHolderLocator(this).Value; }
               set { _P1HandHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P1HandHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P1HandHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P1HandHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P1HandHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P1HandHolder", model => model.Initialize("P1HandHolder", ref model._P1HandHolder, ref _P1HandHolderLocator, _P1HandHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P1HandHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P2HandHolder
          {
               get { return _P2HandHolderLocator(this).Value; }
               set { _P2HandHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P2HandHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P2HandHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P2HandHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P2HandHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P2HandHolder", model => model.Initialize("P2HandHolder", ref model._P2HandHolder, ref _P2HandHolderLocator, _P2HandHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P2HandHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P1GraveyardHolder
          {
               get { return _P1GraveyardHolderLocator(this).Value; }
               set { _P1GraveyardHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P1GraveyardHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P1GraveyardHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P1GraveyardHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P1GraveyardHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P1GraveyardHolder", model => model.Initialize("P1GraveyardHolder", ref model._P1GraveyardHolder, ref _P1GraveyardHolderLocator, _P1GraveyardHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P1GraveyardHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P2GraveyardHolder
          {
               get { return _P2GraveyardHolderLocator(this).Value; }
               set { _P2GraveyardHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P2GraveyardHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P2GraveyardHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P2GraveyardHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P2GraveyardHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P2GraveyardHolder", model => model.Initialize("P2GraveyardHolder", ref model._P2GraveyardHolder, ref _P2GraveyardHolderLocator, _P2GraveyardHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P2GraveyardHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P1SiegeHolder
          {
               get { return _P1SiegeHolderLocator(this).Value; }
               set { _P1SiegeHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P1SiegeHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P1SiegeHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P1SiegeHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P1SiegeHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P1SiegeHolder", model => model.Initialize("P1SiegeHolder", ref model._P1SiegeHolder, ref _P1SiegeHolderLocator, _P1SiegeHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P1SiegeHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P2SiegeHolder
          {
               get { return _P2SiegeHolderLocator(this).Value; }
               set { _P2SiegeHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P2SiegeHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P2SiegeHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P2SiegeHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P2SiegeHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P2SiegeHolder", model => model.Initialize("P2SiegeHolder", ref model._P2SiegeHolder, ref _P2SiegeHolderLocator, _P2SiegeHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P2SiegeHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P1RangeHolder
          {
               get { return _P1RangeHolderLocator(this).Value; }
               set { _P1RangeHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P1RangeHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P1RangeHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P1RangeHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P1RangeHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P1RangeHolder", model => model.Initialize("P1RangeHolder", ref model._P1RangeHolder, ref _P1RangeHolderLocator, _P1RangeHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P1RangeHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P2RangeHolder
          {
               get { return _P2RangeHolderLocator(this).Value; }
               set { _P2RangeHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P2RangeHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P2RangeHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P2RangeHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P2RangeHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P2RangeHolder", model => model.Initialize("P2RangeHolder", ref model._P2RangeHolder, ref _P2RangeHolderLocator, _P2RangeHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P2RangeHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P1MeleeHolder
          {
               get { return _P1MeleeHolderLocator(this).Value; }
               set { _P1MeleeHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P1MeleeHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P1MeleeHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P1MeleeHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P1MeleeHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P1MeleeHolder", model => model.Initialize("P1MeleeHolder", ref model._P1MeleeHolder, ref _P1MeleeHolderLocator, _P1MeleeHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P1MeleeHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P2MeleeHolder
          {
               get { return _P2MeleeHolderLocator(this).Value; }
               set { _P2MeleeHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P2MeleeHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P2MeleeHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P2MeleeHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P2MeleeHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P2MeleeHolder", model => model.Initialize("P2MeleeHolder", ref model._P2MeleeHolder, ref _P2MeleeHolderLocator, _P2MeleeHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P2MeleeHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P1SiegeModifHolder
          {
               get { return _P1SiegeModifHolderLocator(this).Value; }
               set { _P1SiegeModifHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P1SiegeModifHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P1SiegeModifHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P1SiegeModifHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P1SiegeModifHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P1SiegeModifHolder", model => model.Initialize("P1SiegeModifHolder", ref model._P1SiegeModifHolder, ref _P1SiegeModifHolderLocator, _P1SiegeModifHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P1SiegeModifHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P2SiegeModifHolder
          {
               get { return _P2SiegeModifHolderLocator(this).Value; }
               set { _P2SiegeModifHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P2SiegeModifHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P2SiegeModifHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P2SiegeModifHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P2SiegeModifHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P2SiegeModifHolder", model => model.Initialize("P2SiegeModifHolder", ref model._P2SiegeModifHolder, ref _P2SiegeModifHolderLocator, _P2SiegeModifHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P2SiegeModifHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P1RangeModifHolder
          {
               get { return _P1RangeModifHolderLocator(this).Value; }
               set { _P1RangeModifHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P1RangeModifHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P1RangeModifHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P1RangeModifHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P1RangeModifHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P1RangeModifHolder", model => model.Initialize("P1RangeModifHolder", ref model._P1RangeModifHolder, ref _P1RangeModifHolderLocator, _P1RangeModifHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P1RangeModifHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P2RangeModifHolder
          {
               get { return _P2RangeModifHolderLocator(this).Value; }
               set { _P2RangeModifHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P2RangeModifHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P2RangeModifHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P2RangeModifHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P2RangeModifHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P2RangeModifHolder", model => model.Initialize("P2RangeModifHolder", ref model._P2RangeModifHolder, ref _P2RangeModifHolderLocator, _P2RangeModifHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P2RangeModifHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P1MeleeModifHolder
          {
               get { return _P1MeleeModifHolderLocator(this).Value; }
               set { _P1MeleeModifHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P1MeleeModifHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P1MeleeModifHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P1MeleeModifHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P1MeleeModifHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P1MeleeModifHolder", model => model.Initialize("P1MeleeModifHolder", ref model._P1MeleeModifHolder, ref _P1MeleeModifHolderLocator, _P1MeleeModifHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P1MeleeModifHolderDefaultValueFactory = null;
          #endregion

          public ObservableCollection<CardSlot> P2MeleeModifHolder
          {
               get { return _P2MeleeModifHolderLocator(this).Value; }
               set { _P2MeleeModifHolderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property ObservableCollection<CardSlot> P2MeleeModifHolder Setup
          protected Property<ObservableCollection<CardSlot>> _P2MeleeModifHolder = new Property<ObservableCollection<CardSlot>> { LocatorFunc = _P2MeleeModifHolderLocator };
          static Func<BindableBase, ValueContainer<ObservableCollection<CardSlot>>> _P2MeleeModifHolderLocator = RegisterContainerLocator<ObservableCollection<CardSlot>>("P2MeleeModifHolder", model => model.Initialize("P2MeleeModifHolder", ref model._P2MeleeModifHolder, ref _P2MeleeModifHolderLocator, _P2MeleeModifHolderDefaultValueFactory));
          static Func<ObservableCollection<CardSlot>> _P2MeleeModifHolderDefaultValueFactory = null;
          #endregion
          
          public double ObservedHeight
          {
               get { return _ObservedHeightLocator(this).Value; }
               set { _ObservedHeightLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property double ObservedHeight Setup
          protected Property<double> _ObservedHeight = new Property<double> { LocatorFunc = _ObservedHeightLocator };
          static Func<BindableBase, ValueContainer<double>> _ObservedHeightLocator = RegisterContainerLocator<double>("ObservedHeight", model => model.Initialize("ObservedHeight", ref model._ObservedHeight, ref _ObservedHeightLocator, _ObservedHeightDefaultValueFactory));
          static Func<double> _ObservedHeightDefaultValueFactory = null;
          #endregion

          public double ObservedWidth
          {
               get { return _ObservedWidthLocator(this).Value; }
               set { _ObservedWidthLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property double ObservedWidth Setup
          protected Property<double> _ObservedWidth = new Property<double> { LocatorFunc = _ObservedWidthLocator };
          static Func<BindableBase, ValueContainer<double>> _ObservedWidthLocator = RegisterContainerLocator<double>("ObservedWidth", model => model.Initialize("ObservedWidth", ref model._ObservedWidth, ref _ObservedWidthLocator, _ObservedWidthDefaultValueFactory));
          static Func<double> _ObservedWidthDefaultValueFactory = null;
          #endregion

          public String CardZoomFrameVisibility
          {
               get { return _CardZoomFrameVisibilityLocator(this).Value; }
               set { _CardZoomFrameVisibilityLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property String CardZoomFrameVisibility Setup
          protected Property<String> _CardZoomFrameVisibility = new Property<String> { LocatorFunc = _CardZoomFrameVisibilityLocator };
          static Func<BindableBase, ValueContainer<String>> _CardZoomFrameVisibilityLocator = RegisterContainerLocator<String>("CardZoomFrameVisibility", model => model.Initialize("CardZoomFrameVisibility", ref model._CardZoomFrameVisibility, ref _CardZoomFrameVisibilityLocator, _CardZoomFrameVisibilityDefaultValueFactory));
          static Func<String> _CardZoomFrameVisibilityDefaultValueFactory = null;
          #endregion
          
          public String DebugBorder
          {
               get { return _DebugBorderLocator(this).Value; }
               set { _DebugBorderLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property String DebugBorder Setup
          protected Property<String> _DebugBorder = new Property<String> { LocatorFunc = _DebugBorderLocator };
          static Func<BindableBase, ValueContainer<String>> _DebugBorderLocator = RegisterContainerLocator<String>("DebugBorder", model => model.Initialize("DebugBorder", ref model._DebugBorder, ref _DebugBorderLocator, _DebugBorderDefaultValueFactory));
          static Func<String> _DebugBorderDefaultValueFactory = () => "0.5";
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

