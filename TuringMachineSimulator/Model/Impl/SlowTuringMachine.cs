using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TuringMachineSimulator.Util;

namespace TuringMachineSimulator.Model
{
    public class SlowTuringMachine : BaseTuringMachine
    {
        public SlowTuringMachine(string possibleStates, string possibleInputs, string possibleOutputs, string functions, string startState, string finalStates, string input)
            : base(possibleStates, possibleInputs, possibleOutputs, functions, startState, finalStates, input)
        {
            StepsOver = 0;
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

            StepsOver--;
            Tools.UpdateDiagram(FunctionMap.Values.ToList(), fn);
            Tools.WriteSteps(++Steps, TapeAmount);
            Tools.WriteState(CurrentState, TapeAmount);
            return true;
        }

        private void wait()
        {
            if (StepsOver <= 0)
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 2);
                var l = Console.ReadLine();
                int t;
                int.TryParse(l, out t);
                StepsOver = t;
                var newl = (from c in l
                            select ' ').ToArray();
                Tools.Write(0, Console.WindowHeight - 2, new string(newl));
            }
        }
    }
}
