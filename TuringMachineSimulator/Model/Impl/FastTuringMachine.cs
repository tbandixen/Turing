using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TuringMachineSimulator.Util;

namespace TuringMachineSimulator.Model
{
    public class FastTuringMachine : BaseTuringMachine
    {
        public FastTuringMachine(string possibleStates, string possibleInputs, string possibleOutputs, string functions, string startState, string finalStates, string input)
            : base(possibleStates, possibleInputs, possibleOutputs, functions, startState, finalStates, input)
        { }

        public override bool MakeStep()
        {
            StateFunction fn = GetPossibleFunction();
            if (fn == null)
            {
                Tools.WriteSteps(Steps, TapeAmount);
                Tools.WriteState(CurrentState, TapeAmount);
                return false;
            }

            CurrentState = fn.NewState;
            fn.Used = true;
            for (int i = 0; i < TapeAmount; i++)
            {
                Tapes[i].Write(fn.Write[i]);
                Tapes[i].Move(fn.Movement[i]);
            }

            ++Steps;
            return true;
        }
    }
}
