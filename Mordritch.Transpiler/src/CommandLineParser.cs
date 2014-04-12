using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.src
{
    public static class CommandLineParser
    {
        private class Option
        {
            public string Name { get; set; }
            public string Help { get; set; }
            public bool Mandatory { get; set; }
            public bool IsFlagOption { get; set; }
            public Action<string> CallbackAction { get; set; }
            public Action FlagCallbackAction { get; set; }
        }

        private static IList<Option> Options = new List<Option>();

        private const string PREFIX = "-";

        public static void AddOption(string name, Action<string> callbackAction, bool mandatory = false)
        {
            AddOption(name, string.Empty, callbackAction, mandatory);
        }

        public static void AddOption(string name, string help, Action<string> callbackAction, bool mandatory = false)
        {
            Options.Add(new Option
            {
                Name = string.Format("{0}{1}", PREFIX, name),
                Help = help,
                CallbackAction = callbackAction,
                Mandatory = mandatory
            });
        }

        public static void AddFlagOption(string name, Action callbackAction, bool mandatory = false)
        {
            AddFlagOption(name, string.Empty, callbackAction, mandatory);
        }

        public static void AddFlagOption(string name, string help, Action callbackAction, bool mandatory = false)
        {
            Options.Add(new Option
            {
                Name = string.Format("{0}{1}", PREFIX, name),
                Help = help,
                IsFlagOption = true,
                FlagCallbackAction = callbackAction,
                Mandatory = mandatory
            });
        }

        public static void Parse(string[] args)
        {
            var usedOptions = new List<Option>();
            var currentArgument = 0;

            while (currentArgument < args.Length)
            {
                var argumentName = args[currentArgument];
                if (argumentName == string.Format("{0}{1}", PREFIX, "help"))
                {
                    throw new Exception(GetHelp());
                }
                
                var option = Options.FirstOrDefault(x => x.Name == argumentName);
                if (option == null)
                {
                    throw new Exception(string.Format("Unrecognized option '{0}', use {1}help to see options.", argumentName, PREFIX));
                }

                usedOptions.Add(option);

                if (option.IsFlagOption)
                {
                    option.FlagCallbackAction();
                    currentArgument++;
                    continue;
                }

                if (!option.IsFlagOption)
                {
                    ParseString(args, currentArgument, option);
                    currentArgument++;
                    currentArgument++;
                    continue;
                }
            }

            var missingMandatoryOptions = Options.Where(x => x.Mandatory && usedOptions.All(y => y != x));
            if (missingMandatoryOptions.Any())
            {
                var missingMandatoryOptionsString = missingMandatoryOptions.Select(x => x.Name).Aggregate((x, y) => x + ", " + y);
                throw new Exception(string.Format("The following mandatory options were not specified: {0}", missingMandatoryOptionsString));
            }
        }

        private static void ParseString(string[] args, int currentArgument, Option option)
        {
            if (currentArgument + 1 >= args.Length)
            {
                throw new Exception(string.Format("Too few arguments, expected additional argument after.", option.Name));
            }

            var argumentValue = args[currentArgument + 1];
            option.CallbackAction(argumentValue);
        }

        private static string GetHelp()
        {
            var helpString = new StringBuilder();
            var longestOption = Options.Select(x => x.Name.Length).OrderByDescending(x => x).First();
            var emtpyLinePrefix = new String(' ', longestOption +  3);

            helpString.AppendLine("Current options available:");

            foreach (var option in Options)
            {
                var helpText = option.Mandatory
                    ? string.Format("(Mandatory) {0}", option.Help)
                    : option.Help;

                helpText = Utils.WrapText(helpText, Console.WindowWidth - emtpyLinePrefix.Length - 2);
                helpText = string.Format(" {0}  {1}", option.Name.PadRight(longestOption), helpText);
                helpText = helpText.Replace(Environment.NewLine, string.Format("{0}{1}", Environment.NewLine, emtpyLinePrefix));

                helpString.AppendLine(helpText);
                helpString.AppendLine();
            }

            return helpString.ToString();
        }
    }
}
