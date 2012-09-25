using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TuringMachineSimulator.Model;
using TuringMachineSimulator.Util;
using System.Diagnostics;

namespace TuringMachineSimulator
{
    class Program
    {
        const int SWP_NOSIZE = 0x0001;

        [System.Runtime.InteropServices.DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        private static IntPtr MyConsole = GetConsoleWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        private const string appTitle = "Turing Machine v4.5 - Roland Jaggi, Christian Bachmann, Thomas Bandixen";

        public enum StepModeEnum
        {
            Slow, Normal, Fast
        }
        private static StepModeEnum _stepMode;
        private static int _timeOut;
        private static ITuringMachine tm;

        static int Main(string[] args)
        {
            SetWindowPos(MyConsole, 0, 0, 0, 0, 0, SWP_NOSIZE);
            Console.Clear();

            Console.Title = appTitle;
            Console.SetWindowSize(Console.LargestWindowWidth - 3, Console.LargestWindowHeight);

            try
            {
                if (args.Length >= 1)
                {
                    if (args[0] == "-diagram")
                    {
                        //this will launch the default browser
                        Process p = new Process() { StartInfo = new ProcessStartInfo(new FastTuringMachine(Conf.Q, Conf.E, Conf.T, Conf.Fn, Conf.S, Conf.F, "").DiagramUrl) };
                        p.Start();
                        return 0;
                    }
                }

                var inp = initInput();
                initMode();

                Console.Clear();
                Console.CursorVisible = false;

                switch (_stepMode)
                {
                    case StepModeEnum.Slow:
                        tm = new SlowTuringMachine(Conf.Q, Conf.E, Conf.T, Conf.Fn, Conf.S, Conf.F, inp);
                        break;
                    default:
                    case StepModeEnum.Normal:
                        tm = new NormalTuringMachine(Conf.Q, Conf.E, Conf.T, Conf.Fn, Conf.S, Conf.F, inp, _timeOut);
                        break;
                    case StepModeEnum.Fast:
                        tm = new FastTuringMachine(Conf.Q, Conf.E, Conf.T, Conf.Fn, Conf.S, Conf.F, inp);
                        break;
                }

                tm.Print();

                Console.ReadLine();

                try
                {
                    Console.Title = appTitle + " - running";

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (tm.MakeStep()) { }
                    sw.Stop();

                    Console.Title = appTitle;

                    tm.Print();

                    if (_stepMode == StepModeEnum.Fast)
                        Tools.WriteTime(sw.Elapsed, tm.TapeAmount);

                    Tools.WriteResult(tm.TapeAmount, tm.Output.Replace("B", " ").Trim());
                }
                catch (Exception e)
                {
                    Tools.WriteError(0, 0, e);
                }
            }
            catch (Exception e)
            {
                Tools.WriteError(0, 0, e);
            }
            Console.ReadLine();
            Console.CursorVisible = true;

            return 0;
        }

        private static void initMode()
        {
            Console.Write("StepMode ( [s]low / [N]ormal / [f]ast )\n > ");
            var i = Console.ReadKey().KeyChar;
            switch (i)
            {
                case 's':
                    _stepMode = StepModeEnum.Slow;
                    break;
                case 'n':
                    _stepMode = StepModeEnum.Normal;
                    break;
                case 'f':
                    _stepMode = StepModeEnum.Fast;
                    break;
                default:
                    _stepMode = StepModeEnum.Normal;
                    break;
            }

            if (_stepMode == StepModeEnum.Normal)
            {
                Console.Write("\nTimeout (30ms)\n > ");
                var t = Console.ReadLine();
                int ms;
                _timeOut = int.TryParse(t, out ms) ? ms : 30;
            }
        }

        private static string initInput()
        {
            Console.Write("Input (binary)\n > ");
            return Console.ReadLine();
        }
    }
}
