/* COMPLETED */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class CardTemplate
     {
          public int index;
          public int power;
          public String title;
          public String description;
          public String imageLoc;
          public int factionIdx;
          public int typeArray;
          public List<int> effectFlags;
          public List<int> summonFlags;

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
          public const int CardType_Offsensive_Effect = 1024;
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
          public const int FactionId_No_Mans_Land = 1;
          public const int FactionId_Nilfgaard = 2;
          public const int FactionId_Northern_Kingdom = 3;
          public const int FactionId_Scoiatael = 4;

          public CardTemplate()
          {

          }

          public int getFirstEffect()
          {
               if (effectFlags == null || effectFlags.Count == 0)
               {
                    return CardEffect_None;
               }
               return effectFlags[0];
          }

          public bool isType(int type)
          {
               if (type == CardType_None && typeArray != 0)
               {
                    return false;
               }
               return (typeArray & type) == type;
          }

          public bool hasEffect(int effect)
          {
               int counter = 0;
               while (counter < effectFlags.Count)
               {
                    if (effectFlags[counter] == effect)
                    {
                         return true;
                    }
                    ++counter;
               }
               return false;
          }

          public int GetBonusValue()
          {
               int counter = 0;
               int effectValue = 0;
               int bonusValue = 0;
               Dictionary<int, int> effectValueDictionary = CardManager.getInstance().cardValues.getEffectValueDictionary();
               while (counter < effectFlags.Count)
               {
                    effectValue = effectValueDictionary[effectFlags[counter]];
                    if (effectValue != 0)
                    {
                         bonusValue = bonusValue + effectValue;
                    }
                    ++counter;
               }
               return bonusValue;
          }

          public int getPlacementType()
          {
               return typeArray & CardType_SeigeRangedMelee;
          }

          public string getTypeString()
          {
               if (isType(CardType_Row_Modifier))
               {
                    return "horn";
               }
               if (isType(CardType_Weather))
               {
                    if (hasEffect(CardEffect_ClearSky) || hasEffect(CardEffect_Clear_Weather))
                    {
                         return "clearsky";
                    }
                    if (hasEffect(CardEffect_Melee))
                    {
                         return "frost";
                    }
                    if (hasEffect(CardEffect_Ranged))
                    {
                         return "fog";
                    }
                    if (hasEffect(CardEffect_Siege))
                    {
                         return "rain";
                    }
               }
               else if (isType(CardType_Spell))
               {
                    if (hasEffect(CardEffect_UnsummonDummy))
                    {
                         return "dummy";
                    }
               }
               else if (isType(CardType_Global_Effect))
               {
                    if (hasEffect(CardEffect_Scorch))
                    {
                         return "scorch";
                    }
               }
               else if (isType(CardType_Hero))
               {
                    return "Hero";
               }
               return getPlacementTypeString();
          }

          public string getPlacementTypeString()
          {
               if (isType(CardType_Creature))
               {
                    if (isType(CardType_RangedMelee))
                    {
                         return "RangedMelee";
                    }
                    if (isType(CardType_Melee))
                    {
                         return "Melee";
                    }
                    if (isType(CardType_Ranged))
                    {
                         return "Ranged";
                    }
                    if (isType(CardType_Siege))
                    {
                         return "Siege";
                    }
               }
               return "None";
          }

          public int getEffectsAsPlacementType()
          {
               int type = CardType_None;
               if (hasEffect(CardEffect_Melee))
               {
                    type = type | CardType_Melee;
               }
               if (hasEffect(CardEffect_Ranged))
               {
                    type = type | CardType_Ranged;
               }
               if (hasEffect(CardEffect_Siege))
               {
                    type = type | CardType_Siege;
               }
               return type;
          }

          public string getEffectString()
          {
               if (isType(CardType_Creature))
               {
                    if (hasEffect(CardEffect_SummonClones))
                    {
                         return "summonClones";
                    }
                    if (hasEffect(CardEffect_Nurse))
                    {
                         return "nurse";
                    }
                    if (hasEffect(CardEffect_Draw2))
                    {
                         return "spy";
                    }
                    if (hasEffect(CardEffect_SameTypeMorale))
                    {
                         return "stMorale";
                    }
                    if (hasEffect(CardEffect_ImproveNeighbours))
                    {
                         return "impNeighbours";
                    }
                    if (hasEffect(CardEffect_Horn))
                    {
                         return "horn";
                    }
                    if (isType(CardType_RangedMelee))
                    {
                         return "agile";
                    }
                    if (hasEffect(CardEffect_MeleeScorch))
                    {
                         return "scorch";
                    }
               }
               return "None";
          }

          public int getCreatureType()
          {
               return typeArray & CardType_SeigeRangedMelee;
          }

          public string tooltipIcon
          {
               get
               {
                    if (isType(CardType_Row_Modifier))
                    {
                         return "horn";
                    }
                    if (isType(CardType_Weather))
                    {
                         if (hasEffect(CardEffect_ClearSky) || hasEffect(CardEffect_Clear_Weather))
                         {
                              return "clearsky";
                         }
                         if (hasEffect(CardEffect_Melee))
                         {
                              return "frost";
                         }
                         if (hasEffect(CardEffect_Ranged))
                         {
                              return "fog";
                         }
                         if (hasEffect(CardEffect_Siege))
                         {
                              return "rain";
                         }
                    }
                    else if (isType(CardType_Spell))
                    {
                         if (hasEffect(CardEffect_UnsummonDummy))
                         {
                              return "dummy";
                         }
                    }
                    else if (isType(CardType_Global_Effect))
                    {
                         if (hasEffect(CardEffect_Scorch))
                         {
                              return "scorch";
                         }
                    }
                    else if (isType(CardType_Creature))
                    {
                         if (hasEffect(CardEffect_SummonClones))
                         {
                              return "summonClones";
                         }
                         if (hasEffect(CardEffect_Nurse))
                         {
                              return "nurse";
                         }
                         if (hasEffect(CardEffect_Draw2))
                         {
                              return "spy";
                         }
                         if (hasEffect(CardEffect_SameTypeMorale))
                         {
                              return "stMorale";
                         }
                         if (hasEffect(CardEffect_ImproveNeighbours))
                         {
                              return "impNeighbours";
                         }
                         if (hasEffect(CardEffect_Horn))
                         {
                              return "horn";
                         }
                         if (isType(CardType_RangedMelee))
                         {
                              return "agile";
                         }
                         if (hasEffect(CardEffect_MeleeScorch))
                         {
                              return "scorch";
                         }
                    }
                    return "None";
               }
          }

          public string tooltipString
          {
               get
               {
                    if (isType(CardType_Row_Modifier))
                    {
                         return "gwint_card_tooltip_horn";
                    }
                    if (isType(CardType_Weather))
                    {
                         if (hasEffect(CardEffect_ClearSky) || hasEffect(CardEffect_Clear_Weather))
                         {
                              return "gwint_card_tooltip_clearsky";
                         }
                         if (hasEffect(CardEffect_Melee))
                         {
                              return "gwint_card_tooltip_frost";
                         }
                         if (hasEffect(CardEffect_Ranged))
                         {
                              return "gwint_card_tooltip_fog";
                         }
                         if (hasEffect(CardEffect_Siege))
                         {
                              return "gwint_card_tooltip_rain";
                         }
                    }
                    else if (isType(CardType_Spell))
                    {
                         if (hasEffect(CardEffect_UnsummonDummy))
                         {
                              return "gwint_card_tooltip_dummy";
                         }
                    }
                    else if (isType(CardType_Global_Effect))
                    {
                         if (hasEffect(CardEffect_Scorch))
                         {
                              return "gwint_card_tooltip_scorch";
                         }
                    }
                    else if (isType(CardType_Creature))
                    {
                         if (hasEffect(CardEffect_SummonClones))
                         {
                              return "gwint_card_tooltip_summon_clones";
                         }
                         if (hasEffect(CardEffect_Nurse))
                         {
                              return "gwint_card_tooltip_nurse";
                         }
                         if (hasEffect(CardEffect_Draw2))
                         {
                              return "gwint_card_tooltip_spy";
                         }
                         if (hasEffect(CardEffect_SameTypeMorale))
                         {
                              return "gwint_card_tooltip_same_type_morale";
                         }
                         if (hasEffect(CardEffect_ImproveNeighbours))
                         {
                              return "gwint_card_tooltip_improve_neightbours";
                         }
                         if (hasEffect(CardEffect_Horn))
                         {
                              return "gwint_card_tooltip_horn";
                         }
                         if (isType(CardType_RangedMelee))
                         {
                              return "gwint_card_tooltip_agile";
                         }
                         if (isType(CardType_Hero))
                         {
                              return "gwint_card_tooltip_hero";
                         }
                         if (hasEffect(CardEffect_MeleeScorch))
                         {
                              return "gwint_card_villen_melee_scorch";
                         }
                    }
                    else if (isType(CardType_None))
                    {
                         int firstEffect = getFirstEffect();
                         switch (firstEffect)
                         {
                              case CardTemplate.CardEffect_Clear_Weather:
                                   {
                                        return "gwint_card_tooltip_ldr_clear_weather";
                                   }
                              case CardTemplate.CardEffect_Pick_Fog:
                                   {
                                        return "gwint_card_tooltip_ldr_pick_fog";
                                   }
                              case CardTemplate.CardEffect_Siege_Horn:
                                   {
                                        return "gwint_card_tooltip_ldr_siege_horn";
                                   }
                              case CardTemplate.CardEffect_Siege_Scorch:
                                   {
                                        return "gwint_card_tooltip_ldr_siege_scorch";
                                   }
                              case CardTemplate.CardEffect_Pick_Frost:
                                   {
                                        return "gwint_card_tooltip_ldr_pick_frost";
                                   }
                              case CardTemplate.CardEffect_Range_Horn:
                                   {
                                        return "gwint_card_tooltip_ldr_range_horn";
                                   }
                              case CardTemplate.CardEffect_11th_card:
                                   {
                                        return "gwint_card_tooltip_ldr_eleventh_card";
                                   }
                              case CardTemplate.CardEffect_MeleeScorch:
                                   {
                                        return "gwint_card_tooltip_ldr_melee_scorch";
                                   }
                              case CardTemplate.CardEffect_Pick_Rain:
                                   {
                                        return "gwint_card_tooltip_ldr_pick_rain";
                                   }
                              case CardTemplate.CardEffect_View_3_Enemy:
                                   {
                                        return "gwint_card_tooltip_ldr_view_enemy";
                                   }
                              case CardTemplate.CardEffect_Resurect_Enemy:
                                   {
                                        return "gwint_card_tooltip_ldr_resurect_enemy";
                                   }
                              case CardTemplate.CardEffect_Counter_King:
                                   {
                                        return "gwint_card_tooltip_ldr_counter_king";
                                   }
                              case CardTemplate.CardEffect_Bin2_Pick1:
                                   {
                                        return "gwint_card_tooltip_ldr_bin_pick";
                                   }
                              case CardTemplate.CardEffect_Pick_Weather:
                                   {
                                        return "gwint_card_tooltip_ldr_pick_weather";
                                   }
                              case CardTemplate.CardEffect_Resurect:
                                   {
                                        return "gwint_card_tooltip_ldr_resurect";
                                   }
                              case CardTemplate.CardEffect_Melee_Horn:
                                   {
                                        return "gwint_card_tooltip_ldr_melee_horn";
                                   }
                         }
                    }
                    return "";
               }
          }

          public string toString()
          {
               return "[Gwint CardTemplate] index:" + index + ", title:" + title + ", imageLoc:" + imageLoc + ", power:" + power + ", facionIdx:" + factionIdx + ", type:" + typeArray + ", effectString: " + getEffectString();
          }

          public int GetDeployBonusValue()
          {
               int bonusValue = 0;
               Dictionary<int, int> effectValueDictionary = CardManager.getInstance().cardValues.getEffectValueDictionary();
               if (hasEffect(CardTemplate.CardEffect_Draw))
               {
                    bonusValue = bonusValue + effectValueDictionary[CardTemplate.CardEffect_Draw];
               }
               if (hasEffect(CardTemplate.CardEffect_Draw2))
               {
                    bonusValue = bonusValue + effectValueDictionary[CardTemplate.CardEffect_Draw2];
               }
               if (hasEffect(CardTemplate.CardEffect_SummonClones))
               {
                    bonusValue = bonusValue + effectValueDictionary[CardTemplate.CardEffect_SummonClones];
               }
               if (hasEffect(CardTemplate.CardEffect_Nurse))
               {
                    bonusValue = bonusValue + effectValueDictionary[CardTemplate.CardEffect_Draw];
               }
               return bonusValue;
          }

          public string getFactionString()
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
     }
}
