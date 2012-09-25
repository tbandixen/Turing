using System;
using System.Collections.Generic;
using System.Configuration;

namespace TuringMachineSimulator.Util
{
    public static class Conf
    {

        public static string Q { get { return ConfigurationManager.AppSettings["Q"]; } }
        public static string E { get { return ConfigurationManager.AppSettings["E"]; } }
        public static string T { get { return ConfigurationManager.AppSettings["T"]; } }
        public static string Fn { get { return ConfigurationManager.AppSettings["Fn"]; } }
        public static string S { get { return ConfigurationManager.AppSettings["S"]; } }
        public static string F { get { return ConfigurationManager.AppSettings["F"]; } }

        public static int VisibleBandSize
        {
            get
            {
                int _visibleBandSize;
                int.TryParse(ConfigurationManager.AppSettings["VisibleBandSize"], out _visibleBandSize);
                return _visibleBandSize;
            }
        }

        public static int OutputTape
        {
            get
            {
                int i;
                int.TryParse(ConfigurationManager.AppSettings["OutputTape"], out i);
                return i;
            }
        }
    }
}
