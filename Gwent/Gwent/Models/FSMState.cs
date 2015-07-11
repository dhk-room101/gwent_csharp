using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gwent.Models
{
     public class FSMState
     {
          public String stateTag;
          public Action enterStateCallback;
          public Action updateStateCallback;
          public Action leaveStateCallback;
          
          public FSMState()
          {

          }
     }
}
