using Gwent.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Gwent.Converters
{
     class ConverterIconEffect : IValueConverter
     {
          // This converts the result object to the foreground.
          public object Convert(object value, Type targetType,
              object parameter, System.Globalization.CultureInfo culture)
          {
               List<int> effectFlags = value as List<int>;
               string result = @"Images\Icons\";
               
               switch (effectFlags[0])
               {
                    case ValuesRepository.CardEffect_Draw2:
                         {
                              result += "icon_spy.png";
                              return result;
                         }
                    case ValuesRepository.CardEffect_Horn:
                         {
                              result += "icon_horn.png";
                              return result;
                         }
                    case ValuesRepository.CardEffect_SummonClones:
                         {
                              result += "icon_clones.png";
                              return result;
                         }
                    case ValuesRepository.CardEffect_SameTypeMorale:
                         {
                              result += "icon_neighbors.png";
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
