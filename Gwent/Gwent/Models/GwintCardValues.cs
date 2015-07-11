/* COMPLETE */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class GwintCardValues
     {
          public int weatherCardValue;
          public int hornCardValue;
          public int drawCardValue;
          public int scorchCardValue;
          public int summonClonesCardValue;
          public int unsummonCardValue;
          public int improveNeighboursCardValue;
          public int nurseCardValue;
          private Dictionary<int, int> _bufferedDictionary;

          public GwintCardValues()
          {
          }

          public Dictionary<int, int> getEffectValueDictionary()
          {
               if (_bufferedDictionary == null)
               {
                    _bufferedDictionary = new Dictionary<int, int>();
                    _bufferedDictionary[CardTemplate.CardEffect_Horn] = hornCardValue;
                    _bufferedDictionary[CardTemplate.CardEffect_Draw] = drawCardValue;
                    _bufferedDictionary[CardTemplate.CardEffect_Draw2] = drawCardValue * 2;
                    _bufferedDictionary[CardTemplate.CardEffect_Scorch] = scorchCardValue;
                    _bufferedDictionary[CardTemplate.CardEffect_SummonClones] = summonClonesCardValue;
                    _bufferedDictionary[CardTemplate.CardEffect_UnsummonDummy] = unsummonCardValue;
                    _bufferedDictionary[CardTemplate.CardEffect_ImproveNeighbours] = improveNeighboursCardValue;
                    _bufferedDictionary[CardTemplate.CardEffect_Nurse] = nurseCardValue;
               }
               return _bufferedDictionary;
          }
     }
}
