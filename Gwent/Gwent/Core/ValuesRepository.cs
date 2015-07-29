using Gwent.Models;
using Gwent.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Core
{
     public static class ValuesRepository
     {
          public const int TEMPLATE_INVALID = -1;

          public const int PLAYER_INVALID = -1;
          public const int PLAYER_1 = 0;
          public const int PLAYER_2 = 1;
          public const int PLAYER_BOTH = 2;

          public const int CARD_LIST_LOC_INVALID = -1;
          public const int CARD_LIST_LOC_WEATHERSLOT = 0;
          public const int CARD_LIST_LOC_LEADER = 1;
          public const int CARD_LIST_LOC_DECK = 2;
          public const int CARD_LIST_LOC_HAND = 3;
          public const int CARD_LIST_LOC_GRAVEYARD = 4;
          public const int CARD_LIST_LOC_SEIGE = 5;
          public const int CARD_LIST_LOC_RANGED = 6;
          public const int CARD_LIST_LOC_MELEE = 7;
          public const int CARD_LIST_LOC_SEIGEMODIFIERS = 8;
          public const int CARD_LIST_LOC_RANGEDMODIFIERS = 9;
          public const int CARD_LIST_LOC_MELEEMODIFIERS = 10;

          public const int CardType_None = 0;
          public const int CardType_Melee = 1;
          public const int CardType_Ranged = 2;
          public const int CardType_RangedMelee = 3;
          public const int CardType_Siege = 4;
          public const int CardType_SeigeRangedMelee = 7;
          public const int CardType_Creature = 8;
          public const int CardType_Weather = 16;
          public const int CardType_Spell = 32;
          public const int CardType_Row_Modifier = 64;
          public const int CardType_Hero = 128;
          public const int CardType_Spy = 256;
          public const int CardType_Friendly_Effect = 512;
          public const int CardType_Offensive_Effect = 1024;
          public const int CardType_Global_Effect = 2048;

          public const int CardEffect_None = 0;
          public const int CardEffect_Backstab = 1;
          public const int CardEffect_Morale_Boost = 2;
          public const int CardEffect_Ambush = 3;
          public const int CardEffect_ToughSkin = 4;
          public const int CardEffect_Bin2 = 5;
          public const int CardEffect_Bin3 = 6;
          public const int CardEffect_MeleeScorch = 7;
          public const int CardEffect_11th_card = 8;
          public const int CardEffect_Clear_Weather = 9;
          public const int CardEffect_Pick_Weather = 10;
          public const int CardEffect_Pick_Rain = 11;
          public const int CardEffect_Pick_Fog = 12;
          public const int CardEffect_Pick_Frost = 13;
          public const int CardEffect_View_3_Enemy = 14;
          public const int CardEffect_Resurect = 15;
          public const int CardEffect_Resurect_Enemy = 16;
          public const int CardEffect_Bin2_Pick1 = 17;
          public const int CardEffect_Melee_Horn = 18;
          public const int CardEffect_Range_Horn = 19;
          public const int CardEffect_Siege_Horn = 20;
          public const int CardEffect_Siege_Scorch = 21;
          public const int CardEffect_Counter_King = 22;
          public const int CardEffect_Melee = 23;
          public const int CardEffect_Ranged = 24;
          public const int CardEffect_Siege = 25;
          public const int CardEffect_UnsummonDummy = 26;
          public const int CardEffect_Horn = 27;
          public const int CardEffect_Draw = 28;
          public const int CardEffect_Scorch = 29;
          public const int CardEffect_ClearSky = 30;
          public const int CardEffect_SummonClones = 31;
          public const int CardEffect_ImproveNeighbours = 32;
          public const int CardEffect_Nurse = 33;
          public const int CardEffect_Draw2 = 34;
          public const int CardEffect_SameTypeMorale = 35;

          public const int FactionId_Error = -1;
          public const int FactionId_Neutral = 0;
          public const int FactionId_Northern_Kingdom = 1;
          public const int FactionId_Nilfgaard = 2;
          public const int FactionId_Scoiatael = 3;
          public const int FactionId_No_Mans_Land = 4;

          public const int TACTIC_NONE = 0;
          public const int TACTIC_MINIMIZE_LOSS = 1;
          public const int TACTIC_MINIMIZE_WIN = 2;
          public const int TACTIC_MAXIMIZE_WIN = 3;
          public const int TACTIC_AVERAGE_WIN = 4;
          public const int TACTIC_MINIMAL_WIN = 5;
          public const int TACTIC_JUST_WAIT = 6;
          public const int TACTIC_PASS = 7;
          public const int TACTIC_WAIT_DUMMY = 8;
          public const int TACTIC_SPY = 9;
          public const int TACTIC_SPY_DUMMY_BEST_THEN_PASS = 10;

          public static async Task PutTaskDelay(int waitTime)
          {
               await Task.Delay(waitTime);
          }

          public static string getFactionString(int factionIdx)
          {
               switch (factionIdx)
               {
                    case FactionId_Neutral:
                         {
                              return "Neutral";
                         }
                    case FactionId_No_Mans_Land:
                         {
                              return "NoMansLand";
                         }
                    case FactionId_Nilfgaard:
                         {
                              return "Nilfgaard";
                         }
                    case FactionId_Northern_Kingdom:
                         {
                              return "NorthKingdom";
                         }
                    case FactionId_Scoiatael:
                         {
                              return "Scoiatael";
                         }
               }
               return "None";
          }

          public static int typeStringToInt(string typeString)
          {
               switch (typeString)
               {
                    case "TYPE_MELEE":
                         {
                              return CardType_Melee;
                         }
                    case "TYPE_RANGED":
                         {
                              return CardType_Ranged;
                         }
                    case "TYPE_SIEGE":
                         {
                              return CardType_Siege;
                         }
                    case "TYPE_CREATURE":
                         {
                              return CardType_Creature;
                         }
                    case "TYPE_WEATHER"://fog, frost, rain, clear
                         {
                              return CardType_Weather;
                         }
                    case "TYPE_SPELL"://dummy
                         {
                              return CardType_Spell;
                         }
                    case "TYPE_ROW_MODIFIER":
                         {
                              return CardType_Row_Modifier;
                         }
                    case "TYPE_HERO":
                         {
                              return CardType_Hero;
                         }
                    case "TYPE_SPY":
                         {
                              return CardType_Spy;
                         }
                    case "TYPE_FRIENDLY_EFFECT"://dummy
                         {
                              return CardType_Friendly_Effect;
                         }
                    case "TYPE_GLOBAL_EFFECT"://scorch
                         {
                              return CardType_Global_Effect;
                         }
                    default:
                         {
                              //return CardType_None;
                              throw new ArgumentException("needs a string that has a return type!");
                         }
               }
          }

          public static int factionStringToInt(string factionString)
          {
               switch (factionString)
               {
                    case "F_NEUTRAL":
                         {
                              return FactionId_Neutral;
                         }
                    case "F_NO_MANS_LAND":
                         {
                              return FactionId_No_Mans_Land;
                         }
                    case "F_NILFGAARD":
                         {
                              return FactionId_Nilfgaard;
                         }
                    case "F_NORTHERN_KINGDOM":
                         {
                              return FactionId_Northern_Kingdom;
                         }
                    case "F_SCOIATAEL":
                         {
                              return FactionId_Scoiatael;
                         }
                    default:
                         {
                              //return FactionId_Error;
                              throw new ArgumentException("needs a string that has a return faction!");
                         }
               }
          }

          public static int effectStringToInt(string effectString)
          {
               switch (effectString)
               {
                    //regular effects
                    case "EFFECT_NONE":
                         {
                              return CardEffect_None;
                         }
                    case "EFFECT_MELEE":
                         {
                              return CardEffect_Melee;
                         }
                    case "EFFECT_RANGED":
                         {
                              return CardEffect_Ranged;
                         }
                    case "EFFECT_SIEGE":
                         {
                              return CardEffect_Siege;
                         }
                    case "EFFECT_UNSUMMON_DUMMY":
                         {
                              return CardEffect_UnsummonDummy;
                         }
                    case "EFFECT_HORN":
                         {
                              return CardEffect_Horn;
                         }
                    case "EFFECT_SCORCH":
                         {
                              return CardEffect_Scorch;
                         }
                    case "EFFECT_CLEAR_SKY":
                         {
                              return CardEffect_ClearSky;
                         }
                    case "EFFECT_SUMMON_CLONES":
                         {
                              return CardEffect_SummonClones;
                         }
                    case "EFFECT_IMPROVE_NEIGHBOURS":
                         {
                              return CardEffect_ImproveNeighbours;
                         }
                    case "EFFECT_NURSE":
                         {
                              return CardEffect_Nurse;
                         }
                    case "EFFECT_DRAW_X2":
                         {
                              return CardEffect_Draw2;
                         }
                    case "EFFECT_SAME_TYPE_MORALE":
                         {
                              return CardEffect_SameTypeMorale;
                         }
                    //leader effects
                    case "CP_MELEE_SCORCH"://this was actually in the regular cards definitions
                         {
                              return CardEffect_MeleeScorch;
                         }
                    case "CP_PICK_FOG_CARD":
                         {
                              return CardEffect_Pick_Fog;
                         }
                    case "CP_CLEAR_WEATHER":
                         {
                              return CardEffect_Clear_Weather;
                         }
                    case "CP_11TH_CARD":
                         {
                              return CardEffect_11th_card;
                         }
                    case "CP_PICK_WEATHER_CARD":
                         {
                              return CardEffect_Pick_Weather;
                         }
                    case "CP_PICK_RAIN_CARD":
                         {
                              return CardEffect_Pick_Rain;
                         }
                    case "CP_PICK_FROST_CARD":
                         {
                              return CardEffect_Pick_Frost;
                         }
                    case "CP_VIEW_3_ENEMY_CARDS":
                         {
                              return CardEffect_View_3_Enemy;
                         }
                    case "CP_RESURECT_CARD":
                         {
                              return CardEffect_Resurect;
                         }
                    case "CP_RESURECT_FROM_ENEMY":
                         {
                              return CardEffect_Resurect_Enemy;
                         }
                    case "CP_BIN2_PICK1":
                         {
                              return CardEffect_Bin2_Pick1;
                         }
                    case "CP_MELEE_HORN":
                         {
                              return CardEffect_Melee_Horn;
                         }
                    case "CP_RANGE_HORN":
                         {
                              return CardEffect_Range_Horn;
                         }
                    case "CP_SIEGE_HORN":
                         {
                              return CardEffect_Siege_Horn;
                         }
                    case "CP_SIEGE_SCORCH":
                         {
                              return CardEffect_Siege_Scorch;
                         }
                    case "CP_COUNTER_KING_ABILITY":
                         {
                              return CardEffect_Counter_King;
                         }
                    //default, should throw an error…
                    default:
                         {
                              throw new ArgumentException("needs a string that has a return effect!");
                         }
               }
          }

          //TO DO more types
          public static int getLocation(CardSlot card)
          {
               bool row_modifier = false;
               foreach (int type in card.template.typeFlags)
               {
                    if (type == CardType_Row_Modifier)
                    {
                         row_modifier = true;
                         continue;
                    }
               }
               foreach (int type in card.template.typeFlags)
               {
                    if (row_modifier)
                    {
                         if (type == CardType_Melee)
                         {
                              return CARD_LIST_LOC_MELEEMODIFIERS;
                         }
                         else if (type == CardType_Ranged)
                         {
                              return CARD_LIST_LOC_RANGEDMODIFIERS;
                         }
                         else if (type == CardType_Siege)
                         {
                              return CARD_LIST_LOC_SEIGEMODIFIERS;
                         }
                    }
                    else
                    {
                         if (type == CardType_Melee)
                         {
                              return CARD_LIST_LOC_MELEE;
                         }
                         else if (type == CardType_Ranged)
                         {
                              return CARD_LIST_LOC_RANGED;
                         }
                         else if (type == CardType_Siege)
                         {
                              return CARD_LIST_LOC_SEIGE;
                         }
                    }
               }
               return CardType_None;
          }

          public static string attitudeToString(int attitude)
          {
               switch (attitude)
               {
                    case TACTIC_NONE:
                         {
                              return "NONE - ERROR";
                         }
                    case TACTIC_SPY_DUMMY_BEST_THEN_PASS:
                         {
                              return "DUMMY BEST THEN PASS";
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
                    default:
                         {
                              return "ERROR";
                         }
               }
          }

          public static int getCardsInHandWithEffect(ObservableCollection<CardSlot> handHolder, int effectID)
          {
               int counter = 0;
               foreach (CardSlot card in handHolder)
               {
                    if (cardHasEffect(card, effectID))
                    {
                         ++counter;
                    }
               }
               return counter;
          }

          public static bool playerHandHasEffect(ObservableCollection<CardSlot> handHolder, int effectID)
          {
               foreach (CardSlot card in handHolder)
               {
                    if (cardHasEffect(card, effectID))
                    {
                         return true;
                    }
               }
               return false;
          }

          public static bool playerBoardHasEffect(int playerID, int effectID)
          {
               ObservableCollection<CardSlot> holder;

               //check melee
               holder = MainWindow_ViewModel.mSingleton.Holders.ElementAt(playerID * 10 + CARD_LIST_LOC_MELEE);
               foreach (CardSlot card in holder)
               {
                    if (cardHasEffect(card, effectID))
                    {
                         return true;
                    }
               }

               //check ranged
               holder = MainWindow_ViewModel.mSingleton.Holders.ElementAt(playerID * 10 + CARD_LIST_LOC_RANGED);
               foreach (CardSlot card in holder)
               {
                    if (cardHasEffect(card, effectID))
                    {
                         return true;
                    }
               }

               //check siege
               holder = MainWindow_ViewModel.mSingleton.Holders.ElementAt(playerID * 10 + CARD_LIST_LOC_SEIGE);
               foreach (CardSlot card in holder)
               {
                    if (cardHasEffect(card, effectID))
                    {
                         return true;
                    }
               }

               return false;
          }

          public static bool cardHasEffect(CardSlot card, int effectID)
          {
               int counter = 0;
               while (counter < card.template.effectFlags.Count)
               {
                    if (card.template.effectFlags[counter] == effectID)
                    {
                         return true;
                    }
                    ++counter;
               }
               return false;
          }

          public static bool cardIsType(CardSlot card, int typeID)
          {
               int counter = 0;
               while (counter < card.template.typeFlags.Count)
               {
                    if (card.template.typeFlags[counter] == typeID)
                    {
                         return true;
                    }
                    ++counter;
               }
               return false;
          }

          public static List<CardSlot> playerBoardCreatures(int playerID)
          {
               List<CardSlot> result = new List<CardSlot>();
               ObservableCollection<CardSlot> holder;

               //check melee
               holder = MainWindow_ViewModel.mSingleton.Holders.ElementAt(playerID * 10 + CARD_LIST_LOC_MELEE);
               foreach (CardSlot card in holder)
               {
                    if (cardIsType(card, ValuesRepository.CardType_Creature) &&
                         !cardIsType(card, ValuesRepository.CardType_Hero))
                    {
                         result.Add(card);
                    }
               }

               //check ranged
               holder = MainWindow_ViewModel.mSingleton.Holders.ElementAt(playerID * 10 + CARD_LIST_LOC_RANGED);
               foreach (CardSlot card in holder)
               {
                    if (cardIsType(card, ValuesRepository.CardType_Creature) &&
                         !cardIsType(card, ValuesRepository.CardType_Hero))
                    {
                         result.Add(card);
                    }
               }

               //check siege
               holder = MainWindow_ViewModel.mSingleton.Holders.ElementAt(playerID * 10 + CARD_LIST_LOC_SEIGE);
               foreach (CardSlot card in holder)
               {
                    if (cardIsType(card, ValuesRepository.CardType_Creature) &&
                         !cardIsType(card, ValuesRepository.CardType_Hero))
                    {
                         result.Add(card);
                    }
               }

               return result;
          }

          public static int getPowerChange(CardSlot card)
          {
               //TO DO implement more power changes, 
               //currently Row Modifiers and Weather only

               int power = card.template.power;
               int playerID = PLAYER_INVALID;
               //check if spy
               if (cardHasEffect(card, CardEffect_Draw2))
               {
                    if (card.owningPlayer == PLAYER_1)
                    {
                         playerID = PLAYER_2;
                    }
                    else if (card.owningPlayer == PLAYER_2)
                    {
                         playerID = PLAYER_1;
                    }
               }
               else
               {
                    playerID = card.owningPlayer;
               }

               //check for modifiers for player 1
               if (playerID == PLAYER_1)
               {
                    if (cardIsType(card, CardType_Melee))
                    {
                         if (MainWindow_ViewModel.mSingleton.P1MeleeModifHolder.Count > 0)
                         {
                              power = power * 2;
                         }
                    }
                    if (cardIsType(card, CardType_Ranged))
                    {
                         if (MainWindow_ViewModel.mSingleton.P1RangeModifHolder.Count > 0)
                         {
                              power = power * 2;
                         }
                    }
                    if (cardIsType(card, CardType_Siege))
                    {
                         if (MainWindow_ViewModel.mSingleton.P1SiegeModifHolder.Count > 0)
                         {
                              power = power * 2;
                         }
                    }
               }

               //check for modifiers for player 2
               if (playerID == PLAYER_2)
               {
                    if (cardIsType(card, CardType_Melee))
                    {
                         if (MainWindow_ViewModel.mSingleton.P2MeleeModifHolder.Count > 0)
                         {
                              power = power * 2;
                         }
                    }
                    if (cardIsType(card, CardType_Ranged))
                    {
                         if (MainWindow_ViewModel.mSingleton.P2RangeModifHolder.Count > 0)
                         {
                              power = power * 2;
                         }
                    }
                    if (cardIsType(card, CardType_Siege))
                    {
                         if (MainWindow_ViewModel.mSingleton.P2SiegeModifHolder.Count > 0)
                         {
                              power = power * 2;
                         }
                    }
               }

               //Check for weather modifiers
               if (MainWindow_ViewModel.mSingleton.WeatherHolder.Count > 0)
               {
                    foreach (CardSlot weather in MainWindow_ViewModel.mSingleton.WeatherHolder)
                    {
                         if ( cardIsType(weather, CardType_Melee) &&
                              cardIsType(card, CardType_Melee))
                         {
                              power = 1;
                              return power;
                         }
                         if (cardIsType(weather, CardType_Ranged) &&
                              cardIsType(card, CardType_Ranged))
                         {
                              power = 1;
                              return power;
                         }
                         if (cardIsType(weather, CardType_Siege) &&
                              cardIsType(card, CardType_Siege))
                         {
                              power = 1;
                              return power;
                         }
                    }
               }

               return power;
          }
     }
}
