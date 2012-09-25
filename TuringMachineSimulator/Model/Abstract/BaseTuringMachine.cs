using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TuringMachineSimulator.Util;

namespace TuringMachineSimulator.Model
{
    public abstract class BaseTuringMachine : ITuringMachine
    {
        public string Input { get; private set; }

        public IList<string> PossibleStates { get; private set; }
        public IList<string> PossibleInputs { get; private set; }
        public IList<string> PossibleOutputs { get; private set; }
        public Dictionary<string, StateFunction> FunctionMap { get; private set; }
        public string CurrentState { get; set; }
        public IList<string> FinalStates { get; private set; }
        public IList<Tape> Tapes = new List<Tape>();
        public int TapeAmount { get; private set; }

        public long Steps { get; protected set; }
        public int StepsOver { get; set; }
        public int TimeOut { get; set; }

        public BaseTuringMachine(string possibleStates, string possibleInputs, string possibleOutputs, string functions, string startState, string finalStates, string input)
        {
            this.PossibleStates = clean(possibleStates).Split(',');
            this.PossibleInputs = clean(possibleInputs).Split(',');
            this.PossibleOutputs = clean(possibleOutputs).Split(',');
            this.CurrentState = startState.Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();
            this.FinalStates = clean(finalStates).Split(',');

            Steps = 0;

            this.Input = input;

            var tmp = input;
            foreach (var item in PossibleInputs)
                tmp = tmp.Replace(item, "");

            if (!string.IsNullOrEmpty(tmp))
                throw new TuringException("Input wird nicht akzeptiert.");

            initFunctions(clean(functions).Replace("),(", ")~(").Split('~'));

            for (int i = 0; i < TapeAmount; i++)
                Tapes.Add(new Tape(Tapes.Count, i == 0 ? input : "B"));

            Tools.WriteBandBorder(TapeAmount);
            Tools.WriteState(CurrentState, TapeAmount);
            Tools.WriteDiagram(FunctionMap.Values.ToList());
            Tools.WriteInput(Input, TapeAmount);
        }

        protected StateFunction GetPossibleFunction()
        {
            StringBuilder sb = new StringBuilder(CurrentState);
            for (int i = 0; i < TapeAmount; i++)
                sb.Append(Tapes[i].Read());

            var read = sb.ToString();

            try
            {
                return FunctionMap[read];
            }
            catch (Exception)
            {
                if (FinalStates.Contains(CurrentState))
                    return null;
                throw new TuringException(string.Format("Kann keine Übergangsfunktion für \"({0}, {1})\" finden.", CurrentState, read.Replace(CurrentState, "")));
            }
        }

        public string Output
        {
            get
            {
                return Tapes[Conf.OutputTape - 1].Value;
            }
        }

        public string DiagramUrl
        {
            get
            {
                var l = from fn in FunctionMap.Values.ToList()
                        select new { fn.CurrentState, fn.NewState };

                StringBuilder sb = new StringBuilder();
                sb.Append("http://yuml.me/diagram/scruffy;/activity/" + string.Format("(start)->({0})", Conf.S));

                foreach (var item in l.Distinct())
                {
                    sb.Append(",");
                    sb.AppendFormat("({0})->({1})", item.CurrentState, item.NewState);
                }
                foreach (var item in FinalStates)
                {
                    sb.Append(",");
                    sb.AppendFormat("({0})->(end)", item);
                }
                return sb.ToString();
            }
        }


        public void Print()
        {
            for (int i = 0; i < TapeAmount; i++)
            {
                Tapes[i].Print();
            }
        }

        public abstract bool MakeStep();

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
                                throw new TuringException(string.Format("{0} ist kein gültiges Lesezeichen.", c));
                        }
                        foreach (var c in p[3])
                        {
                            if (!Conf.T.Contains(c.ToString()))
                                throw new TuringException(string.Format("{0} ist kein gültiges Schreibzeichen.", c));
                        }
                        foreach (var c in p[4])
                        {
                            Model.StateFunction.Direction tmp;
                            if (!Enum.TryParse<Model.StateFunction.Direction>(c.ToString(), out tmp))
                                throw new TuringException(string.Format("{0} ist keine gültige Bewegung.", c));
                        }

                        FunctionMap.Add(p[0] + p[1], new StateFunction(this, p[0], p[1], p[2], p[3], p[4]));
                    }
                    else
                        throw new TuringException(string.Format("{0} ist kein gültiger Zustand.", p[2]));
                }
                else
                    throw new TuringException(string.Format("{0} ist kein gültiger Zustand.", p[0]));
            }
        }

        private static string clean(string s)
        {
            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();
            return s.Substring(1, s.Length - 2);
        }
    }
}
