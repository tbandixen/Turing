using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TuringMachineSimulator.Util;

namespace TuringMachineSimulator.Model
{
    public class TuringMachine
    {
        public enum StepModeEnum
        {
            Slow, Normal, Fast
        }
        public StepModeEnum StepMode { get; set; }


        public string Input { get; private set; }

        public IList<string> PossibleStates { get; private set; }
        public IList<string> PossibleInputs { get; private set; }
        public IList<string> PossibleOutputs { get; private set; }
        public Dictionary<string, StateFunction> FunctionMap { get; private set; }
        public string CurrentState { get; set; }
        public IList<string> FinalStates { get; private set; }
        public IList<Tape> Tapes = new List<Tape>();
        public int TapeAmount { get; private set; }

        public long Steps { get; private set; }

        public int StepsOver { get; set; }
        public int TimeOut { get; set; }

        public TuringMachine(string possibleStates, string possibleInputs, string possibleOutputs, string functions, string startState, string finalStates)
        {
            this.PossibleStates = clean(possibleStates).Split(',');
            this.PossibleInputs = clean(possibleInputs).Split(',');
            this.PossibleOutputs = clean(possibleOutputs).Split(',');
            this.CurrentState = startState.Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();
            this.FinalStates = clean(finalStates).Split(',');

            Steps = 0;

            initFunctions(clean(functions).Replace("),(", ")~(").Split('~'));
        }

        public void SetInput(string input)
        {
            this.Input = input;
            for (int i = 0; i < TapeAmount; i++)
                Tapes.Add(new Tape(Tapes.Count, i == 0 ? input : "B"));
        }

        public void Print()
        {
            foreach (var t in Tapes)
            {
                t.Print();
            }
        }

        public bool MakeStep()
        {
            Steps++;
            StringBuilder sb = new StringBuilder();
            foreach (var tape in Tapes)
                sb.Append(tape.Read());

            var read = sb.ToString();

            var key = CurrentState + read;
            StateFunction fn;
            try
            {
                fn = FunctionMap[key];
            }
            catch (Exception)
            {
                if (FinalStates.Contains(CurrentState))
                    return false;
                throw new TuringException(string.Format("Kann keine Übergangsfunktion für \"({0}, {1})\" finden.", CurrentState, read));
            }

            if (StepMode != StepModeEnum.Fast)
                Tools.UpdateDiagram(FunctionMap.Values.ToList(), fn);

            CurrentState = fn.NewState;
            fn.Used = true;
            if (StepMode != StepModeEnum.Fast)
                wait();
            for (int i = 0; i < TapeAmount; i++)
            {
                Tapes[i].Write(fn.Write[i]);
                if (StepMode != StepModeEnum.Fast)
                    Tapes[i].Print();
            }
            if (StepMode != StepModeEnum.Fast)
                wait();
            for (int i = 0; i < TapeAmount; i++)
            {
                Tapes[i].Move(fn.Movement[i]);
                if (StepMode != StepModeEnum.Fast)
                    Tapes[i].Print();
            }
            if (StepMode != StepModeEnum.Fast)
                wait();
            return true;
        }

        private void wait()
        {
            switch (StepMode)
            {
                case StepModeEnum.Slow:
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
                    break;
                case StepModeEnum.Normal:
                    var ts = new TimeSpan(0, 0, 0, 0, TimeOut);
                    System.Threading.Thread.Sleep(ts);
                    break;
            }
        }

        private void initFunctions(IList<string> list)
        {
            FunctionMap = new Dictionary<string, StateFunction>();
            // init state functions
            foreach (var item in list)
            {
                var p = item.Substring(1, item.Length - 2).Replace(")=(", ",").Replace("(", "").Replace(")", "").Split(',');
                if (TapeAmount == 0)
                    TapeAmount = p[1].Length;
                if (Conf.Q.Contains(p[0]))
                {
                    if (Conf.Q.Contains(p[2]))
                    {
                        foreach (var c in p[1])
                        {
                            if (!Conf.E.Contains(c.ToString()))
                            {
                                throw new TuringException(string.Format("{0} ist kein gültiges Lesezeichen.", c));
                            }
                        }
                        foreach (var c in p[3])
                        {
                            if (!Conf.T.Contains(c.ToString()))
                            {
                                throw new TuringException(string.Format("{0} ist kein gültiges Schreibzeichen.", c));
                            }
                        }
                        foreach (var c in p[4])
                        {
                            Model.StateFunction.Direction tmp;
                            if (!Enum.TryParse<Model.StateFunction.Direction>(c.ToString(), out tmp))
                            {
                                throw new TuringException(string.Format("{0} ist keine gültige Bewegung.", c));
                            }
                        }

                        FunctionMap.Add(p[0] + p[1], new StateFunction(this, p[0], p[1], p[2], p[3], p[4])); ;
                    }
                    else
                    {
                        throw new TuringException(string.Format("{0} ist kein gültiger Zustand.", p[2]));
                    }
                }
                else
                {
                    throw new TuringException(string.Format("{0} ist kein gültiger Zustand.", p[0]));
                }
            }
        }

        private static string clean(string s)
        {
            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();
            return s.Substring(1, s.Length - 2);
        }
    }
}
