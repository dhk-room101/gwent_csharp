/* movieclips/sounds/text */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Gwent.Models
{
     public class CardFXManager
     {
          //private var weatherParent:flash.display.MovieClip;
          //public var weatherMeleeP1Ongoing:flash.display.MovieClip;
          //public var weatherMeleeP2Ongoing:flash.display.MovieClip;
          //public var weatherRangedP1Ongoing:flash.display.MovieClip;
          //public var weatherRangedP2Ongoing:flash.display.MovieClip;
          //public var weatherSeigeP1Ongoing:flash.display.MovieClip;
          //public var weatherSeigeP2Ongoing:flash.display.MovieClip;

          private Dictionary<string, string> effectDictionary;
          public Dictionary<string, string> activeFXDictionary;

          private bool weatherMeleeOngoing_Active = false;
          private bool weatherSeigeOngoing_Active = false;
          private bool weatherRangedOngoing_Active = false;

          protected Timer hidingWeatherMeleeTimer;
          protected Timer hidingWeatherRangedTimer;
          protected Timer hidingWeatherSiegeTimer;

          protected float _rowEffectX;
          protected float _seigeEnemyRowEffectY;
          protected float _rangedEnemyRowEffectY;
          protected float _meleeEnemyRowEffectY;
          protected float _meleePlayerRowEffectY;
          protected float _rangedPlayerRowEffectY;
          protected float _seigePlayerRowEffectY;

          private int numEffectsPlaying = 0;
          private static int _instanceIDGenerator = 0;

          protected static CardFXManager _instance;

          public object _placeSeigeFXClassRef;
          public object _placeMeleeFXClassRef;
          public object _hornFXClassRef;
          public object _placeRangedFXClassRef;
          public object _scorchFXClassRef;
          public object _dummyFXClassRef;
          public object _clearWeatherFXClassRef;
          public object _rainFXClassRef;
          public object _hornRowFXClassRef;
          public object _fogFXClassRef;
          public object _summonClonesArriveFXClassRef;
          public object _frostFXClassRef;
          public object _tightBondsFXClassRef;
          public object _moraleBoostFXClassRef;
          public object _summonClonesFXClassRef;
          public object _resurrectFXClassRef;
          public object _placeHeroFXClassRef;
          public object _placeSpyFXClassRef;

          protected string _clearWeatherFXName;
          protected string _hornFXName;
          protected string _scorchFXName;
          protected string _dummyFXName;
          protected string _placeMeleeFXName;
          protected string _placeRangedFXName;
          protected string _moraleBoostFXName;
          protected string _tightBondsFXName;
          protected string _frostFXName;
          protected string _summonClonesArriveFX;
          protected string _fogFXName;
          protected string _hornRowFXName;
          protected string _rainFXName;
          protected string _summonClonesFXName;
          protected string _resurrectFXName;
          protected string _placeHeroFXName;
          protected string _placeSpyFXName;
          protected string _placeSeigeFXName;

          public CardFXManager()
          {
               _instance = this;
          }

          public static CardFXManager getInstance()
          {
               return _instance;
          }

          public bool isPlayingAnyCardFX()
          {
               return numEffectsPlaying > 0;
          }

          public void ShowWeatherOngoing(int listID, bool isEnabled)
          {
               Console.WriteLine("GFX -------------------------------------------------------===================================");
               Console.WriteLine("GFX - ShowWeatherOngoing called for slot: " + listID + ", with value: " + isEnabled);
          }

          public void spawnFX(CardInstance card, Action a, object o)
          {
               Console.WriteLine("spawnFX not implemented yet!");
          }
     }
}
