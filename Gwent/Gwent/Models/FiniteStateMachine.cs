using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Gwent.Models
{
     public class FiniteStateMachine
     {
          private Dictionary<string,FSMState> stateList;
          private String currentStateName = "";
          private String previousStateName = "";
          private String nextState = "";
          private bool disallowStateChangeFunc;
          private Timer updateTimer;

        public FiniteStateMachine()
          {
               stateList = new Dictionary<string,FSMState>();

               updateTimer = new Timer(1000);
               updateTimer.Elapsed += updateStates;
               updateTimer.Enabled = true;
          }

        private void updateStates(Object source = null, ElapsedEventArgs e = null)
          {
               if (nextState != currentStateName && disallowStateChangeFunc)
               {
                    Console.WriteLine("finite state machine: next and current are different, but disallow is true");
                    return;
               }
               if (nextState != currentStateName && stateList[nextState] != null)
               {
                    Console.WriteLine("GFX - [FSM] Switching from: {0} to: {1}", currentStateName, nextState);
                    if (currentState != "" && stateList[currentStateName] != null && stateList[currentStateName].leaveStateCallback != null)
                    {
                         stateList[currentStateName].leaveStateCallback();
                    }
                    previousStateName = currentStateName;
                    currentStateName = nextState;
                    if (stateList[nextState] != null && stateList[nextState].enterStateCallback != null)
                    {
                         stateList[nextState].enterStateCallback();
                    }
               }
               if (currentStateName == "")
               {
                    return;
               }
               if (stateList[currentStateName].updateStateCallback != null)
               {
                    stateList[currentStateName].updateStateCallback();
               }
               if (nextState != currentStateName && disallowStateChangeFunc)
               {
                    Console.WriteLine("finite state machine: next and current are different, but disallow is true #2");
                    return;
               }
               if (nextState != currentStateName && stateList[nextState] != null)
               {
                    Console.WriteLine("GFX - [FSM] Switching from: {0} to: {1}  |Round2", currentStateName, nextState);
                    if (stateList[currentStateName] != null && stateList[currentStateName].leaveStateCallback != null)
                    {
                         stateList[currentStateName].leaveStateCallback();
                    }
                    previousStateName = currentStateName;
                    currentStateName = nextState;
                    if (stateList[nextState] != null && stateList[nextState].enterStateCallback != null)
                    {
                         stateList[nextState].enterStateCallback();
                    }
               }
          }

        public void ForceUpdateState()
          {
               updateStates();
          }

        public void ChangeState(String state)
        {
            if (stateList[nextState] != null)
            {
                nextState = state;
            }
            else 
            {
                Console.WriteLine("GFX - [WARNING] Tried to change to an unknown state: {0}", state);
            }
        }

        public void AddState(String stateTag, Action enterStateCallback, Action updateStateCallback, Action leaveStateCallback)
        {
            FSMState fsmState = new FSMState();
            fsmState.stateTag = stateTag;
            fsmState.enterStateCallback = enterStateCallback;
            fsmState.updateStateCallback = updateStateCallback;
            fsmState.leaveStateCallback = leaveStateCallback;
            stateList[stateTag] = fsmState;
            if (currentStateName == "" && nextState == "") 
            {
                nextState = stateTag;
            }
        }

        public bool awaitingNextState
        {
            get {return currentStateName != nextState;}
        }

        public String currentState
        {
            get {return currentStateName;}
        }

        public String previousState
        {
             get { return previousStateName; }
        }

        public bool pauseOnStateChangeIfFunc
        {
            set {disallowStateChangeFunc = value;}
        }
     }
}
