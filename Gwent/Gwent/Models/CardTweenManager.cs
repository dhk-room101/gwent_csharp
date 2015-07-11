using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class CardTweenManager
     {
          protected const double FLIP_TIMELAPSE_DURATION = 0.5;
          protected const double FLIP_TIMELAPSE_SCALE = 1.4;
          protected const double FLIP_MIN_SCALE = 0.001;
          protected const double FLIP_SCALE = 1.2;
          protected const double DEFAULT_TWEEN_DURATION = 1;
          protected const double MOVE_TWEEN_SPEED = 2000;
          protected Dictionary<int, int> _cardTweens;
          protected Dictionary<int, int> _cardPositions;
          protected static CardTweenManager _instance;
          
          public CardTweenManager()
          {

          }

          public static CardTweenManager getInstance()
          {
               if (_instance == null)
               {
                    _instance = new CardTweenManager();
               }
               return _instance;
          }

          public bool isAnyCardMoving()
          {
               //TO DO conditions true
               return false;
          }
     }
}
