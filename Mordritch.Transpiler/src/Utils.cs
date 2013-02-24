using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler
{
    public static class Utils
    {
        public static int Indent = 0;
        
        public static void Pause()
        {
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        public static void Log(string data)
        {
            Console.WriteLine("".PadLeft(Indent) + data);
        }

        public static void Log(string data, ConsoleColor textColor)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = textColor;
            Log(data);
            Console.ForegroundColor = originalColor;
        }

        public static void IncreaseIndent()
        {
            Indent += 2;
        }

        public static void DecreaseIndent()
        {
            Indent -= 2;
        }
    }
}
