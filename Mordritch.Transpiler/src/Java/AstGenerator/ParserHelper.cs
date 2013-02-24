using Mordritch.Transpiler.Java.AstGenerator.Parsers;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator
{
    public static class ParserHelper
    {
        public static IAstNode Parse<TParser>(InputElementDataSource inputElementDataSource) where TParser : IParser, new()
        {
            PreMessage<TParser>(inputElementDataSource);

            while (inputElementDataSource.IsWhiteSpace)
            {
                inputElementDataSource.Pointer++;
            }

            var parser = new TParser();
            var returnValue = parser.Parse(inputElementDataSource);

            PostMessage<TParser>(returnValue);

            return returnValue;
        }

        public static void PreMessage<TParser>(InputElementDataSource inputElementDataSource)
        {
            Utils.Log(string.Format(
                "--> [{0},{1}] {2}",
                (inputElementDataSource.GetCurrentInputElement().Line + 1).ToString().PadLeft(3),
                (inputElementDataSource.GetCurrentInputElement().Column + 1).ToString().PadLeft(3),
                typeof(TParser).Name), ConsoleColor.DarkGray);
            Utils.IncreaseIndent();
        }

        public static void PostMessage<TParser>(IAstNode astNode)
        {
            Utils.Log(astNode.DebugOut(), ConsoleColor.Yellow);
            Utils.DecreaseIndent();
            Utils.Log(string.Format("<-- {0}", typeof(TParser).Name), ConsoleColor.DarkGray);
        }

        public static IAstNode Parse<TParser>(IList<IInputElement> inputElements) where TParser : IParser, new()
        {
            var inputElementDataSource = new InputElementDataSource(inputElements);
            var parser = new TParser();
            return parser.Parse(inputElementDataSource);
        }
    }
}
