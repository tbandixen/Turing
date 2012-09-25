using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using TuringMachineSimulator.Model;

namespace TuringMachineSimulator.Util
{
    public static class Tools
    {
        public static void Write(int left, int top, string s)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(s);
        }
        public static void WriteBandBorder(int amount)
        {
            Write(0, 0, "+");
            Write(Conf.VisibleBandSize + 3, 0, "+");
            Write(0, amount * 2 + 2, "+");
            Write(Conf.VisibleBandSize + 3, amount * 2 + 2, "+");
            for (int i = 0; i < Conf.VisibleBandSize + 2; i++)
            {
                Write(i + 1, 0, "-");
                Write(i + 1, amount * 2 + 2, "-");
            }

            for (int i = 0; i < 2 * amount + 1; i++)
            {
                Write(0, i + 1, (i % 2 == 0) ? "|" : "+[");
                Write(Conf.VisibleBandSize + 2, i + 1, (i % 2 == 0) ? " |" : "]+");
            }
        }
        public static void WriteInput(string input, int amount)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var n in input.Split('x'))
            {
                var ds = Binary2DecimalString(n);
                sb.AppendFormat("{0} x ", ds > 999 ? ds.ToString("#,000") : ds.ToString());
            }
            Write(0, 2 * amount + 3, string.Format(" Input:\t\t\t{0} ({1}) ", input, sb.ToString().Substring(0, sb.ToString().Length - 3)));
        }
        public static void WriteState(string state, int amount)
        {
            Write(0, 2 * amount + 4, string.Format(" Zustand:\t\t{0} ", state));
        }
        public static void WriteSteps(long steps, int amount)
        {
            Write(0, 2 * amount + 5, string.Format(" Schritt:\t\t{0} ", steps > 999 ? steps.ToString("#,000") : steps.ToString()));
        }
        public static void WriteTime(TimeSpan time, int amount)
        {
            Write(0, 2 * amount + 6, string.Format(" Zeit:\t\t\t{0}", time.ToString()));
        }
        public static void WriteResult(int amount, string bin)
        {
            ConsoleColor f = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.White;

            BigInteger res = Binary2DecimalString(bin);
            var resString = res.ToString();

            Console.SetCursorPosition(0, 2 * amount + 7);
            Console.WriteLine(string.Format(" Resultat (Bin):\t{0}", bin));
            Console.WriteLine(string.Format(" Resultat (Dec):\t{0}", res.ToString("E")));
            Console.WriteLine(string.Format(" Resultat (Dec):\t{0}", res > 999 ? res.ToString("#,000") : res.ToString()));

            Console.ForegroundColor = f;
        }
        public static void WriteError(int left, int top, Exception e)
        {
            ConsoleColor b = Console.BackgroundColor;
            ConsoleColor f = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            if (e is TuringException)
                Console.Write("Fehler: {0}", e.Message.ToString());
            else
                Console.Write("Fehler: {0}", e.ToString());

            Console.BackgroundColor = b;
            Console.ForegroundColor = f;
        }
        public static void WriteDiagram(IList<StateFunction> list)
        {
            foreach (var fn in list)
            {
                string m = string.Empty;
                foreach (var mov in fn.Movement)
                {
                    m += mov;
                }
                var s = string.Format("({0}, {1}) = ({2}, {3}, {4})", fn.CurrentState, fn.Read, fn.NewState, fn.Write, m);
                Tools.Write(Console.WindowWidth - 50, list.IndexOf(fn), s);
            }
        }
        private static StateFunction highlightedFn;
        public static void UpdateDiagram(IList<StateFunction> list, StateFunction fn)
        {
            if (highlightedFn != null)
            {
                string mOld = string.Empty;
                foreach (var mov in highlightedFn.Movement)
                {
                    mOld += mov;
                }
                var sOld = string.Format("({0}, {1}) = ({2}, {3}, {4})", highlightedFn.CurrentState, highlightedFn.Read, highlightedFn.NewState, highlightedFn.Write, mOld);
                //TODO
                Tools.Write(Console.WindowWidth - 50, list.IndexOf(highlightedFn), sOld);
            }

            ConsoleColor f = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;

            string m = string.Empty;
            foreach (var mov in fn.Movement)
            {
                m += mov;
            }
            var s = string.Format("({0}, {1}) = ({2}, {3}, {4})", fn.CurrentState, fn.Read, fn.NewState, fn.Write, m);
            Tools.Write(Console.WindowWidth - 50, list.IndexOf(fn), s);

            highlightedFn = fn;

            Console.ForegroundColor = f;
        }
        public static BigInteger Binary2DecimalString(string bin)
        {
            BigInteger resultat = new BigInteger();

            for (int i = 0; i < bin.Length; i++)
                resultat = BigInteger.Add(resultat, BigInteger.Parse(bin[i].ToString()) * BigInteger.Pow(2, bin.Length - i - 1));

            return resultat;
        }
    }
}
