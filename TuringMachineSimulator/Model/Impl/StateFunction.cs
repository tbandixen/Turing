using System;
using System.Collections.Generic;

namespace TuringMachineSimulator.Model
{
    public class StateFunction
    {
        public enum Direction
        {
            L,
            R,
            N
        }
        public string CurrentState { get; private set; }
        public string NewState { get; private set; }
        public string Read { get; private set; }
        public string Write { get; private set; }
        public IList<Direction> Movement { get; private set; }
        public bool Used { get; set; }

        public StateFunction(ITuringMachine tm, string currentState, string read, string newState, string write, string movement)
        {
            if ((read + write + movement).Trim().Length != tm.TapeAmount * 3)
                throw new ArgumentException("Parameter sind nicht korrekt", "read, write, movement");

            CurrentState = currentState;
            NewState = newState;
            Read = read;
            Write = write;
            Movement = new List<Direction>();

            for (int i = 0; i < movement.Length; i++)
            {
                Movement.Add((Direction)Enum.Parse(typeof(Direction), movement[i].ToString()));
            }
        }
    }
}
