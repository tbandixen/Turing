using System;
using System.Collections.Generic;
using System.Text;
using TuringMachineSimulator.Util;

namespace TuringMachineSimulator.Model
{
    public class Tape
    {
        public int BandNr { get; private set; }

        private int _listIndex = 0;
        private readonly LinkedList<char> _list = new LinkedList<char>();
        private LinkedListNode<char> _current;

        public Tape(int bandNr, IEnumerable<char> content)
        {
            BandNr = bandNr;
            foreach (var c in content)
                _list.AddLast(new LinkedListNode<char>(c));

            _current = _list.First;
        }
        public void Move(StateFunction.Direction d)
        {
            switch (d)
            {
                case StateFunction.Direction.L:
                    if (_current.Previous == null)
                    {
                        _list.AddBefore(_current, new LinkedListNode<char>('B'));
                        _listIndex++;
                    }
                    _current = _current.Previous;
                    _listIndex--;
                    break;
                case StateFunction.Direction.R:
                    if (_current.Next == null) {
                        _list.AddAfter(_current, new LinkedListNode<char>('B'));
                    }
                    _current = _current.Next;
                    _listIndex++;
                    break;
            }
        }
        public char Read()
        {
            return _current.Value;
        }
        public void Write(char c)
        {
            _current.Value = c;
        }

        public void Print()
        {
            int offsetLeft = 2;
            int offsetTop = 2 * BandNr + 2;

            string s = VisibleArea.Replace('B', ' ');
            int i = (int)Math.Floor(Conf.VisibleBandSize / 2f);
            string s1 = s.Substring(0, i);
            string s2 = s.Substring(i, 1);
            string s3 = s.Substring(i + 1);

            ConsoleColor b = Console.BackgroundColor;
            ConsoleColor f = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            Tools.Write(offsetLeft, offsetTop, s1);

            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;

            Tools.Write(offsetLeft + s1.Length, offsetTop, s2);

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            Tools.Write(offsetLeft + s1.Length + s2.Length, offsetTop, s3);

            Console.BackgroundColor = b;
            Console.ForegroundColor = f;
        }

        public string VisibleArea
        {
            get
            {
                char[] area = new char[Conf.VisibleBandSize];
                int middle = ((int)Math.Floor(Conf.VisibleBandSize / 2f)) - _listIndex;
                var value = Value;
                for (int i = 0; i < area.Length; i++)
                {
                    if (i >= middle && i < (middle + value.Length))
                        area[i] = value[i - middle];
                    else
                        area[i] = ' ';
                }

                return new string(area);
            }
        }
        public string Value
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var c in _list)
                    sb.Append(c);

                return sb.ToString();
            }
        }
    }
}
