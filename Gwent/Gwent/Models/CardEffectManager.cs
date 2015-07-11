/* COMPLETE */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class CardEffectManager
     {
          private List<CardInstance> seigeP2List;
          private List<CardInstance> rangedP2List;
          private List<CardInstance> meleeP2List;
          private List<CardInstance> meleeP1List;
          private List<CardInstance> rangedP1List;
          private List<CardInstance> seigeP1List;

          public CardEffectManager()
          {
               seigeP2List = new List<CardInstance>();
               rangedP2List = new List<CardInstance>();
               meleeP2List = new List<CardInstance>();
               meleeP1List = new List<CardInstance>();
               rangedP1List = new List<CardInstance>();
               seigeP1List = new List<CardInstance>();
          }

          public void flushAllEffects()
          {
               meleeP1List = new List<CardInstance>();
               meleeP2List = new List<CardInstance>();
               rangedP1List = new List<CardInstance>();
               rangedP2List = new List<CardInstance>();
               seigeP1List = new List<CardInstance>();
               seigeP2List = new List<CardInstance>();
               return;
          }

          private List<CardInstance> getEffectList(int cardLocation, int playerID)
          {
               if (playerID == CardManager.PLAYER_1)
               {
                    if (cardLocation == CardManager.CARD_LIST_LOC_MELEE)
                    {
                         return meleeP1List;
                    }
                    if (cardLocation == CardManager.CARD_LIST_LOC_RANGED)
                    {
                         return rangedP1List;
                    }
                    if (cardLocation == CardManager.CARD_LIST_LOC_SEIGE)
                    {
                         return seigeP1List;
                    }
               }
               if (playerID == CardManager.PLAYER_2)
               {
                    if (cardLocation == CardManager.CARD_LIST_LOC_MELEE)
                    {
                         return meleeP2List;
                    }
                    if (cardLocation == CardManager.CARD_LIST_LOC_RANGED)
                    {
                         return rangedP2List;
                    }
                    if (cardLocation == CardManager.CARD_LIST_LOC_SEIGE)
                    {
                         return seigeP2List;
                    }
               }
               return null;
          }

          public void registerActiveEffectCardInstance(CardInstance cardInstance, int listID, int playerID)
          {
               List<CardInstance> effectList = getEffectList(listID, playerID);
               Console.WriteLine("GFX - effect was registed in list:", listID, ", for playerID:", playerID, " and CardInstance:", cardInstance);
               if (effectList != null)
               {
                    effectList.Add(cardInstance);
               }
               else
               {
                    throw new ArgumentException("GFX - Failed to set effect into proper list in GFX manager. listID: " + listID.ToString() + ", playerID: " + playerID);
               }
               CardManager.getInstance().recalculateScores();
          }

          public void unregisterActiveEffectCardInstance(CardInstance cardInstance)
          {
               int index = 0;
               Console.WriteLine("GFX - unregistering Effect: {0}", cardInstance);
               index = seigeP2List.IndexOf(cardInstance);
               if (index != -1)
               {
                    seigeP2List.RemoveRange(index, 1);
               }
               index = rangedP2List.IndexOf(cardInstance);
               if (index != -1)
               {
                    rangedP2List.RemoveRange(index, 1);
               }
               index = meleeP2List.IndexOf(cardInstance);
               if (index != -1)
               {
                    meleeP2List.RemoveRange(index, 1);
               }
               index = meleeP1List.IndexOf(cardInstance);
               if (index != -1)
               {
                    meleeP1List.RemoveRange(index, 1);
               }
               index = rangedP1List.IndexOf(cardInstance);
               if (index != -1)
               {
                    rangedP1List.RemoveRange(index, 1);
               }
               index = seigeP1List.IndexOf(cardInstance);
               if (index != -1)
               {
                    seigeP1List.RemoveRange(index, 1);
               }
               CardManager.getInstance().recalculateScores();
               return;
          }

          public List<CardInstance> getEffectsForList(int listID, int playerID)
          {
               int counter = 0;
               List<CardInstance> resultList = new List<CardInstance>();
               List<CardInstance> effectList = getEffectList(listID, playerID);
               if (effectList != null)
               {
                    counter = 0;
                    while (counter < effectList.Count)
                    {
                         resultList.Add(effectList[counter]);
                         ++counter;
                    }
               }
               return resultList;
          }
     }
}
