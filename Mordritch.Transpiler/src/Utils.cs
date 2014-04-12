using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler
{
    public static class Utils
    {
        public static bool LoggingEnabled = true;
        
        public static int Indent = 0;

        public static void ConditionalPause(bool condition)
        {
            if (condition)
            {
                Pause();
            }
        }

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
            if (!LoggingEnabled)
            {
                return;
            }

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

        public static string WrapText(string text, int limit)
        {
            var words = text.Split(' ');
            var output = new StringBuilder();

            string line = string.Empty;
            foreach (string word in words)
            {
                if ((line + word).Length > limit)
                {
                    output.AppendLine(line);
                    line = string.Empty;
                }

                line += string.Format("{0} ", word);
            }

            if (line.Length > 0)
            {
                output.AppendLine(line);
            }

            return output.ToString();
        }
    }
}
