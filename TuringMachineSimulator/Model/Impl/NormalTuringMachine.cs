using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TuringMachineSimulator.Util;

namespace TuringMachineSimulator.Model
{
    public class NormalTuringMachine : BaseTuringMachine
    {
        public NormalTuringMachine(string possibleStates, string possibleInputs, string possibleOutputs, string functions, string startState, string finalStates, string input, int timeOut)
            : base(possibleStates, possibleInputs, possibleOutputs, functions, startState, finalStates, input)
        {
            this.TimeOut = timeOut;
        }

        public override bool MakeStep()
        {
            StateFunction fn = GetPossibleFunction();
            if (fn == null) return false;

            CurrentState = fn.NewState;
            fn.Used = true;
            wait();
            for (int i = 0; i < TapeAmount; i++)
            {
                Tapes[i].Write(fn.Write[i]);
                Tapes[i].Print();
            }
            wait();
            for (int i = 0; i < TapeAmount; i++)
            {
                Tapes[i].Move(fn.Movement[i]);
                Tapes[i].Print();
            }
            wait();

            Tools.UpdateDiagram(FunctionMap.Values.ToList(), fn);
            Tools.WriteSteps(++Steps, TapeAmount);
            Tools.WriteState(CurrentState, TapeAmount);
            return true;
        }

        private void wait()
        {
            var ts = new TimeSpan(0, 0, 0, 0, TimeOut);
            System.Threading.Thread.Sleep(ts);
        }
    }
}
