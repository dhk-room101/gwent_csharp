using Gwent.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gwent.Converters
{
     class ConverterIconType : IValueConverter
     {
          // This converts the result object to the foreground.
          public object Convert(object value, Type targetType,
              object parameter, System.Globalization.CultureInfo culture)
          {
               List<int> typeFlags = value as List<int>;
               int typeID = 0;
               string result = @"Images\Icons\";

               foreach (int t in typeFlags)
               {
                    if (t < 8 && typeID == 0)
                    {
                         typeID = t;
                    }
                    else
                    {
                         //TO DO implement Melee/Ranged multiple types
                         Console.WriteLine("type converter: multiple types!");
                    }
               }
               
               switch (typeID)
               {
                    case ValuesRepository.CardType_Melee:
                         {
                              result += "icon_melee.png";
                              return result;
                         }
                    case ValuesRepository.CardType_Ranged:
                         {
                              result += "icon_ranged.png";
                              return result;
                         }
                    case ValuesRepository.CardType_Siege:
                         {
                              result += "icon_siege.png";
                              return result;
                         }
                    default:
                         {
                              return null;
                         }
               }
          }

          // No need to implement converting back on a one-way binding 
          public object ConvertBack(object value, Type targetType,
              object parameter, System.Globalization.CultureInfo culture)
          {
               throw new NotImplementedException();
          }
     }
}
