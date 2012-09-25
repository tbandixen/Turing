using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TuringMachineSimulator.Model
{
    public interface ITuringMachine
    {
        int TapeAmount { get; }
        string CurrentState { get; }
        int TimeOut { get; set; }

        string Input { get; }
        string Output { get; }

        void Print();
        bool MakeStep();

        string DiagramUrl { get; }
    }
}
