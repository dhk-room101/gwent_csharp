﻿/* COMPLETE */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class GwintRoundResult
     {
          private List<int> roundScores;
          private int roundWinner;

          public GwintRoundResult()
          {

          }

          public bool played
          {
               get { return roundScores != null; }
          }

          public int getPlayerScore(int id)
          {
               if (played && id != CardManager.PLAYER_INVALID)
               {
                    return roundScores[id];
               }
               return -1;
          }

          public void reset()
          {
               roundScores = null;
          }

          public int winningPlayer
          {
               get
               {
                    if (roundWinner != -1)
                    {
                         return roundWinner;
                    }
                    if (played)
                    {
                         if (roundScores[CardManager.PLAYER_1] == roundScores[CardManager.PLAYER_2])
                         {
                              return CardManager.PLAYER_INVALID;
                         }
                         if (roundScores[CardManager.PLAYER_1] > roundScores[CardManager.PLAYER_2])
                         {
                              return CardManager.PLAYER_1;
                         }
                         return CardManager.PLAYER_2;
                    }
                    return CardManager.PLAYER_INVALID;
               }
          }

          public void setResults(int p1, int p2, int w)
          {
               if (played)
               {
                    throw new ArgumentException("GFX - Tried to set round results on a round that already had results!");
               }
               roundScores = new List<int>();
               roundScores.Add(p1);
               roundScores.Add(p2);
               roundWinner = w;
          }

          public string toString()
          {
               if (roundScores != null)
               {
                    return "[ROUND RESULT] p1Score: " + roundScores[0] + ", p2Score: " + roundScores[1] + ", roundWinner: " + roundWinner;
               }
               return "[ROUND RESULT] empty!";
          }
     }
}
