using Mordritch.Transpiler.Compilers;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.src.Compilers.TypeScript
{
    /// <summary>
    /// Used to track dependancies on types
    /// </summary>
    public class OtherTypes
    {
        public struct UsageDetails
        {
            public IInputElement InputElement;

            public string ContextDescription;
        }

        public static IDictionary<string, IList<UsageDetails>> TypeList = new Dictionary<string, IList<UsageDetails>>();

        public static IList<string> ToBeCompiledList = new List<string>();

        public static string BasePath;

        public static void AddToList(IInputElement inputElement, string contextDescription)
        {
            if (ToBeCompiledList.Any(x => x == inputElement.Data))
            {
                return;
            }

            if (!TypeList.ContainsKey(inputElement.Data))
            {
                TypeList.Add(inputElement.Data, new List<UsageDetails>());
            }

            TypeList[inputElement.Data].Add(new UsageDetails
            {
                InputElement = inputElement,
                ContextDescription = contextDescription
            });

        }

        public static void DumpList()
        {
            var counter = 1;
            var margin = "      ";


            var list = TypeList
                .Where(x => File.Exists(BasePath + x.Key + ".java"))
                .OrderBy(x => x.Key)
                .Select(x1 => counter++ + ". " + x1.Key + Environment.NewLine + x1.Value
                    .Select(z => margin + FormatPosition(z.InputElement) + " (" + z.ContextDescription + ")")
                    .Aggregate((xx, yy) => xx + Environment.NewLine + yy))
                .Aggregate((x, y) => x + Environment.NewLine + y);

            Console.WriteLine(list);
            Console.WriteLine();
            Console.WriteLine("================================================================================================");
            Console.WriteLine();

            counter = 1;
            var fileExistsList = TypeList
                .Where(x => !File.Exists(BasePath + x.Key + ".java"))
                .OrderBy(x => x.Key)
                .Select(x1 => counter++ + ". " + x1.Key + Environment.NewLine + x1.Value
                    .Select(z => margin + FormatPosition(z.InputElement) + " (" + z.ContextDescription + ")")
                    .Aggregate((xx, yy) => xx + Environment.NewLine + yy))
                .Aggregate((x, y) => x + Environment.NewLine + y);

            Console.WriteLine(fileExistsList);
        }

        private static string FormatPosition(IInputElement inputElement)
        {
            return string.Format("{0} {1}", inputElement.Source, inputElement.Line + 1);
        }
    }
}
