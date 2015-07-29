using System.Reactive.Linq;
using MVVMSidekick.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Gwent.Core;
using Gwent.Models;
using System.IO;
using System.Xml.XPath;
using System.Xml;

namespace Gwent.ViewModels
{
     public class MainWindow_ViewModel : ViewModelBase<MainWindow_ViewModel>
     {
          public bool debug { get; set; }
          private int cardsDealt { get; set; }
          public static MainWindow_ViewModel mSingleton { get; set; }
          public Dictionary<int, CardTemplate> cardTemplates { get; set; }
          public List<CardDeck>[] completeDecks { get; set; }
          public List<CardDeck> currentGameDecks { get; set; }
          public List<GameRoundResult> roundResults { get; set; }
          public List<int> currentPlayerScores { get; set; }
          public bool[] currentRoundStatus { get; set; }
          
          public MainWindow_ViewModel()
          {
               //TO DO card values

               Title = "Gwent";

               mSingleton = this;

               InitializeVariables();

               DefaultSettings();

               InitializeHolders();

               //create empty decks
               createDecks();

               //handle card template from XML
               getCardTemplates();

               if (debug) { randomizeFaction(); }
          }

          private void InitializeVariables()
          {
               cardTemplates = new Dictionary<int, CardTemplate>();

               //initialize current game decks
               currentGameDecks = new List<CardDeck>();//randomizeFaction
               currentGameDecks.Add(new CardDeck());
               currentGameDecks.Add(new CardDeck());

               //initialize current game score(2 out of 3)
               roundResults = new List<GameRoundResult>();
               roundResults.Add(new GameRoundResult());
               roundResults.Add(new GameRoundResult());
               roundResults.Add(new GameRoundResult());

               //initialize current score for players
               currentPlayerScores = new List<int>();
               currentPlayerScores.Add(0);
               currentPlayerScores.Add(0);

               //Initialize the scores used for display
               P1MeleeScore = 0;
               P1RangeScore = 0;
               P1SiegeScore = 0;
               P1Score = P1MeleeScore + P1RangeScore + P1SiegeScore;

               P2MeleeScore = 0;
               P2RangeScore = 0;
               P2SiegeScore = 0;
               P2Score = P2MeleeScore + P2RangeScore + P2SiegeScore;

               //Initialize current round status: true/active, false/pass
               currentRoundStatus = new bool[2];
               currentRoundStatus[ValuesRepository.PLAYER_1] = true;//active
               currentRoundStatus[ValuesRepository.PLAYER_2] = true;//active
          }

          private void DefaultSettings()
          {
               debug = true;
               DebugBorder = "0";
               CardZoomFrameVisibility = "Hidden";
               SelectedCardSlot = null;
               IsAITurn = false;
               AIAttitude = ValuesRepository.TACTIC_NONE;
          }

          private void InitializeHolders()
          {
               Holders = new List<ObservableCollection<CardSlot>>();

               WeatherHolder = new ObservableCollection<CardSlot>();//0

               P1LeaderHolder = new ObservableCollection<CardSlot>();//1
               P1DeckHolder = new ObservableCollection<CardSlot>();//2
               P1HandHolder = new ObservableCollection<CardSlot>();//3
               P1GraveyardHolder = new ObservableCollection<CardSlot>();//4
               P1SiegeHolder = new ObservableCollection<CardSlot>();//5
               P1RangeHolder = new ObservableCollection<CardSlot>();//6
               P1MeleeHolder = new ObservableCollection<CardSlot>();//7
               P1SiegeModifHolder = new ObservableCollection<CardSlot>();//8
               P1RangeModifHolder = new ObservableCollection<CardSlot>();//9
               P1MeleeModifHolder = new ObservableCollection<CardSlot>();//10

               P2LeaderHolder = new ObservableCollection<CardSlot>();//11
               P2DeckHolder = new ObservableCollection<CardSlot>();//12
               P2HandHolder = new ObservableCollection<CardSlot>();//13
               P2GraveyardHolder = new ObservableCollection<CardSlot>();//14
               P2SiegeHolder = new ObservableCollection<CardSlot>();//15
               P2RangeHolder = new ObservableCollection<CardSlot>();//16
               P2MeleeHolder = new ObservableCollection<CardSlot>();//17
               P2SiegeModifHolder = new ObservableCollection<CardSlot>();//18
               P2RangeModifHolder = new ObservableCollection<CardSlot>();//19
               P2MeleeModifHolder = new ObservableCollection<CardSlot>();//20

               Holders.Add(WeatherHolder);

               Holders.Add(P1LeaderHolder);
               Holders.Add(P1DeckHolder);
               Holders.Add(P1HandHolder);
               Holders.Add(P1GraveyardHolder);
               Holders.Add(P1SiegeHolder);
               Holders.Add(P1RangeHolder);
               Holders.Add(P1MeleeHolder);
               Holders.Add(P1SiegeModifHolder);
               Holders.Add(P1RangeModifHolder);
               Holders.Add(P1MeleeModifHolder);

               Holders.Add(P2LeaderHolder);
               Holders.Add(P2DeckHolder);
               Holders.Add(P2HandHolder);
               Holders.Add(P2GraveyardHolder);
               Holders.Add(P2SiegeHolder);
               Holders.Add(P2RangeHolder);
               Holders.Add(P2MeleeHolder);
               Holders.Add(P2SiegeModifHolder);
               Holders.Add(P2RangeModifHolder);
               Holders.Add(P2MeleeModifHolder);
          }

          private void createDecks()
          {
               completeDecks = new List<CardDeck>[2];
               completeDecks[ValuesRepository.PLAYER_1] = new List<CardDeck>();
               completeDecks[ValuesRepository.PLAYER_2] = new List<CardDeck>();

               //complete decks for player ID 0
               completeDecks[ValuesRepository.PLAYER_1].Add(new CardDeck { deckTypeName = "Neutral", deckTypeIndex = 0 });
               completeDecks[ValuesRepository.PLAYER_1].Add(new CardDeck { deckTypeName = "NorthKingdom", deckTypeIndex = 1 });
               completeDecks[ValuesRepository.PLAYER_1].Add(new CardDeck { deckTypeName = "Nilfgaard", deckTypeIndex = 2 });
               completeDecks[ValuesRepository.PLAYER_1].Add(new CardDeck { deckTypeName = "Scoiatael", deckTypeIndex = 3 });
               completeDecks[ValuesRepository.PLAYER_1].Add(new CardDeck { deckTypeName = "NoMansLand", deckTypeIndex = 4 });

               //complete decks for player ID 1
               completeDecks[ValuesRepository.PLAYER_2].Add(new CardDeck { deckTypeName = "Neutral", deckTypeIndex = 0 });
               completeDecks[ValuesRepository.PLAYER_2].Add(new CardDeck { deckTypeName = "NorthKingdom", deckTypeIndex = 1 });
               completeDecks[ValuesRepository.PLAYER_2].Add(new CardDeck { deckTypeName = "Nilfgaard", deckTypeIndex = 2 });
               completeDecks[ValuesRepository.PLAYER_2].Add(new CardDeck { deckTypeName = "Scoiatael", deckTypeIndex = 3 });
               completeDecks[ValuesRepository.PLAYER_2].Add(new CardDeck { deckTypeName = "NoMansLand", deckTypeIndex = 4 });
          }

          private async void randomizeCards(int playerID)
          {
               //TO DO handle leader
               int deckTypeIndex = currentGameDecks[playerID].deckTypeIndex;
               SafeRandom random = new SafeRandom();
               int i = 0;
               while (i < 10)
               {
                    await ValuesRepository.PutTaskDelay(500);
                    int cardIndex = random.Next(Holders.ElementAt(playerID * 10 + ValuesRepository.CARD_LIST_LOC_DECK).Count);
                    CardSlot cardSlot = Holders.ElementAt(playerID * 10 + ValuesRepository.CARD_LIST_LOC_DECK).ElementAt(cardIndex);
                    if (playerID == ValuesRepository.PLAYER_1)
                    {
                         cardSlot.IsTransactionReady = true;
                    }
                    moveCard(cardSlot, ValuesRepository.CARD_LIST_LOC_HAND);
                    //Console.WriteLine("processed card {0} for player {1}", i, playerID);
                    ++i;
               }
               //Console.WriteLine("player {0} Drew {1} cards", playerID, i);
               ++cardsDealt;
               if (cardsDealt == 2)//both players got their cards
               {
                    CurrentState = "P1 Turn";
               }
               //Console.WriteLine("==================================");
          }

          private void randomizeCards2(int playerID)
          {

          }

          private void randomizeFaction()
          {
               /**this is done once per game
               *cards may get multiple instances
               *should employ some further rebalancing**/
               CurrentState = "Dealing";
               SafeRandom random = new SafeRandom();
               //List<int> deckInstance = new List<int>();
               int deckInstanceLength = 0;
               int randomFaction = 0;
               int counter = 0;

               randomFaction = random.Next(1, 5);//P1
               currentGameDecks[ValuesRepository.PLAYER_1] = completeDecks[ValuesRepository.PLAYER_1].ElementAt(randomFaction);

               //randomize current game Deck instance
               deckInstanceLength = random.Next(currentGameDecks[ValuesRepository.PLAYER_1].cardIndicesInDeck.Count - 10, currentGameDecks[ValuesRepository.PLAYER_1].cardIndicesInDeck.Count - 5) + random.Next(1, 5);
               //convert card indices from abstract deck into card slot in visual deck
               while (counter < deckInstanceLength)
               {
                    int cardIndex = random.Next(currentGameDecks[ValuesRepository.PLAYER_1].cardIndicesInDeck.Count);
                    int cardID = currentGameDecks[ValuesRepository.PLAYER_1].cardIndicesInDeck.ElementAt(cardIndex);
                    convertToSlotInDeckHolder(cardID, ValuesRepository.PLAYER_1, ValuesRepository.CARD_LIST_LOC_DECK);
                    ++counter;
               }
               randomizeCards(ValuesRepository.PLAYER_1);

               randomFaction = random.Next(1, 5);//P2
               currentGameDecks[ValuesRepository.PLAYER_2] = completeDecks[ValuesRepository.PLAYER_2].ElementAt(randomFaction);
               //randomize current game Deck instance
               deckInstanceLength = random.Next(currentGameDecks[ValuesRepository.PLAYER_2].cardIndicesInDeck.Count - 10, currentGameDecks[ValuesRepository.PLAYER_2].cardIndicesInDeck.Count - 5) + random.Next(1, 5);
               //reset counter
               counter = 0;
               //convert card indices from abstract deck into card slot in visual deck
               while (counter < deckInstanceLength)
               {
                    int cardIndex = random.Next(currentGameDecks[ValuesRepository.PLAYER_2].cardIndicesInDeck.Count);
                    int cardID = currentGameDecks[ValuesRepository.PLAYER_2].cardIndicesInDeck.ElementAt(cardIndex);
                    convertToSlotInDeckHolder(cardID, ValuesRepository.PLAYER_2, ValuesRepository.CARD_LIST_LOC_DECK);
                    ++counter;
               }
               randomizeCards(ValuesRepository.PLAYER_2);

               for (var i = 0; i < currentGameDecks.Count; i++)
               {
                    CardDeck deck = currentGameDecks.ElementAt(i);
                    Console.WriteLine("player {2} faction is {0}, with deckTypeIndex {1}", deck.deckTypeName, deck.deckTypeIndex, i);
                    Console.WriteLine("==================================================");
               }
          }

          private void getCardTemplates()
          {
               List<string> definitions = new List<string>();
               definitions.Add("def_gwint_cards_final.xml");
               definitions.Add("def_gwint_battle_king_cards.xml");

               foreach (String definition in definitions)
               {
                    string fileContents = GetResourceTextFile(definition);
                    if (fileContents != null)
                    {
                         var doc = new XmlDocument();
                         doc.LoadXml(fileContents);
                         var nav = doc.CreateNavigator();
                         var locations = nav.Select("/redxml/custom/card_definitions");
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
                                   int faction_index = ValuesRepository.factionStringToInt(cards.Current.GetAttribute("faction_index", ""));

                                   var xml_type_flags = cards.Current.Select("type_flags");
                                   List<int> type_flags = new List<int>();
                                   while (xml_type_flags.MoveNext() == true)
                                   {
                                        var xml_flag = xml_type_flags.Current.Select("flag");
                                        while (xml_flag.MoveNext() == true)
                                        {
                                             int flag = ValuesRepository.typeStringToInt(xml_flag.Current.GetAttribute("name", ""));
                                             type_flags.Add(flag);
                                        }
                                   }

                                   var xml_effect_flags = cards.Current.Select("effect_flags");
                                   List<int> effect_flags = new List<int>();
                                   while (xml_effect_flags.MoveNext() == true)
                                   {
                                        var xml_flag = xml_effect_flags.Current.Select("flag");
                                        while (xml_flag.MoveNext() == true)
                                        {
                                             int flag = ValuesRepository.effectStringToInt(xml_flag.Current.GetAttribute("name", ""));
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
                                        typeFlags = type_flags,
                                        effectFlags = effect_flags,
                                        summonFlags = summonFlags
                                   };

                                   cardTemplates[index] = template;

                                   //TO DO handle leaders
                                   if (index < 1000)
                                   {
                                        addToDeck(template);
                                   }
                              }
                         }

                    }
               }
               if (debug)
               {
                    for (var i = 0; i < completeDecks.Length; i++)
                    {
                         foreach (CardDeck deck in completeDecks[i])
                         {
                              Console.WriteLine("deck {0} for player {1} has {2} cards", deck.deckTypeName, i, deck.cardIndicesInDeck.Count);
                         }
                    }
               }
          }

          private void addToDeck(CardTemplate template)
          {
               string factionString = ValuesRepository.getFactionString(template.factionIdx);
               for (var i = 0; i < completeDecks.Length; i++)
               {
                    foreach (CardDeck deck in completeDecks[i])
                    {
                         if (deck.deckTypeName == factionString)
                         {
                              deck.cardIndicesInDeck.Add(template.index);
                         }
                    }
               }
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

          public void convertToSlotInDeckHolder(int cardID, int playerID, int listID)
          {
               //get template from cardID
               CardTemplate template = cardTemplates[cardID];

               //Get image for template
               Uri uri;
               Image image = new Image();
               uri = new Uri("pack://application:,,,/Images/Cards/" + template.imageLoc + ".jpg");
               image.Source = new BitmapImage(uri);

               //create new card slot with image
               CardSlot cardSlot = new CardSlot
               {
                    cardImage = image,
                    template = template,
                    power = template.power,
                    owningPlayer = playerID,
                    owningHolder = listID,
                    instance = Guid.NewGuid(),
                    IsTransactionReady = false
               };

               //Add card slot to holder, to be displayed
               Holders.ElementAt(playerID * 10 + listID).Add(cardSlot);
          }

          public void moveCard(CardSlot card, int listID)
          {
               bool cardFound = false;
               //owning Holder= old; listID= new
               foreach (var item in Holders.ElementAt(card.owningPlayer * 10 + card.owningHolder))
               {
                    if (item.instance == card.instance)
                    {
                         cardFound = true;
                         Holders.ElementAt(card.owningPlayer * 10 + card.owningHolder).Remove(item);
                         //Console.WriteLine("card {0} removed from holder {1} for player {2}", card.template.title, card.owningHolder, card.owningPlayer);
                         break;
                    }
               }
               if (cardFound)
               {
                    //TO DO add logic for spy and other types
                    //TO DO add logic for actual power if any effect
                    card.owningHolder = listID;

                    //reset margin back to zero, it was changed during selected stage
                    Thickness margin = card.cardImage.Margin;
                    margin.Bottom = 0;
                    margin.Top = 0;
                    card.cardImage.Margin = margin;

                    Holders.ElementAt(card.owningPlayer * 10 + listID).Add(card);
                    //Console.WriteLine("card {0} added in holder {1} for player {2}", cardSlot.template.title, cardSlot.owningHolder, cardSlot.owningPlayer);

                    RecalculateScore();
               }
               else
               {
                    throw new ArgumentNullException("not found");
               }
          }

          //this is the entry point for handling transactions
          public void startCardTransaction(CardSlot card)
          {
               SelectedCardSlot = card;
               //TO DO implement logic for
               /**
               if leader
               if has target
               if global effect
               **/
               transferTransactionCardToDestination(card);
          }

          private async void transferTransactionCardToDestination(CardSlot card)
          {
               int destinationList = ValuesRepository.getLocation(card);
               if (destinationList != ValuesRepository.CardType_None)
               {
                    SelectedCardSlot = null;
                    //TO DO add logic for transaction enable
                    card.IsTransactionReady = false;
                    //moveCard(card, card.owningPlayer * 10 + destinationList);
                    moveCard(card, destinationList);
                    //switch turn
                    IsAITurn = !IsAITurn;
                    await ValuesRepository.PutTaskDelay(1000);
               }
               else
               {
                    Console.WriteLine("wrong destination!");
               }
          }

          //AI
          private async void AITurn()
          {
               CurrentState = "AI choosing strategy…";
               await ValuesRepository.PutTaskDelay(2000);

               int t = await Task.Run(() => ChooseAttitude());

               CurrentState = ValuesRepository.attitudeToString(AIAttitude);
               await ValuesRepository.PutTaskDelay(1000);

               CardTransaction decided = await Task.Run(() => DecideCard());
               if (decided == null && AIAttitude != ValuesRepository.TACTIC_PASS)
               {
                    AIAttitude = ValuesRepository.TACTIC_PASS;
               }

               CurrentState = "AI Deciding Hand";
               await ValuesRepository.PutTaskDelay(2000);
               
               //TO DO implement weather and such criteria, besides dummies and spies
               //Also implement attitude if critical round
               if ( decided != null && decided.sourceCard != null)
               {
                    
                    Console.WriteLine("#AI# - AI decided on the following transaction: {0}, {1}", decided.sourceCard.template.index, decided.sourceCard.template.title);
                    //TO DO implement spy and targets

                    SelectedCardSlot = decided.sourceCard;
                    CurrentState = "AI Playing Card";
                    await ValuesRepository.PutTaskDelay(2000);

                    startCardTransaction(decided.sourceCard);
               }
          }

          private int ChooseAttitude()
          {
               //if zero cards in AI hand, don't waste time, and just pass…
               if (P2HandHolder.Count == 0)
               {
                    AIAttitude = ValuesRepository.TACTIC_PASS;
                    return AIAttitude;
               }

               //otherwise do some work :-)
               //initialize dummy and spy potential counters
               int dummies = 0;
               int spies = 0;

               //count the cards in hand, this is for debug, not cheating
               double P2HandCards = Convert.ToDouble(P2HandHolder.Count);
               double P1HandCards = Convert.ToDouble(P1HandHolder.Count);

               double deltaCards = P2HandCards - P1HandCards;
               int deltaScores = currentPlayerScores[ValuesRepository.PLAYER_2] - currentPlayerScores[ValuesRepository.PLAYER_1];

               //check previous round status
               bool P2PreviousWinner = false;
               bool P1PreviousWinner = false;

               int previousWinner = 0;
               int roundResultsCounter = 0;
               while (roundResultsCounter < roundResults.Count)
               {
                    if (roundResults[roundResultsCounter].played)
                    {
                         previousWinner = roundResults[roundResultsCounter].winningPlayer;
                         if (previousWinner == ValuesRepository.PLAYER_2 || previousWinner == ValuesRepository.PLAYER_INVALID)
                         {
                              P2PreviousWinner = true;
                         }
                         if (previousWinner == ValuesRepository.PLAYER_1 || previousWinner == ValuesRepository.PLAYER_INVALID)
                         {
                              P1PreviousWinner = true;
                         }
                    }
                    ++roundResultsCounter;
               }

               //set critical if needed
               currentRoundCritical = P1PreviousWinner;

               //check if opponent active or done
               bool IsP1Active = currentRoundStatus[ValuesRepository.PLAYER_1];
               SafeRandom random = new SafeRandom();

               if (debug)
               {
                    Console.WriteLine("#AI# ###############################################################################");
                    Console.WriteLine("#AI#---------------------------- AI Deciding his next move --------------------------------");
                    Console.WriteLine("#AI#------ previousTactic: " + ValuesRepository.attitudeToString(AIAttitude));
                    Console.WriteLine("#AI#------ playerCardsInHand: " + P2HandCards);
                    Console.WriteLine("#AI#------ opponentCardsInHand: " + P1HandCards);
                    Console.WriteLine("#AI#------ cardAdvantage: " + deltaCards);
                    Console.WriteLine("#AI#------ scoreDifference: " + deltaScores + ", his score: " + currentPlayerScores[ValuesRepository.PLAYER_2] + ", enemy score: " + currentPlayerScores[ValuesRepository.PLAYER_1]);
                    Console.WriteLine("#AI#------ opponent has won: " + P1PreviousWinner);
                    Console.WriteLine("#AI#------ has won: " + P2PreviousWinner);
                    Console.WriteLine("#AI#------ Num units in hand: " + P2HandCards);
                    if (IsP1Active)
                    {
                         Console.WriteLine("#AI#------ has opponent passed: false");
                    }
                    else
                    {
                         Console.WriteLine("#AI#------ has opponent passed: true");
                    }
                    Console.WriteLine("#AI#=======================================================================================");
                    Console.WriteLine("#AI#-----------------------------   AI CARDS AT HAND   ------------------------------------");

                    foreach (CardSlot card in P2HandHolder)
                    {
                         Console.WriteLine("#AI# Points[ " + card.template.power + " ], Card - " + card.template.index + "  " + card.template.title);
                    }
                    Console.WriteLine("#AI#=======================================================================================");
               }

               //check for Nilfgaard faction, for tactic draw that ends in win
               int P1DeckFaction = currentGameDecks[ValuesRepository.PLAYER_1].deckTypeIndex;
               int P2DeckFaction = currentGameDecks[ValuesRepository.PLAYER_2].deckTypeIndex;
               if (P2DeckFaction == ValuesRepository.FactionId_Nilfgaard &&
                    P1DeckFaction != ValuesRepository.FactionId_Nilfgaard &&
                    IsP1Active == false &&
                    deltaScores == 0)
               {
                    AIAttitude = ValuesRepository.TACTIC_PASS;
               }
               //check if player passed and previous attitude
               else if (!P1PreviousWinner && AIAttitude == ValuesRepository.TACTIC_SPY_DUMMY_BEST_THEN_PASS)
               {
                    if (IsP1Active)
                    {
                         AIAttitude = ValuesRepository.TACTIC_SPY_DUMMY_BEST_THEN_PASS;
                    }
               }
               //check AI hand for SPY and the opponent/player board for SPY
               else if (!P1PreviousWinner &&
                         ValuesRepository.playerHandHasEffect(P2HandHolder, ValuesRepository.CardEffect_Draw2) &&
                         (random.NextDouble() < 0.2 || ValuesRepository.playerBoardHasEffect(ValuesRepository.PLAYER_1, ValuesRepository.CardEffect_Draw2)) &&
                         AIAttitude != ValuesRepository.TACTIC_SPY_DUMMY_BEST_THEN_PASS)
               {
                    AIAttitude = ValuesRepository.TACTIC_SPY;
               }
               //if it found spy in AI hand, and previous SPY used, do it again
               else if (AIAttitude == ValuesRepository.TACTIC_SPY &&
                    ValuesRepository.playerHandHasEffect(P2HandHolder, ValuesRepository.CardEffect_Draw2))
               {
                    AIAttitude = ValuesRepository.TACTIC_SPY;
               }
               //if P1/Player is still active and didn't pass or card count 0
               else if (IsP1Active)
               {
                    if (deltaScores > 0)
                    {
                         if (P1PreviousWinner)
                         {
                              AIAttitude = ValuesRepository.TACTIC_JUST_WAIT;
                         }
                         else
                         {
                              AIAttitude = ValuesRepository.TACTIC_NONE;
                              if (P2PreviousWinner)
                              {
                                   dummies = ValuesRepository.getCardsInHandWithEffect(P2HandHolder, ValuesRepository.CardEffect_UnsummonDummy);
                                   spies = ValuesRepository.getCardsInHandWithEffect(P2HandHolder, ValuesRepository.CardEffect_Draw2);
                                   if (random.NextDouble() < 0.2 || P2HandCards == dummies + spies)
                                   {
                                        AIAttitude = ValuesRepository.TACTIC_SPY_DUMMY_BEST_THEN_PASS;
                                   }
                                   else
                                   {
                                        if (dummies > 0 && ValuesRepository.playerBoardCreatures(ValuesRepository.PLAYER_2).Count > 0 )
                                        {
                                             AIAttitude = ValuesRepository.TACTIC_WAIT_DUMMY;
                                        }
                                        else if (random.NextDouble() < deltaScores / 30 && random.NextDouble() < (P2HandHolder.Count * P2HandHolder.Count) / 36)
                                        {
                                             AIAttitude = ValuesRepository.TACTIC_MAXIMIZE_WIN;
                                        }
                                   }
                              }
                              if (AIAttitude == ValuesRepository.TACTIC_NONE)
                              {
                                   if (random.NextDouble() < P2HandCards / 10 || P2HandCards > 8)
                                   {
                                        if (random.NextDouble() < 0.2 || P2HandCards == dummies + spies)
                                        {
                                             AIAttitude = ValuesRepository.TACTIC_SPY_DUMMY_BEST_THEN_PASS;
                                        }
                                        else
                                        {
                                             AIAttitude = ValuesRepository.TACTIC_JUST_WAIT;
                                        }
                                   }
                                   else
                                   {
                                        AIAttitude = ValuesRepository.TACTIC_PASS;
                                   }
                              }
                         }
                    }
                    else if (P2PreviousWinner)
                    {
                         dummies = ValuesRepository.getCardsInHandWithEffect(P2HandHolder, ValuesRepository.CardEffect_UnsummonDummy);
                         spies = ValuesRepository.getCardsInHandWithEffect(P2HandHolder, ValuesRepository.CardEffect_Draw2);
                         if (!P1PreviousWinner && (random.NextDouble() < 0.2 || P2HandCards == dummies + spies))
                         {
                              AIAttitude = ValuesRepository.TACTIC_SPY_DUMMY_BEST_THEN_PASS;
                         }
                         else
                         {
                              AIAttitude = ValuesRepository.TACTIC_MAXIMIZE_WIN;
                         }
                    }
                    else if (P1PreviousWinner)
                    {
                         AIAttitude = ValuesRepository.TACTIC_MINIMAL_WIN;
                    }
                    else if (!roundResults[0].played && deltaScores < -11 && 
                         random.NextDouble() < (Math.Abs(deltaScores) - 10) / 20)
                    {
                         if (random.NextDouble() < 0.9)
                         {
                              AIAttitude = ValuesRepository.TACTIC_SPY_DUMMY_BEST_THEN_PASS;
                         }
                         else
                         {
                              AIAttitude = ValuesRepository.TACTIC_PASS;
                         }
                    }
                    else if (random.NextDouble() < P2HandCards / 10)
                    {
                         AIAttitude = ValuesRepository.TACTIC_MINIMAL_WIN;
                    }
                    else if (random.NextDouble() < P2HandCards / 10)
                    {
                         AIAttitude = ValuesRepository.TACTIC_AVERAGE_WIN;
                    }
                    else if (random.NextDouble() < P2HandCards / 10)
                    {
                         AIAttitude = ValuesRepository.TACTIC_MAXIMIZE_WIN;
                    }
                    else if (P2HandCards <= 8 && 
                         random.NextDouble() > P2HandCards / 10)
                    {
                         AIAttitude = ValuesRepository.TACTIC_PASS;
                    }
                    else
                    {
                         AIAttitude = ValuesRepository.TACTIC_JUST_WAIT;
                    }
               }
               else if (AIAttitude != ValuesRepository.TACTIC_MINIMIZE_LOSS)
               {
                    if (!P1PreviousWinner && deltaScores <= 0 && 
                         random.NextDouble() < deltaScores / 20)
                    {
                         AIAttitude = ValuesRepository.TACTIC_MINIMIZE_LOSS;
                    }
                    else if (!P2PreviousWinner && deltaScores > 0)
                    {
                         AIAttitude = ValuesRepository.TACTIC_MINIMIZE_WIN;
                    }
                    else if (deltaScores > 0)
                    {
                         AIAttitude = ValuesRepository.TACTIC_PASS;
                    }
                    else
                    {
                         AIAttitude = ValuesRepository.TACTIC_MINIMAL_WIN;
                    }
               }
               else
               {
                    AIAttitude = ValuesRepository.TACTIC_MINIMIZE_LOSS;
               }

               return AIAttitude;
          }

          private CardTransaction DecideCard()
          {
               //unless noted otherwise, any sorting is done considering effects applied, not the default template power
               //IE: power considers effect/s, template.power considers the default value
               CardTransaction result = new CardTransaction();
               int deltaScores = currentPlayerScores[ValuesRepository.PLAYER_2] - currentPlayerScores[ValuesRepository.PLAYER_1];
               SafeRandom random = new SafeRandom();

               switch (AIAttitude)
               {
                    case ValuesRepository.TACTIC_SPY_DUMMY_BEST_THEN_PASS:
                         {
                              //get SPY/SPIES if any
                              List<CardSlot> spies = new List<CardSlot>();
                              foreach (CardSlot card in P2HandHolder)
                              {
                                   if (ValuesRepository.cardHasEffect(card, ValuesRepository.CardEffect_Draw2))
                                   {
                                        spies.Add(card);
                                   }
                              }

                              //Sort ascending the list if multiple spies, and pick hero or smallest?
                              if (spies.Count > 0)
                              {
                                   List<CardSlot> SortedSpies = spies.OrderBy(o => o.power).ToList();
                                   result.sourceCard = SortedSpies.ElementAt(0);
                                   return result;
                              }

                              //get DUMMY/DUMMIES if any
                              List<CardSlot> dummies = new List<CardSlot>();
                              foreach (CardSlot card in P2HandHolder)
                              {
                                   if (ValuesRepository.cardHasEffect(card, ValuesRepository.CardEffect_UnsummonDummy))
                                   {
                                        dummies.Add(card);
                                   }
                              }

                              if (dummies.Count > 0)//no need to sort
                              {
                                   result.sourceCard = dummies.ElementAt(0);
                                   //sort descending the creatures on the AI board, and get the highest
                                   List<CardSlot> AIBoard = ValuesRepository.playerBoardCreatures(ValuesRepository.PLAYER_2);
                                   if (AIBoard.Count > 0)
                                   {
                                        CardSlot target = AIBoard.OrderByDescending(o => o.power).ToList().ElementAt(0);
                                        result.targetCard = target;
                                   }
                                   
                                   return result;
                              }

                              //if for some reason spies and dummies failed
                              AIAttitude = ValuesRepository.TACTIC_PASS;
                              break;
                         }
                    case ValuesRepository.TACTIC_MINIMIZE_LOSS:
                         {
                              //get DUMMY/DUMMIES if any
                              List<CardSlot> dummies = new List<CardSlot>();
                              foreach (CardSlot card in P2HandHolder)
                              {
                                   if (ValuesRepository.cardHasEffect(card, ValuesRepository.CardEffect_UnsummonDummy))
                                   {
                                        dummies.Add(card);
                                   }
                              }

                              if (dummies.Count > 0)//no need to sort
                              {
                                   result.sourceCard = dummies.ElementAt(0);
                                   //sort descending the creatures on the AI board, and get the highest
                                   List<CardSlot> AIBoard = ValuesRepository.playerBoardCreatures(ValuesRepository.PLAYER_2);
                                   if (AIBoard.Count > 0)
                                   {
                                        CardSlot target = AIBoard.OrderByDescending(o => o.power).ToList().ElementAt(0);
                                        result.targetCard = target;
                                   }

                                   return result;
                              }

                              //get SPY/SPIES if any
                              List<CardSlot> spies = new List<CardSlot>();
                              foreach (CardSlot card in P2HandHolder)
                              {
                                   if (ValuesRepository.cardHasEffect(card, ValuesRepository.CardEffect_Draw2))
                                   {
                                        spies.Add(card);
                                   }
                              }

                              //Sort ascending the list if multiple spies, and pick hero or smallest?
                              if (spies.Count > 0)
                              {
                                   List<CardSlot> SortedSpies = spies.OrderBy(o => o.power).ToList();
                                   result.sourceCard = SortedSpies.ElementAt(0);
                                   return result;
                              }

                              //if for some reason spies and dummies failed
                              AIAttitude = ValuesRepository.TACTIC_PASS;
                              break;
                         }
                    case ValuesRepository.TACTIC_MINIMIZE_WIN:
                         {
                              //get DUMMY/DUMMIES if any
                              List<CardSlot> dummies = new List<CardSlot>();
                              foreach (CardSlot card in P2HandHolder)
                              {
                                   if (ValuesRepository.cardHasEffect(card, ValuesRepository.CardEffect_UnsummonDummy))
                                   {
                                        dummies.Add(card);
                                   }
                              }

                              if (dummies.Count > 0)//no need to sort
                              {
                                   result.sourceCard = dummies.ElementAt(0);
                                   //sort ascending the creatures on the AI board, and get the the first bigger than deltaScores
                                   List<CardSlot> AIBoard = ValuesRepository.playerBoardCreatures(ValuesRepository.PLAYER_2);
                                   if (AIBoard.Count > 0)
                                   {
                                        List<CardSlot> creatures = AIBoard.OrderBy(o => o.power).ToList();
                                        foreach (CardSlot creature in creatures)
                                        {
                                             if (creature.power > deltaScores)
                                             {
                                                  result.targetCard = creature;
                                                  return result;
                                             }
                                        }
                                   }
                              }

                              //get SPY/SPIES if any
                              List<CardSlot> spies = new List<CardSlot>();
                              foreach (CardSlot card in P2HandHolder)
                              {
                                   if (ValuesRepository.cardHasEffect(card, ValuesRepository.CardEffect_Draw2))
                                   {
                                        spies.Add(card);
                                   }
                              }

                              //Sort ascending the list if multiple spies, and pick hero or smallest?
                              if (spies.Count > 0)
                              {
                                   List<CardSlot> SortedSpies = spies.OrderBy(o => o.power).ToList();
                                   foreach (CardSlot spy in SortedSpies)
                                   {
                                        if (ValuesRepository.getPowerChange(spy) < Math.Abs(deltaScores))
                                        {
                                             result.sourceCard = spy;
                                             return result;
                                        }
                                   }
                              }

                              //if for some reason spies and dummies failed
                              AIAttitude = ValuesRepository.TACTIC_PASS;
                              break;
                         }
                    case ValuesRepository.TACTIC_MAXIMIZE_WIN:
                         {
                              List<CardSlot> hand = new List<CardSlot>();
                              foreach (CardSlot card in P2HandHolder)
                              {
                                   CardSlot temp = new CardSlot();
                                   temp.instance = card.instance;//to match GUID
                                   hand.Add(temp);
                              }

                              foreach (CardSlot temp in hand)
                              {
                                   temp.power = ValuesRepository.getPowerChange(temp);
                              }

                              List<CardSlot> SortedHand = hand.OrderByDescending(o => o.power).ToList();
                              if (SortedHand.Count > 0)
                              {
                                   foreach (CardSlot card in P2HandHolder)
                                   {
                                        if (card.instance == SortedHand.ElementAt(0).instance)
                                        {
                                             result.sourceCard = card;//get highest
                                             return result;
                                        }
                                   }
                              }
                              throw new ArgumentNullException("didn't find actual card with the same instance as temp");
                              //break;
                         }
                    case ValuesRepository.TACTIC_AVERAGE_WIN:
                         {
                              List<CardSlot> hand = new List<CardSlot>();
                              foreach (CardSlot card in P2HandHolder)
                              {
                                   CardSlot temp = new CardSlot();
                                   temp.instance = card.instance;//to match GUID
                                   hand.Add(temp);
                              }

                              foreach (CardSlot temp in hand)
                              {
                                   temp.power = ValuesRepository.getPowerChange(temp);
                              }

                              List<CardSlot> SortedHand = hand.OrderBy(o => o.power).ToList();
                              List<CardSlot> Average = new List<CardSlot>();
                              foreach (CardSlot c in SortedHand)
                              {
                                   if (c.power > Math.Abs(deltaScores))
                                   {
                                        Average.Add(c);
                                   }
                              }
                              if (Average.Count > 0)
                              {
                                   foreach (CardSlot card in P2HandHolder)
                                   {
                                        if (card.instance == Average.ElementAt(random.Next(1, Average.Count)).instance)
                                        {
                                             result.sourceCard = card;
                                             return result;
                                        }
                                   }
                              }
                              throw new ArgumentNullException("didn't find actual card with the same instance as temp");
                              //break;
                         }
                    case ValuesRepository.TACTIC_MINIMAL_WIN:
                         {
                              List<CardSlot> hand = new List<CardSlot>();
                              foreach (CardSlot card in P2HandHolder)
                              {
                                   CardSlot temp = new CardSlot();
                                   temp.instance = card.instance;//to match GUID
                                   temp.template = card.template;
                                   temp.owningPlayer = ValuesRepository.PLAYER_2;
                                   hand.Add(temp);
                              }

                              foreach (CardSlot temp in hand)
                              {
                                   temp.power = ValuesRepository.getPowerChange(temp);
                              }

                              List<CardSlot> SortedHand = hand.OrderBy(o => o.power).ToList();
                              List<CardSlot> Average = new List<CardSlot>();
                              foreach (CardSlot c in SortedHand)
                              {
                                   if (c.power > Math.Abs(deltaScores))
                                   {
                                        Average.Add(c);
                                   }
                              }
                              if (Average.Count > 0)
                              {
                                   foreach (CardSlot card in P2HandHolder)
                                   {
                                        if (card.instance == Average.ElementAt(0).instance)
                                        {
                                             result.sourceCard = card;//pick the first bigger than Delta score
                                             return result;
                                        }
                                   }
                              }
                              throw new ArgumentNullException("didn't find actual card with the same instance as temp");
                              //break;
                         }
                    case ValuesRepository.TACTIC_JUST_WAIT:
                         {
                              if (P2HandHolder.Count == 0)
                              {
                                   return null;
                              }
                              throw new ArgumentException("tactic just wait was chosen, why?");
                              //return result;
                         }
                    case ValuesRepository.TACTIC_WAIT_DUMMY:
                         {
                              //get DUMMY/DUMMIES if any
                              List<CardSlot> dummies = new List<CardSlot>();
                              foreach (CardSlot card in P2HandHolder)
                              {
                                   if (ValuesRepository.cardHasEffect(card, ValuesRepository.CardEffect_UnsummonDummy))
                                   {
                                        dummies.Add(card);
                                   }
                              }

                              if (dummies.Count > 0)//no need to sort
                              {
                                   result.sourceCard = dummies.ElementAt(0);
                                   //sort descending the creatures on the AI board, and get the highest
                                   List<CardSlot> AIBoard = ValuesRepository.playerBoardCreatures(ValuesRepository.PLAYER_2);
                                   if (AIBoard.Count > 0)
                                   {
                                        CardSlot target = AIBoard.OrderByDescending(o => o.power).ToList().ElementAt(0);
                                        result.targetCard = target;
                                   }

                                   return result;
                              }

                              Console.WriteLine("tactic wait dummy, but there is no dummy, huh?");
                              break;
                         }
                    case ValuesRepository.TACTIC_SPY:
                         {
                              //get SPY/SPIES if any
                              List<CardSlot> spies = new List<CardSlot>();
                              foreach (CardSlot card in P2HandHolder)
                              {
                                   if (ValuesRepository.cardHasEffect(card, ValuesRepository.CardEffect_Draw2))
                                   {
                                        spies.Add(card);
                                   }
                              }

                              //Sort ascending the list if multiple spies, and pick hero or smallest?
                              if (spies.Count > 0)
                              {
                                   List<CardSlot> SortedSpies = spies.OrderBy(o => o.power).ToList();
                                   result.sourceCard = SortedSpies.ElementAt(0);
                                   return result;
                              }

                              break;
                         }
               }
               //Outside the switch, try and use a spy
               if (AIAttitude != ValuesRepository.TACTIC_PASS &&
                    AIAttitude != ValuesRepository.TACTIC_MINIMIZE_WIN)
               {
                    //get SPY/SPIES if any
                    List<CardSlot> spies = new List<CardSlot>();
                    foreach (CardSlot card in P2HandHolder)
                    {
                         if (ValuesRepository.cardHasEffect(card, ValuesRepository.CardEffect_Draw2))
                         {
                              spies.Add(card);
                         }
                    }

                    //Sort ascending the list if multiple spies, and pick hero or smallest?
                    if (spies.Count > 0)
                    {
                         List<CardSlot> SortedSpies = spies.OrderBy(o => o.power).ToList();
                         result.sourceCard = SortedSpies.ElementAt(0);
                         return result;
                    }
               }

               return null;
          }

          private void RecalculateScore()
          {
               P1MeleeScore = 0;
               P1RangeScore = 0;
               P1SiegeScore = 0;

               if (P1MeleeHolder != null && P1MeleeHolder.Count > 0)
               {
                    foreach (CardSlot card in P1MeleeHolder)
                    {
                         P1MeleeScore += card.power;
                    }
               }
               if (P1RangeHolder != null && P1RangeHolder.Count > 0)
               {
                    foreach (CardSlot card in P1RangeHolder)
                    {
                         P1RangeScore += card.power;
                    }
               }
               if (P1SiegeHolder != null && P1SiegeHolder.Count > 0)
               {
                    foreach (CardSlot card in P1SiegeHolder)
                    {
                         P1SiegeScore += card.power;
                    }
               }

               P2MeleeScore = 0;
               P2RangeScore = 0;
               P2SiegeScore = 0;

               if (P2MeleeHolder != null && P2MeleeHolder.Count > 0)
               {
                    foreach (CardSlot card in P2MeleeHolder)
                    {
                         P2MeleeScore += card.power;
                    }
               }
               if (P2RangeHolder != null && P2RangeHolder.Count > 0)
               {
                    foreach (CardSlot card in P2RangeHolder)
                    {
                         P2RangeScore += card.power;
                    }
               }
               if (P2SiegeHolder != null && P2SiegeHolder.Count > 0)
               {
                    foreach (CardSlot card in P2SiegeHolder)
                    {
                         P2SiegeScore += card.power;
                    }
               }

               P1Score = P1MeleeScore + P1RangeScore + P1SiegeScore;
               P2Score = P2MeleeScore + P2RangeScore + P2SiegeScore;

               currentPlayerScores[ValuesRepository.PLAYER_1] = P1Score;
               currentPlayerScores[ValuesRepository.PLAYER_2] = P2Score;
          }

          //sidekick
          public CardSlot SelectedCardSlot
          {
               get { return _SelectedCardSlotLocator(this).Value; }
               set
               {
                    _SelectedCardSlotLocator(this).SetValueAndTryNotify(value);
                    if (value != null)
                    {
                         if (value.owningPlayer == ValuesRepository.PLAYER_1 && value.owningHolder == ValuesRepository.CARD_LIST_LOC_HAND)
                         {
                              foreach (CardSlot card in P1HandHolder)
                              {
                                   Thickness margin = card.cardImage.Margin;
                                   //Console.WriteLine("{0}, {1}", card.template.title, card.instance);
                                   if (card == value && card.instance == value.instance)
                                   {
                                        margin.Bottom = 10;
                                        margin.Top = -10;
                                        card.cardImage.Margin = margin;
                                   }
                                   else
                                   {
                                        margin.Bottom = 0;
                                        margin.Top = 0;
                                        card.cardImage.Margin = margin;
                                   }
                              }
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

          public int P1Score
          {
               get { return _P1ScoreLocator(this).Value; }
               set { _P1ScoreLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property int P1Score Setup
          protected Property<int> _P1Score = new Property<int> { LocatorFunc = _P1ScoreLocator };
          static Func<BindableBase, ValueContainer<int>> _P1ScoreLocator = RegisterContainerLocator<int>("P1Score", model => model.Initialize("P1Score", ref model._P1Score, ref _P1ScoreLocator, _P1ScoreDefaultValueFactory));
          static Func<int> _P1ScoreDefaultValueFactory = null;
          #endregion

          public int P1MeleeScore
          {
               get { return _P1MeleeScoreLocator(this).Value; }
               set { _P1MeleeScoreLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property int P1MeleeScore Setup
          protected Property<int> _P1MeleeScore = new Property<int> { LocatorFunc = _P1MeleeScoreLocator };
          static Func<BindableBase, ValueContainer<int>> _P1MeleeScoreLocator = RegisterContainerLocator<int>("P1MeleeScore", model => model.Initialize("P1MeleeScore", ref model._P1MeleeScore, ref _P1MeleeScoreLocator, _P1MeleeScoreDefaultValueFactory));
          static Func<int> _P1MeleeScoreDefaultValueFactory = null;
          #endregion

          public int P1RangeScore
          {
               get { return _P1RangeScoreLocator(this).Value; }
               set { _P1RangeScoreLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property int P1RangeScore Setup
          protected Property<int> _P1RangeScore = new Property<int> { LocatorFunc = _P1RangeScoreLocator };
          static Func<BindableBase, ValueContainer<int>> _P1RangeScoreLocator = RegisterContainerLocator<int>("P1RangeScore", model => model.Initialize("P1RangeScore", ref model._P1RangeScore, ref _P1RangeScoreLocator, _P1RangeScoreDefaultValueFactory));
          static Func<int> _P1RangeScoreDefaultValueFactory = null;
          #endregion

          public int P1SiegeScore
          {
               get { return _P1SiegeScoreLocator(this).Value; }
               set { _P1SiegeScoreLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property int P1SiegeScore Setup
          protected Property<int> _P1SiegeScore = new Property<int> { LocatorFunc = _P1SiegeScoreLocator };
          static Func<BindableBase, ValueContainer<int>> _P1SiegeScoreLocator = RegisterContainerLocator<int>("P1SiegeScore", model => model.Initialize("P1SiegeScore", ref model._P1SiegeScore, ref _P1SiegeScoreLocator, _P1SiegeScoreDefaultValueFactory));
          static Func<int> _P1SiegeScoreDefaultValueFactory = null;
          #endregion

          public int P2Score
          {
               get { return _P2ScoreLocator(this).Value; }
               set { _P2ScoreLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property int P2Score Setup
          protected Property<int> _P2Score = new Property<int> { LocatorFunc = _P2ScoreLocator };
          static Func<BindableBase, ValueContainer<int>> _P2ScoreLocator = RegisterContainerLocator<int>("P2Score", model => model.Initialize("P2Score", ref model._P2Score, ref _P2ScoreLocator, _P2ScoreDefaultValueFactory));
          static Func<int> _P2ScoreDefaultValueFactory = null;
          #endregion

          public int P2MeleeScore
          {
               get { return _P2MeleeScoreLocator(this).Value; }
               set { _P2MeleeScoreLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property int P2MeleeScore Setup
          protected Property<int> _P2MeleeScore = new Property<int> { LocatorFunc = _P2MeleeScoreLocator };
          static Func<BindableBase, ValueContainer<int>> _P2MeleeScoreLocator = RegisterContainerLocator<int>("P2MeleeScore", model => model.Initialize("P2MeleeScore", ref model._P2MeleeScore, ref _P2MeleeScoreLocator, _P2MeleeScoreDefaultValueFactory));
          static Func<int> _P2MeleeScoreDefaultValueFactory = null;
          #endregion

          public int P2RangeScore
          {
               get { return _P2RangeScoreLocator(this).Value; }
               set { _P2RangeScoreLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property int P2RangeScore Setup
          protected Property<int> _P2RangeScore = new Property<int> { LocatorFunc = _P2RangeScoreLocator };
          static Func<BindableBase, ValueContainer<int>> _P2RangeScoreLocator = RegisterContainerLocator<int>("P2RangeScore", model => model.Initialize("P2RangeScore", ref model._P2RangeScore, ref _P2RangeScoreLocator, _P2RangeScoreDefaultValueFactory));
          static Func<int> _P2RangeScoreDefaultValueFactory = null;
          #endregion

          public int P2SiegeScore
          {
               get { return _P2SiegeScoreLocator(this).Value; }
               set { _P2SiegeScoreLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property int P2SiegeScore Setup
          protected Property<int> _P2SiegeScore = new Property<int> { LocatorFunc = _P2SiegeScoreLocator };
          static Func<BindableBase, ValueContainer<int>> _P2SiegeScoreLocator = RegisterContainerLocator<int>("P2SiegeScore", model => model.Initialize("P2SiegeScore", ref model._P2SiegeScore, ref _P2SiegeScoreLocator, _P2SiegeScoreDefaultValueFactory));
          static Func<int> _P2SiegeScoreDefaultValueFactory = null;
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

          public String CurrentState
          {
               get { return _CurrentStateLocator(this).Value; }
               set { _CurrentStateLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property String CurrentState Setup
          protected Property<String> _CurrentState = new Property<String> { LocatorFunc = _CurrentStateLocator };
          static Func<BindableBase, ValueContainer<String>> _CurrentStateLocator = RegisterContainerLocator<String>("CurrentState", model => model.Initialize("CurrentState", ref model._CurrentState, ref _CurrentStateLocator, _CurrentStateDefaultValueFactory));
          static Func<String> _CurrentStateDefaultValueFactory = null;
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

          public bool currentRoundCritical
          {
               get { return _currentRoundCriticalLocator(this).Value; }
               set { _currentRoundCriticalLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property bool currentRoundCritical Setup
          protected Property<bool> _currentRoundCritical = new Property<bool> { LocatorFunc = _currentRoundCriticalLocator };
          static Func<BindableBase, ValueContainer<bool>> _currentRoundCriticalLocator = RegisterContainerLocator<bool>("currentRoundCritical", model => model.Initialize("currentRoundCritical", ref model._currentRoundCritical, ref _currentRoundCriticalLocator, _currentRoundCriticalDefaultValueFactory));
          static Func<bool> _currentRoundCriticalDefaultValueFactory = null;
          #endregion

          public bool IsAITurn
          {
               get { return _IsAITurnLocator(this).Value; }
               set
               {
                    _IsAITurnLocator(this).SetValueAndTryNotify(value);
                    if (value == true)//disabled P1 hand
                    {
                         foreach (CardSlot card in P1HandHolder)
                         {
                              card.IsTransactionReady = false;
                         }
                         CurrentState = "P2 Turn";
                         AITurn();
                         //TO DO implement AI
                    }
                    else
                    {
                         CurrentState = "P1 Turn";
                         //reenable P1 Hand
                         if (P1HandHolder != null && P1HandHolder.Count > 0)
                         {
                              foreach (CardSlot card in P1HandHolder)
                              {
                                   card.IsTransactionReady = true;
                              }
                         }
                         //TO DO implement AI
                    }
               }
          }

          #region Property bool IsAITurn Setup
          protected Property<bool> _IsAITurn = new Property<bool> { LocatorFunc = _IsAITurnLocator };
          static Func<BindableBase, ValueContainer<bool>> _IsAITurnLocator = RegisterContainerLocator<bool>("IsAITurn", model => model.Initialize("IsAITurn", ref model._IsAITurn, ref _IsAITurnLocator, _IsAITurnDefaultValueFactory));
          static Func<bool> _IsAITurnDefaultValueFactory = null;
          #endregion

          public int AIAttitude
          {
               get { return _AIAttitudeLocator(this).Value; }
               set { _AIAttitudeLocator(this).SetValueAndTryNotify(value); }
          }

          #region Property int AIAttitude Setup
          protected Property<int> _AIAttitude = new Property<int> { LocatorFunc = _AIAttitudeLocator };
          static Func<BindableBase, ValueContainer<int>> _AIAttitudeLocator = RegisterContainerLocator<int>("AIAttitude", model => model.Initialize("AIAttitude", ref model._AIAttitude, ref _AIAttitudeLocator, _AIAttitudeDefaultValueFactory));
          static Func<int> _AIAttitudeDefaultValueFactory = null;
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

          //Utilities
          public string GetResourceTextFile(string filename)
          {
               string result = string.Empty;

               using (Stream stream = this.GetType().Assembly.
                    GetManifestResourceStream("Gwent.XML." + filename))
               //GetManifestResourceStream("assembly.folder." + filename))
               {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                         result = sr.ReadToEnd();
                    }
               }
               return result;
          }
     }
}

