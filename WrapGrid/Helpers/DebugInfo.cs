using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WrapGrid.Helpers
{
    internal static class DebugInfo
    {
        private const string ControlName = "WrapGrid";

        public static void Log(string message)
        {
            Debug.WriteLine(string.Format("{0}:{1}", ControlName, message));
        }
    }
}
